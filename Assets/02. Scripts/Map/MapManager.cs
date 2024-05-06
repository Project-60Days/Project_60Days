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

    public EnemyUnit enemyCtrl;
    public ArrowUnit arrowCtrl;

    public PlayerUnit playerCtrl;
    public TileController tileCtrl;
    public DroneUnit droneCtrl;
    public StructUnit structCtrl;

    public bool mouseIntreractable;

    [SerializeField] ETileMouseState mouseState;
    public MapCamCtrl cameraCtrl;

    Camera mainCamera;
    TileController curTileController;

    bool canPlayerMove;
    bool isDronePrepared;
    bool isDisturbtorPrepared;

    private TileController cameraTarget;

    public MapData data { get; private set; }

    [SerializeField] Transform mapTransform;
    [SerializeField] Transform mapParentTransform;
    [SerializeField] Transform objectsTransform;


    [Header("안개")]
    [Space(5f)]
    public csFogWar fog;

    List<TileController> selectedTiles = new List<TileController>();

    List<TileController> pathTiles = new List<TileController>();

    List<Tile> sightTiles = new List<Tile>();

    public TileController targetTile;
    bool isLoadingComplete;

    public bool LoadingComplete => isLoadingComplete;

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
        DeselectAllBorderTiles();
        InitSight();

        InitMaps();

        cameraCtrl.Init();
        InitValue();
    }

    public void InitSight()
    {
        var allTiles = GetAllTiles();

        foreach (var tile in allTiles)
        {
            ((GameObject)tile.GameEntity).SetActive(false);
        }

        var structs = structCtrl.GetStructObjects();

        foreach (var structure in structs)
        {
            structure.gameObject.SetActive(false);
        }

        ReInitSight(tileCtrl.Model);
    }

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
        SetETileMoveState();

        if (mouseState != ETileMouseState.Nothing)
        {
            MouseOverEvents();
        }
    }

    void MouseOverEvents()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            DeselectAllBorderTiles();
            return;
        }

        RaycastHit hit;
        TileController tileController;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        int onlyLayerMaskTile = 1 << LayerMask.NameToLayer("Tile");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile))
        {
            tileController = hit.transform.parent.GetComponent<TileController>();

            DeselectAllBorderTiles();

            if (!CheckPlayersView(tileController))
            {
                App.Manager.UI.GetPanel<MapPanel>().TileInfo(false);
                return;
            }
            
            switch (mouseState)
            {
                case ETileMouseState.CanClick:
                    DefaultMouseOverState(tileController);

                    if (tileController != curTileController)
                        App.Manager.UI.GetPanel<MapPanel>().TileInfo(false);
                    break;

                case ETileMouseState.CanPlayerMove:
                    TilePathFinderSurroundings(tileController);
                    AddSelectedTilesList(tileController);
                    break;

                case ETileMouseState.DronePrepared:
                    if (isDisturbtorPrepared)
                    {
                        droneCtrl.DisrubtorPathFinder(tileController);
                    }
                    else
                    {
                        ExplorerPathFinder(tileController, 5);
                    }

                    break;
            }

            curTileController = tileController;
        }
        else
        {
            DeselectAllBorderTiles();
            App.Manager.UI.GetPanel<MapPanel>().TileInfo(false);
        }

        MouseClickEvents();
    }

    void MouseClickEvents()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        int onlyLayerMaskPlayer = 1 << LayerMask.NameToLayer("Player");
        int onlyLayerMaskTile = 1 << LayerMask.NameToLayer("Tile");

        if (Input.GetMouseButtonDown(0))
        {
            // 플레이어를 클릭한 경우
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskPlayer))
            {
                if (!isDronePrepared)
                    canPlayerMove = CheckPlayerCanMove();
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile))
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
                        arrowCtrl.ArrowOn(tileController.transform.position);
                        canPlayerMove = false;
                    }
                    else
                        return;
                }
                else if (isDronePrepared)
                {
                    if (isDisturbtorPrepared)
                    {
                        droneCtrl.SelectTileForDisturbtor(tileController);
                    }
                    else
                    {
                        droneCtrl.SelectTileForExplorer(tileController);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            DeselectAllBorderTiles();

            if (canPlayerMove)
            {
                canPlayerMove = false;
            }

            // 목적지 정한 이후 취소 가능

            if (isDronePrepared)
            {
                if (isDisturbtorPrepared)
                {
                    droneCtrl.CancelDisrubtor();
                }
                else
                {
                    droneCtrl.CancelExplorer();
                }
            }

            MovePathDelete();
        }
    }

    void SetETileMoveState()
    {
        if (!mouseIntreractable)
            mouseState = ETileMouseState.Nothing;

        else if (!canPlayerMove && !isDronePrepared)
            mouseState = ETileMouseState.CanClick;

        else if (canPlayerMove)
            mouseState = ETileMouseState.CanPlayerMove;

        else if (isDronePrepared)
            mouseState = ETileMouseState.DronePrepared;
    }

    public void NextDay()
    {
        DeselectAllBorderTiles();

        ReInitMaps();

        ReInitSight(tileCtrl.Model);

        InitValue();
    }

    public bool CheckCanInstallDrone()
        => mouseState == ETileMouseState.CanClick;
    public void InitValue()
    {
        mouseIntreractable = true;
        canPlayerMove = false;
        isDronePrepared = false;
        isDisturbtorPrepared = false;
    }

    public string GetLandformBGM(TileBase _tile) => _tile.tileData.English switch
    {
        "None" => "Ambience_City",
        "Jungle" => "Ambience_Jungle",
        "Desert" => "Ambience_Desert",
        "Tundra" => "Ambience_Tundra",
        _ => "Ambience_City",
    };

    public void NormalStructureResearch(StructBase structure)
    {
        int randomNumber = UnityEngine.Random.Range(1, 4);

        if (randomNumber == 3)
            enemyCtrl.SpawnStructureZombies(structure.colleagues);

        // 경로 삭제
        MovePathDelete();

        SpawnSpecialItemRandomTile(structure.colleagueBases);
    }

    public void MovePathDelete()
    {
        if (playerCtrl.IsMovePathSaved() == false)
            return;

        arrowCtrl.ArrowOff();
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

    public void SetIsDronePrepared(bool _isDronePrepared, string type)
    {
        isDronePrepared = _isDronePrepared;
        
        if(type == "Distrubtor")
            isDisturbtorPrepared = true;
        else
            isDisturbtorPrepared = false;
    }

    public void GenerateMap()
    {
        // Add some noise to Y position of tiles
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

    public void ExplorerPathFinder(TileController tileController, int num = 3)
    {
        int moveRange = 0;
        if (tileController.Model != tileCtrl.Model)
        {
            foreach (Coords coords in AStar.FindPath(tileCtrl.Model.Coords, tileController.Model.Coords))
            {
                if (moveRange == num)
                    break;

                var tile = TileToTileController(GetTileFromCoords(coords));

                if (CheckTileType(tileController.Model, "LandformRocks", "LandformPlain") == false)
                    continue;

                SelectBorder(tile, ETileState.None);
                selectedTiles.Add(tile);
                moveRange++;
            }

            if (moveRange != num && tileController.gameObject.GetComponent<TileBase>().structure?.isAccessible == false)
                SelectBorder(tileController, ETileState.Unable);
            else
                SelectBorder(tileController, ETileState.Moveable);
        }
        else
        {
            SelectBorder(tileController, ETileState.Unable);
        }
    }

    public void TilePathFinderSurroundings(TileController tileController)
    {
        var neighborTiles = hexaMapCtrl.Map.GetTilesInRange(tileCtrl.Model, playerCtrl.PlayerMoveRange);

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

    public bool CheckPlayerCanMove() => playerCtrl.PlayerMoveRange != 0;

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

        playerCtrl.player.UpdateMovePath(AStar.FindPath(tileCtrl.Model.Coords, tileController.Model.Coords));

        DeselectAllBorderTiles();
        //isPlayerSelected = false;
    }

    public void DeletePlayerMovePath()
    {
        playerCtrl.player.UpdateMovePath(null);
        DeselectAllBorderTiles();
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

    void DeselectNormalBorder(TileController tileController)
    {
        tileController.GetComponent<Borders>().OffNormalBorder();

        if (selectedTiles.Contains(tileController))
            selectedTiles.Remove(tileController);
    }

    void ClearTiles(List<TileController> tiles)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            TileController tile = tiles[i];
            DeselectNormalBorder(tile);
        }

        tiles.Clear();
    }

    public void DeselectAllBorderTiles()
    {
        if (selectedTiles == null)
            return;

        ClearTiles(selectedTiles);

        if (pathTiles == null)
            return;

        ClearTiles(pathTiles);
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

    public bool CheckPlayersView(TileController tileController)
    {
        var getTiles = GetTilesInRange(3);

        if (tileCtrl == tileController)
            return true;

        if (getTiles.Contains(tileController.Model))
        {
            return true;
        }
        else
            return false;
    }

    public void SpawnSpecialItemRandomTile(List<TileBase> tileBases)
    {
        int randomInt = UnityEngine.Random.Range(0, tileBases.Count);
        var randomTile = tileBases[randomInt];

        if (randomTile.structure == null)
            Debug.Log("비어있음");

        randomTile.AddSpecialItem();
    }

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

    public void ReInitSight(Tile _targetTile)
    {
        foreach (var tile in sightTiles)
        {
            ((GameObject)tile.GameEntity).SetActive(false);
        }

        sightTiles = GetTilesInRange(5, _targetTile);
        sightTiles.Add(_targetTile);

        var structs = structCtrl.GetStructObjects();

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

    public List<Tile> GetPlayerSightTiles()
    {
        var list = GetTilesInRange(2);
        return list;
    }
}