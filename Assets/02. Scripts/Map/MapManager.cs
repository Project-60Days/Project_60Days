using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Hexamap;
using FischlWorks_FogWar;
using System.Collections.Generic;

[Serializable]
public class MapData
{
    public int resourcePercent = 70;

    public int enemyDetectRange = 3;

    public int zombieCount = 80;

    public int durability = 200;
}

[Serializable]
public class BuffData
{
    public int fogSightRange = 4;

    public int moveRange = 2;

    public int resourceCount = 2;

    public bool canDetect = true;
}


public class MapManager : Manager
{
    [SerializeField] List<MapBase> Maps;
    private Dictionary<Type, MapBase> MapDic;

    public HexamapController hexaMapCtrl;

    public TileController tileCtrl;
    public MapCamCtrl cameraCtrl;

    Camera mainCamera;
    TileController curTileController;

    bool canPlayerMove = false;
    bool isDronePrepared = false;
    public bool canClick => !canPlayerMove && !isDronePrepared;

    [SerializeField] Transform mapParentTransform;

    [Header("안개")]
    [Space(5f)]
    public csFogWar fog;

    public List<TileController> selectedTiles = new List<TileController>();

    List<Tile> sightTiles = new();

    public TileController targetTile;

    int playerLayer;
    int tileLayer;

    Ray ray;

    public BuffData Buff { get; private set; }
    private BuffData defaultBuff;

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
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Buff = defaultBuff = App.Manager.Test.Buff;

        GenerateMap();
        AllBorderOff();
        InitMaps();
        InitSight();
        cameraCtrl.Init();
        InitValue();
        fog.Add(GetUnit<PlayerUnit>().PlayerTransform, Buff.fogSightRange, true);

        playerLayer = 1 << LayerMask.NameToLayer("Player");
        tileLayer = 1 << LayerMask.NameToLayer("Tile");
    }

    private void GenerateMap()
    {
        FastNoise _fastNoise = new();

        _fastNoise.SetFrequency(0.1f);
        _fastNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
        _fastNoise.SetSeed(hexaMapCtrl.Map.Seed);

        foreach (Tile tile in hexaMapCtrl.Map.Tiles)
        {
            var noiseY = _fastNoise.GetValue(tile.Coords.X, tile.Coords.Y);
            (tile.GameEntity as GameObject).transform.position += new Vector3(0, noiseY * 2, 0);
        }

        mapParentTransform.position = Vector3.forward * 200f;

        UpdateCurrentTile(GetTileController(hexaMapCtrl.Map.GetTileFromCoords(new Coords(0, 0))));
        targetTile = tileCtrl;

        App.Manager.UI.GetPanel<LoadingPanel>().ClosePanel();
    }

    public void InitValue()
    {
        canPlayerMove = false;
        isDronePrepared = false;
    }

    #region Occlusion Culling
    public void InitSight()
    {
        var allTiles = GetAllTiles();

        foreach (var tile in allTiles)
        {
            ((GameObject)tile.GameEntity).SetActive(false);
        }

        ReInitSight(tileCtrl.Model);
    }

    public void ReInitSight(Tile _targetTile)
    {
        foreach (var tile in sightTiles)
        {
            ((GameObject)tile.GameEntity).SetActive(false);
        }

        sightTiles = GetTilesInRange(5, _targetTile);
        sightTiles.Add(_targetTile);

        foreach (var tile in sightTiles)
        {
            ((GameObject)tile.GameEntity).SetActive(true);
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
        if (EventSystem.current.IsPointerOverGameObject())
        {
            AllBorderOff();
            return;
        }

        TileController hitTile;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, tileLayer))
        {
            hitTile = hit.transform.parent.GetComponent<TileController>();

            AllBorderOff();

            if (!CheckTileInSight(hitTile))
            {
                App.Manager.UI.GetPanel<MapPanel>().SetInfoActive(false);
                return;
            }

            if (canClick)
            {
                DefaultMouseOverState(hitTile);

                if (hitTile != curTileController)
                    App.Manager.UI.GetPanel<MapPanel>().SetInfoActive(false);
            }

            else if (canPlayerMove)
            {
                TilePathFinderSurroundings(hitTile);
                selectedTiles.Add(hitTile);
            }

            else if (isDronePrepared)
            {
                GetUnit<DroneUnit>().SetPath(hitTile);
            }

            curTileController = hitTile;
        }
        else
        {
            AllBorderOff();
            App.Manager.UI.GetPanel<MapPanel>().SetInfoActive(false);
        }
    }

    void LeftClickEvent()
    {
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);

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
                if (App.Manager.Map.GetTilesInRange(2).Contains(tileController.Model))
                {
                    tileController.Base.UpdateTileInfo();
                    App.Manager.UI.GetPanel<MapPanel>().SetInfoActive(true);
                }
            }
            else if (canPlayerMove)
            {
                if (SelectPlayerMovePoint(tileController))
                {
                    GetUnit<ArrowUnit>().ArrowOn(tileController.transform.position);
                    canPlayerMove = false;
                }
                else
                    return;
            }
            else if (isDronePrepared)
            {
                GetUnit<DroneUnit>().SetTileForDrone(tileController);
            }
        }
    }

    void RightClickEvent()
    {
        AllBorderOff();

        canPlayerMove = false;

        if (isDronePrepared)
        {
            GetUnit<DroneUnit>().Cancel();
        }

        isDronePrepared = false;

        MovePathDelete();
    }
    #endregion

    public void NextDay()
    {
        Buff = defaultBuff;
        AllBorderOff();
        TileEffectCheck();
        UpdateCurrentTile(tileCtrl);
        ReInitMaps();

        ReInitSight(tileCtrl.Model);
        InitValue();
    }

    private void TileEffectCheck()
    {
        var tileBase = App.Manager.Map.tileCtrl.Base;

        tileBase.Buff();
        tileBase.DeBuff();
    }

    public void MovePathDelete()
    {
        GetUnit<ArrowUnit>().ArrowOff();
        AllBorderOff();
    }

    public List<Tile> GetAllTiles() => hexaMapCtrl.Map.Tiles.Where(x => ((GameObject)x.GameEntity).CompareTag("Tile")).ToList();

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
        var candidate = GetTilesInRange(1, tileCtrl.Model);

        Tile tile = candidate[0];

        for (int i = 0; i < candidate.Count; i++)
        {
            if (GetTileController(candidate[i]).Base.canMove)
            {
                TileBase tileBase = GetTileController(candidate[i]).Base;
                if (tileBase.structure == null && !tileBase.isZombie)
                {
                    tile = candidate[i];
                    break;
                }
            }
        }

        tileCtrl = GetTileController(tile);
    }

    public void TilePathFinderSurroundings(TileController tileController)
    {
        var neighborTiles = hexaMapCtrl.Map.GetTilesInRange(tileCtrl.Model, Buff.moveRange);

        var neighborController = neighborTiles
            .Select(x => GetTileController(x)).ToList();

        for (var index = 0; index < neighborController.Count; index++)
        {
            var value = neighborController[index];

            if (!tileController.Base.canMove)
                continue;

            selectedTiles.Add(value);
            SelectBorder(value, ETileState.None);
        }

        if (tileController.Base.structure?.isAccessible == false
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
            && !tileController.Base.canMove)
        {
            SavePlayerMovePath(tileController);
            return true;
        }
        else
            return false;
    }

    public void SavePlayerMovePath(TileController tileController)
    {
        targetTile = tileController;

        AllBorderOff();
    }

    public void SelectBorder(TileController tileController, ETileState state)
    {
        if (state != ETileState.Target) 
            tileController.Base.BorderOn(state);

        selectedTiles.Add(tileController);
    }

    public void AllBorderOff()
    {
        if (selectedTiles == null) return;

        for (int i = 0; i < selectedTiles.Count; i++)
        {
            TileController tile = selectedTiles[i];
            tile.Base.OffNormalBorder();
        }

        selectedTiles.Clear();
    }

    public Tile GetTileFromCoords(Coords coords)
    {
        return hexaMapCtrl.Map.GetTileFromCoords(coords);
    }

    public List<Tile> GetTilesInRange(int num, Tile _tile = null)
    {
        var tile = _tile ?? tileCtrl.Model;

        return hexaMapCtrl.Map.GetTilesInRange(tile, num);
    }

    public bool CalculateDistanceToPlayer(Tile tile, int range)
    {
        var searchTiles = hexaMapCtrl.Map.GetTilesInRange(tile, range);

        return searchTiles.Exists(x => x == tileCtrl.Model);
    }

    public bool CheckTileInSight(TileController tileController)
        => GetTilesInRange(3).Contains(tileController.Model) || tileCtrl == tileController;

    public void UpdateCurrentTile(TileController tileController)
    {
        tileCtrl = tileController;
        Player.PlayerSightUpdate?.Invoke();
    }

    public string GetLandformBGM() => tileCtrl.Base.GetTileType().ToString() switch
    {
        "None" => "Ambience_City",
        "Jungle" => "Ambience_Jungle",
        "Desert" => "Ambience_Desert",
        "Tundra" => "Ambience_Tundra",
        _ => "Ambience_City",
    };

    public void AddMoveRange(int num)
    {
        Buff.moveRange += num;
    }

    public void SetMoveRange(int num)
    {
        Buff.moveRange = num;
    }

    public void SetCloaking(int num)
    {
        Buff.canDetect = false;
        GetUnit<PlayerUnit>().SetCloaking(num);
    }

    public void UnsetCloaking()
    {
        Buff.canDetect = true;
    }

    public void SetResourceCount(int num)
    {
        Buff.resourceCount += num;
    }

    private TileController GetTileController(Tile _tile)
        => ((GameObject)_tile.GameEntity).GetComponent<TileController>();
}