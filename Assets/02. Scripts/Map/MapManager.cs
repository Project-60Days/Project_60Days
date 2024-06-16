using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Hexamap;
using System.Collections.Generic;

public class MapManager : Manager
{
    [SerializeField] List<MapBase> Maps;
    private Dictionary<Type, MapBase> MapDic;

    public TileController tileCtrl;
    public MapCamCtrl cameraCtrl;

    Camera mainCamera;
    TileController curTileController;

    bool canPlayerMove = false;
    bool isDronePrepared = false;
    public bool canClick => !canPlayerMove && !isDronePrepared;

    List<TileController> selectedTiles = new();
    List<Tile> neighborTiles = new();
    List<Tile> sightTiles = new();

    private TileController targetTile;

    int playerLayer;
    int tileLayer;

    Ray ray;

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
    }

    private void Start()
    {
        mainCamera = Camera.main;

        AllTile = App.Manager.Asset.Hexamap.Map.Tiles.Where(x => x.GameEntity.CompareTag("Tile")).ToList();

        targetTile = App.Manager.Asset.Hexamap.Map.GetTileFromCoords(new Coords(0, 0)).Ctrl;
        UpdateCurrentTile();

        InitMaps();
        InitSight();
        cameraCtrl.Init();
        InitValue();

        App.Manager.UI.GetPanel<LoadingPanel>().ClosePanel();

        playerLayer = 1 << LayerMask.NameToLayer("Player");
        tileLayer = 1 << LayerMask.NameToLayer("Tile");

        AllTile.Clear(); //clear memory
    }

    public void InitValue()
    {
        canPlayerMove = false;
        isDronePrepared = false;

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
            tile.Ctrl.Base.UpdateResource();
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

    public void ReInitMaps()
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
    void Update()
    {
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

    void MouseOverEvents()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        AllBorderOff();

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, tileLayer))
        {
            TileController hitTile = hit.transform.parent.GetComponent<TileController>();

            if (hitTile != curTileController) 
                    App.Manager.UI.GetPanel<MapPanel>().SetInfoActive(false);

            if (canPlayerMove)
            {
                TilePathFinderSurroundings(hitTile);
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

    void LeftClickEvent()
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
            TileController tileController = hit.transform.parent.GetComponent<TileController>();

            if (!canPlayerMove && !isDronePrepared)
            {
                if (App.Manager.Asset.Hexamap.Map.GetTilesInRange(tileCtrl.Model, 2).Contains(tileController.Model))
                {
                    tileController.Base.UpdateTileInfo();
                    App.Manager.UI.GetPanel<MapPanel>().SetInfoActive(true);
                    curTileController = tileController;
                }
            }
            else if (canPlayerMove)
            {
                if (SelectPlayerMovePoint(tileController))
                {
                    SetTargetTile(tileController);
                    canPlayerMove = false;
                }
                else
                    return;
            }
            else if (isDronePrepared)
            {
                GetUnit<DroneUnit>().Install(tileController);
            }
        }
    }

    void RightClickEvent()
    {
        if (isDronePrepared)
        {
            GetUnit<DroneUnit>().Cancel();
        }

        canPlayerMove = false;
        isDronePrepared = false;

        CancleTargetTile();
    }

    private void SetTargetTile(TileController tileController)
    {
        targetTile = tileController;
        GetUnit<ArrowUnit>().ArrowOn(tileController.transform.position);
        AllBorderOff();
    }

    private void CancleTargetTile()
    {
        targetTile = tileCtrl;
        GetUnit<ArrowUnit>().ArrowOff();
        AllBorderOff();
    }
    #endregion

    public void NextDay()
    {
        tileCtrl.Base.SetBuff();

        UpdateCurrentTile();

        ReInitMaps();

        InitValue();
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

        neighborTiles.Clear();
        neighborTiles = App.Manager.Asset.Hexamap.Map.GetTilesInRange(tileCtrl.Model, App.Manager.Test.Buff.moveRange);
    }

    public void DefaultMouseOverState(TileController tileController)
    {
        if (!tileController.Base.canMove)
        {
            SelectBorder(tileController, ETileState.Unable);
        }
        else if (tileController != null && !selectedTiles.Contains(tileController))
        {
            SelectBorder(tileController, ETileState.Unable);
        }
    }

    public void SetRandomTile()
    {
        var tiles = App.Manager.Asset.Hexamap.Map.GetTilesInRange(tileCtrl.Model, 1);

        foreach (var tile in tiles)
        {
            if (tile.Ctrl.Base.canMove)
            {
                tileCtrl = tile.Ctrl;
                return;
            }
        }
    }

    public void TilePathFinderSurroundings(TileController tileController)
    {
        for (var index = 0; index < neighborTiles.Count; index++)
        {
            var value = neighborTiles[index];

            if (!tileController.Base.canMove)
                continue;

            selectedTiles.Add(value.Ctrl);
            SelectBorder(value.Ctrl, ETileState.None);
        }

        if (tileController.Base.isAccessable == false
            || !tileController.Base.canMove)
        {
            SelectBorder(tileController, ETileState.Unable);
        }
        else if (neighborTiles.Contains(tileController.Model) == false)
        {
            SelectBorder(tileController, ETileState.Unable);
        }
        else if (tileController.Base.isZombie)
        {
            SelectBorder(tileController, ETileState.Unable);
        }
        else if (neighborTiles.Contains(tileController.Model))
        {
            SelectBorder(tileController, ETileState.Moveable);
        }
    }

    public bool SelectPlayerMovePoint(TileController tileController)
    {
        if (tileController.Base.GetEtileState() == ETileState.Moveable
            && tileCtrl.Model != tileController.Model
            && tileController.Base.canMove)
        {
            return true;
        }
        else
            return false;
    }

    public void SelectBorder(TileController tileController, ETileState state)
    {
        if (state != ETileState.Target) 
            tileController.Base.BorderOn(state);

        selectedTiles.Add(tileController);
    }

    private void AllBorderOff()
    {
        foreach (var tile in selectedTiles)
        {
            tile.Base.OffNormalBorder();
        }

        selectedTiles.Clear();
    }

    public string GetLandformBGM() => tileCtrl.Base.GetTileType().ToString() switch
    {
        "None" => "Ambience_City",
        "Jungle" => "Ambience_Jungle",
        "Desert" => "Ambience_Desert",
        "Tundra" => "Ambience_Tundra",
        _ => "Ambience_City",
    };
}