using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Hexamap;
using System.Collections.Generic;

public class MapManager : Manager, IListener
{
    [SerializeField] List<MapBase> Maps;
    private Dictionary<Type, MapBase> MapDic;

    private Camera mainCamera;

    bool canPlayerMove = false;
    bool isDronePrepared = false;
    public bool CanClick => !canPlayerMove && !isDronePrepared;

    public TileBase tileCtrl;
    private TileBase targetTile;
    private TileBase showInfoTile;

    private List<TileBase> selectedTiles = new();
    private List<Tile> neighborTiles = new();
    private List<Tile> sightTiles = new();

    private int playerLayer;
    private int tileLayer;

    Ray ray;

    public bool isMapActive = false;

    public List<Tile> AllTile { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        MapDic = new(Maps.Count);

        foreach (var Map in Maps)
        {
            MapDic.Add(Map.GetUnitType(), Map);
        }

        Maps.Clear(); // clear memory

        App.Manager.Event.AddListener(EventCode.NextDayStart, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.NextDayStart:
                NextDay();
                break;
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;

        AllTile = App.Manager.Asset.Hexamap.Map.Tiles.Where(x => x.GameEntity.CompareTag("Tile")).ToList();

        targetTile = App.Manager.Asset.Hexamap.Map.GetTileFromCoords(new Coords(0, 0)).Ctrl;

        UpdateCurrentTile();

        InitMaps();
        InitSight();
        InitValue();

        App.Manager.Event.PostEvent(EventCode.GameStart, this);

        AllTile.Clear(); //clear memory

        playerLayer = 1 << LayerMask.NameToLayer("Player");
        tileLayer = 1 << LayerMask.NameToLayer("Tile");
}

    private void InitValue()
    {
        canPlayerMove = false;
        isDronePrepared = false;

        neighborTiles.Clear();
        neighborTiles = App.Manager.Asset.Hexamap.Map.GetTilesInRange(tileCtrl.Model, App.Data.Test.Buff.moveRange);

        AllBorderOff();
        ReInitSight();
    }

    #region Occlusion Culling
    private void InitSight()
    {
        foreach (var tile in AllTile)
        {
            tile.GameEntity.SetActive(false);
        }
    }

    private void ReInitSight()
    {
        foreach (var tile in sightTiles)
        {
            tile.GameEntity.SetActive(false);
        }

        sightTiles = App.Manager.Asset.Hexamap.Map.GetTilesInRange(tileCtrl.Model, 5);
        sightTiles.Add(tileCtrl.Model);

        foreach (var tile in sightTiles)
        {
            tile.GameEntity.SetActive(true);
            tile.Ctrl.UpdateResource();
        }
    }
    #endregion

    #region Units
    private void InitMaps()
    {
        foreach (var Map in MapDic.Values)
        {
            try { Map.Init(); }
            catch (Exception error)
            { Debug.LogError($"ERROR: {error.Message}\n{error.StackTrace}"); }
        }
    }

    private void ReInitMaps()
    {
        foreach (var Map in MapDic.Values)
        {
            try { Map.ReInit(); }
            catch (Exception error)
            { Debug.LogError($"ERROR: {error.Message}\n{error.StackTrace}"); }
        }
    }
    #endregion

    #region Get Unit
    public T GetUnit<T>() where T : MapBase => (T)MapDic[typeof(T)];

    public bool TryGetUnit<T>(out T _unit) where T : MapBase
    {
        if (MapDic.TryGetValue(typeof(T), out var unit))
        {
            _unit = (T)unit;
            return true;
        }

        _unit = default;
        return false;
    }
    #endregion

    #region Update
    private void Update()
    {
        if (!isMapActive) return;

        ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            LeftClickEvent();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            RightClickEvent();
        }
        else
        {
            MouseOverEvents();
        }
    }

    private void MouseOverEvents()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        AllBorderOff();

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, tileLayer))
        {
            TileBase hitTile = hit.transform.parent.GetComponent<TileBase>();

            if (hitTile != showInfoTile) 
                    App.Manager.UI.GetPanel<MapPanel>().SetInfoActive(false);

            if (canPlayerMove)
            {
                SetTileBorder(hitTile);
                selectedTiles.Add(hitTile);
            }
            else if (isDronePrepared)
            {
                GetUnit<DroneUnit>().SetPath(hitTile);
            }
        }
        else
        {
            App.Manager.UI.GetPanel<MapPanel>().SetInfoActive(false);
        }
    }

    private void LeftClickEvent()
    {
        if (Physics.Raycast(ray, Mathf.Infinity, playerLayer))
        {
            if (!isDronePrepared)
            {
                canPlayerMove = true;
            }
        }
        else if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, tileLayer))
        {
            TileBase tileController = hit.transform.GetComponentInParent<TileBase>();

            if (!canPlayerMove && !isDronePrepared)
            {
                if (App.Manager.Asset.Hexamap.Map.GetTilesInRange(tileCtrl.Model, 2).Contains(tileController.Model))
                {
                    App.Manager.UI.GetPanel<MapPanel>().SetInfoActive(true);
                    tileController.UpdateTileInfo();
                    showInfoTile = tileController;
                }
            }
            else if (canPlayerMove)
            {
                if (CanSetTargetTile(tileController))
                {
                    SetTargetTile(tileController);
                    canPlayerMove = false;
                }
            }
            else if (isDronePrepared)
            {
                GetUnit<DroneUnit>().Install(tileController);
            }
        }
    }

    private void RightClickEvent()
    {
        if (isDronePrepared)
        {
            GetUnit<DroneUnit>().Cancel();
        }

        canPlayerMove = false;
        isDronePrepared = false;

        CancelTargetTile();
    }
    #endregion

    public void NextDay()
    {
        UpdateCurrentTile();

        ReInitMaps();

        InitValue();

        App.Manager.Event.PostEvent(EventCode.NextDayMiddle, this);
    }

    private void UpdateCurrentTile()
    {
        tileCtrl = targetTile;

        foreach (var Map in MapDic.Values)
        {
            try { Map.SetTile(tileCtrl); }
            catch (Exception error)
            { Debug.LogError($"ERROR: {error.Message}\n{error.StackTrace}"); }
        }

        tileCtrl.SetBuff();

        App.Manager.Event.PostEvent(EventCode.TileUpdate, this, tileCtrl);
    }

    public void SetRandomTile()
    {
        var tiles = App.Manager.Asset.Hexamap.Map.GetTilesInRange(tileCtrl.Model, 1);

        foreach (var tile in tiles)
        {
            if (tile.Ctrl.canMove)
            {
                targetTile = tile.Ctrl;
                return;
            }
        }
    }

    private void SetTileBorder(TileBase tileController)
    {
        foreach (var tile in neighborTiles)
        {
            selectedTiles.Add(tile.Ctrl);
            tile.Ctrl.Border.BorderOn(TileState.None);
        }
        
        if (neighborTiles.Contains(tileController.Model))
        {
            var state = tileController.isAccessable == false || !tileController.canMove ?
                TileState.Unable : TileState.Moveable;

            tileController.Border.BorderOn(state);
        }
        else
        {
            tileController.Border.BorderOn(TileState.Unable);
        }
    }

    private bool CanSetTargetTile(TileBase tileController) 
        => tileController.Border.TileState == TileState.Moveable;

    private void SetTargetTile(TileBase tileController)
    {
        targetTile = tileController;
        GetUnit<ArrowUnit>().ArrowOn(tileController.transform.position);
        AllBorderOff();
    }

    private void CancelTargetTile()
    {
        targetTile = tileCtrl;
        GetUnit<ArrowUnit>().ArrowOff();
        AllBorderOff();
    }

    private void AllBorderOff()
    {
        foreach (var tile in selectedTiles)
        {
            tile.Border.BorderOff();
        }

        selectedTiles.Clear();
    }

    public string GetLandformBGM() => tileCtrl.GetTileType().ToString() switch
    {
        "None" => "Ambience_City",
        "Jungle" => "Ambience_Jungle",
        "Desert" => "Ambience_Desert",
        "Tundra" => "Ambience_Tundra",
        _ => "Ambience_City",
    };
}