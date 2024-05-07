using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Hexamap;
using FischlWorks_FogWar;
using System.Collections.Generic;

public class MapManager : Manager
{
    [SerializeField] List<MapBase> Maps;
    private Dictionary<Type, MapBase> MapDic;

    public HexamapController hexaMapCtrl;

    public TileController tileCtrl;

    public MapCamCtrl cameraCtrl;

    Camera mainCamera;
    TileController curTileController;

    bool canPlayerMove;
    bool isDronePrepared;
    public bool canClick => !canPlayerMove && !isDronePrepared;

    private TileController cameraTarget;

    public MapData data { get; private set; }

    [SerializeField] Transform mapTransform;
    [SerializeField] Transform mapParentTransform;
    [SerializeField] Transform objectsTransform;


    [Header("안개")]
    [Space(5f)]
    public csFogWar fog;

    public List<TileController> selectedTiles = new List<TileController>();

    List<TileController> pathTiles = new List<TileController>();

    List<Tile> sightTiles = new List<Tile>();

    public TileController targetTile;
    bool isLoadingComplete;

    public bool LoadingComplete => isLoadingComplete;

    int playerLayer;
    int tileLayer;

    Ray ray;

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
        data = App.Manager.Test.mapData;

        GenerateMap();
        AllBorderOff();
        InitMaps();
        InitSight();
        cameraCtrl.Init();
        InitValue();

        playerLayer = 1 << LayerMask.NameToLayer("Player");
        tileLayer = 1 << LayerMask.NameToLayer("Tile");
    }

    private void GenerateMap()
    {
        FastNoise _fastNoise = new FastNoise();

        _fastNoise.SetFrequency(0.1f);
        _fastNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
        _fastNoise.SetSeed(hexaMapCtrl.Map.Seed);

        foreach (Tile tile in hexaMapCtrl.Map.Tiles)
        {
            var noiseY = _fastNoise.GetValue(tile.Coords.X, tile.Coords.Y);
            (tile.GameEntity as GameObject).transform.position += new Vector3(0, noiseY * 2, 0);
        }

        mapParentTransform.position = Vector3.forward * 200f;

        UpdateCurrentTile(TileToTileController(hexaMapCtrl.Map.GetTileFromCoords(new Coords(0, 0))));
        targetTile = tileCtrl;

        isLoadingComplete = true;
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

        var structs = GetUnit<StructUnit>().GetStructObjects();

        foreach (var structure in structs)
        {
            structure.gameObject.SetActive(false);
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

        var structs = GetUnit<StructUnit>().GetStructObjects();

        foreach (var structure in structs)
        {
            bool check = sightTiles.Contains(structure.currTile);

            structure.gameObject.SetActive(check);
        }

        foreach (var tile in sightTiles)
        {
            ((GameObject)tile.GameEntity).SetActive(true);
        }
    }
    #endregion

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
                App.Manager.UI.GetPanel<MapPanel>().TileInfo(false);
                return;
            }

            if (canClick)
            {
                DefaultMouseOverState(hitTile);

                if (hitTile != curTileController)
                    App.Manager.UI.GetPanel<MapPanel>().TileInfo(false);
            }

            else if (canPlayerMove)
            {
                TilePathFinderSurroundings(hitTile);
                AddSelectedTilesList(hitTile);
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
            App.Manager.UI.GetPanel<MapPanel>().TileInfo(false);
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
                tileController.GetComponent<TileBase>().TileInfoUpdate();
                App.Manager.UI.GetPanel<MapPanel>().TileInfo(true);
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

    public void NextDay()
    {
        AllBorderOff();

        ReInitMaps();

        ReInitSight(tileCtrl.Model);

        InitValue();
    }



    public string GetLandformBGM() => tileCtrl.GetComponent<TileBase>().tileData.Code switch
    {
        "None" => "Ambience_City",
        "Jungle" => "Ambience_Jungle",
        "Desert" => "Ambience_Desert",
        "Tundra" => "Ambience_Tundra",
        _ => "Ambience_City",
    };

    public void MovePathDelete()
    {
        if (GetUnit<PlayerUnit>().IsMovePathSaved() == false)
            return;

        GetUnit<ArrowUnit>().ArrowOff();
        DeletePlayerMovePath();
    }

    /// <summary>
    /// returns the coordinates of the exact center of the camera
    /// </summary>
    public void GetCameraCenterTile()
    {
        Vector3 centerPos = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
        Ray ray = Camera.main.ScreenPointToRay(centerPos);

        int onlyLayerMaskTile = 1 << LayerMask.NameToLayer("Tile");

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, onlyLayerMaskTile))
        {
            var target = hit.transform.parent.GetComponent<TileController>();

            if (target == null) return;

            if (cameraTarget != target)
            {
                cameraTarget = target;
            }
        }
    }

    public List<Tile> GetAllTiles() => hexaMapCtrl.Map.Tiles.Where(x => ((GameObject)x.GameEntity).CompareTag("Tile")).ToList();

    public void DefaultMouseOverState(TileController tileController)
    {
        if (CheckTileType(tileController.Model, "LandformRocks", "LandformPlain") == false)
        {
            SelectBorder(tileController, ETileState.Unable);
        }
        else if (tileController != null && !selectedTiles.Contains(tileController))
        {
            SelectBorder(tileController, ETileState.Unable);
        }
    }

    

    public void TilePathFinderSurroundings(TileController tileController)
    {
        var neighborTiles = hexaMapCtrl.Map.GetTilesInRange(tileCtrl.Model, GetUnit<PlayerUnit>().PlayerMoveRange);

        var neighborController = neighborTiles
            .Select(x => ((GameObject)x.GameEntity).GetComponent<TileController>()).ToList();

        for (var index = 0; index < neighborController.Count; index++)
        {
            var value = neighborController[index];

            if (CheckTileType(tileController.Model, "LandformRocks", "LandformPlain") == false)
                continue;

            selectedTiles.Add(value);
            SelectBorder(value, ETileState.None);
        }

        if (tileController.gameObject.GetComponent<TileBase>().structure?.isAccessible == false
            || CheckTileType(tileController.Model, "LandformRocks", "LandformPlain") == false)
        {
            SelectBorder(tileController, ETileState.Unable);
        }
        else if (neighborTiles.Contains(tileController.Model) == false)
        {
            SelectBorder(tileController, ETileState.Unable);
        }
        else if (tileController.gameObject.GetComponent<TileBase>().currZombies != null)
        {
            SelectBorder(tileController, ETileState.Unable);
        }
        else if (neighborTiles.Contains(tileController.Model))
        {
            SelectBorder(tileController, ETileState.Moveable);
        }
    }

    public void AddSelectedTilesList(TileController tileController)
    {
        selectedTiles.Add(tileController);
    }

    public bool SelectPlayerMovePoint(TileController tileController)
    {
        if (tileController.GetComponent<Borders>().GetEtileState() == ETileState.Moveable
            && tileCtrl.Model != tileController.Model
            && CheckTileType(tileController.Model, "LandformRocks", "LandformPlain"))
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

        GetUnit<PlayerUnit>().player.UpdateMovePath(AStar.FindPath(tileCtrl.Model.Coords, tileController.Model.Coords));

        AllBorderOff();
        //isPlayerSelected = false;
    }

    public void DeletePlayerMovePath()
    {
        GetUnit<PlayerUnit>().player.UpdateMovePath(null);
        AllBorderOff();
    }

    public TileController TileToTileController(Tile tile)
    {
        return ((GameObject)tile.GameEntity).GetComponent<TileController>();
    }

    public void SelectBorder(TileController tileController, ETileState state)
    {
        if (state != ETileState.Target) 
            tileController.GetComponent<Borders>().BorderOn(state);

        selectedTiles.Add(tileController);
    }

    public void AllBorderOff()
    {
        BorderOff(selectedTiles);

        BorderOff(pathTiles);
    }

    void BorderOff(List<TileController> tiles)
    {
        if (tiles == null) return;

        for (int i = 0; i < tiles?.Count; i++)
        {
            TileController tile = tiles?[i];
            tile?.GetComponent<Borders>().OffNormalBorder();
        }

        tiles?.Clear();
    }

    public Tile GetTileFromCoords(Coords coords)
    {
        return hexaMapCtrl.Map.GetTileFromCoords(coords);
    }

    public List<Tile> GetTilesInRange(int num, Tile tile = null)
    {
        if (tile == null)
        {
            return hexaMapCtrl.Map.GetTilesInRange(tileCtrl.Model, num);
        }
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

    public bool CheckTileType(Tile tile, params string[] types)
    {
        var landform = tile.Landform.GetType().Name;
        foreach (var type in types) 
        {
            if (type == landform)
                return true;
        }

        return false;
    }


}