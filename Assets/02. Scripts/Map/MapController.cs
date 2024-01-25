using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Hexamap;
using UnityEngine.EventSystems;
using FischlWorks_FogWar;
using UnityEditor;
using Random = UnityEngine.Random;

public class MapController : Singleton<MapController>
{
    [Header("컴포넌트")] [Space(5f)] [SerializeField]
    HexamapController hexaMap;

    [SerializeField] csFogWar fogOfWar;

    [Header("트랜스폼")] [Space(5f)] [SerializeField]
    Transform zombiesTransform;

    [SerializeField] Transform mapTransform;
    [SerializeField] Transform mapParentTransform;
    [SerializeField] Transform objectsTransform;

    [Header("프리팹")] [Space(5f)] [SerializeField]
    MapPrefabSO mapPrefab;

    List<TileController> selectedTiles = new List<TileController>();
    List<Tile> preemptiveTiles = new List<Tile>();
    List<TileController> pathTiles = new List<TileController>();
    List<GameObject> zombiesList = new List<GameObject>();

    List<Tile> sightTiles = new List<Tile>();

    Player player;

    public Player Player
    {
        get { return player; }
    }

    private MapData mapData;

    GameObject disturbtor;
    GameObject explorer;

    TileController targetTileController;
    bool isLoadingComplete;

    public bool LoadingComplete => isLoadingComplete;

    public TileController TargetPointTile
    {
        get { return targetTileController; }
    }

    private void Start()
    {
        App.instance.GetMapManager().GetAdditiveSceneObjectsCoroutine();
    }

    public IEnumerator GenerateMap()
    {
        hexaMap.Destroy();

        var timeBefore = DateTime.Now;

        hexaMap.Generate();

        double timeSpent = (DateTime.Now - timeBefore).TotalSeconds;

        hexaMap.Draw();

        // Add some noise to Y position of tiles
        FastNoise _fastNoise = new FastNoise();
        _fastNoise.SetFrequency(0.1f);
        _fastNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
        _fastNoise.SetSeed(hexaMap.Map.Seed);

        foreach (Tile tile in hexaMap.Map.Tiles)
        {
            var noiseY = _fastNoise.GetValue(tile.Coords.X, tile.Coords.Y);
            (tile.GameEntity as GameObject).transform.position += new Vector3(0, noiseY * 2, 0);
        }

        mapParentTransform.position = Vector3.forward * 200f;

        yield return StartCoroutine(GenerateMapObjects());

        isLoadingComplete = true;
    }

    public void RegenerateMap()
    {
        Destroy(player);

        foreach (var item in zombiesList)
        {
            Destroy(item.gameObject);
        }

        zombiesList.Clear();
        StartCoroutine(GenerateMap());
    }

    /// <summary>
    /// 맵에서 스폰되는 오브젝트들에 대한 초기화를 하는 함수이다.
    /// 플레이어, 좀비, 안개를 생성하고, 플레이어의 위치를 리소스 매니저에게 전달한다.
    /// </summary>
    IEnumerator GenerateMapObjects()
    {
        App.instance.GetDataManager().gameData.TryGetValue("Data_MinCount_ZombieObject", out GameData min);
        App.instance.GetDataManager().gameData.TryGetValue("Data_MaxCount_ZombieObject", out GameData max);

        SpawnPlayer();
        GenerateTower();
        //Generate3TileStructure(new Coords(-3,-1));
        Generate7TileStructure(new Coords(3, -1));

        SpawnZombies(mapData.zombieCount);

        csFogWar.instance.InitializeMapControllerObjects(player.gameObject, mapData.fogSightRange);
        DeselectAllBorderTiles();

        StartCoroutine(RandomTileResource(mapData.resourcePercent));
        yield return null;
    }

    IEnumerator RandomTileResource(float _percent)
    {
        bool complete = false;

        List<TileBase> tileBaseList = GetAllTiles()
            .Select(x => ((GameObject)x.GameEntity).GetComponent<TileBase>())
            .ToList();

        float randomTileCount = tileBaseList.Count - (tileBaseList.Count * (_percent * 0.01f));

        for (int i = 0; i < randomTileCount; ++i)
        {
            int randNum = Random.Range(0, tileBaseList.Count);
            tileBaseList.RemoveAt(randNum);
        }

        for (int i = 0; i < tileBaseList.Count; i++)
        {
            TileBase tile = tileBaseList[i];
            tile.SpawnRandomResource();

            if (i == tileBaseList.Count - 1)
                complete = true;
        }

        yield return new WaitUntil(() => complete);

        OcclusionCheck(player.TileController.Model);
    }

    public List<Tile> GetAllTiles()
    {
        return hexaMap.Map.Tiles.Where(x => ((GameObject)x.GameEntity).CompareTag("Tile")).ToList();
    }

    void SpawnPlayer()
    {
        Vector3 spawnPos = TileToTileController(hexaMap.Map.GetTileFromCoords(new Coords(0, 0))).transform.position;
        spawnPos.y += 0.7f;

        var playerObject = Instantiate(mapPrefab.items[(int)EMabPrefab.Player].prefab, spawnPos,
            Quaternion.Euler(0, -90, 0));
        player = playerObject.GetComponent<Player>();
        player.transform.parent = mapParentTransform;
        player.InputDefaultData(mapData.playerMovementPoint, mapData.durability);

        player.UpdateCurrentTile(TileToTileController(hexaMap.Map.GetTileFromCoords(new Coords(0, 0))));
        targetTileController = player.TileController;
        StartCoroutine(FloatingAnimation());

        preemptiveTiles.Add(player.TileController.Model);
    }

    IEnumerator FloatingAnimation()
    {
        yield return new WaitUntil(() => player != null);
        player.StartFloatingAnimation();
    }

    void SpawnZombies(int zombiesNumber)
    {
        var tileList = GetAllTiles();
        var selectedTiles = RandomTileSelect(EObjectSpawnType.ExcludePlayer, zombiesNumber);

        // 오브젝트 생성.
        for (int i = 0; i < selectedTiles.Count; i++)
        {
            var tile = tileList[selectedTiles[i]];
            var spawnPos = ((GameObject)tile.GameEntity).transform.position;
            spawnPos.y += 0.6f;

            var zombie = Instantiate(mapPrefab.items[(int)EMabPrefab.Zombie].prefab, spawnPos,
                Quaternion.Euler(0, Random.Range(0, 360), 0), zombiesTransform);
            zombie.name = "Zombie " + (i + 1);
            zombie.GetComponent<ZombieBase>().Init(tile);
            zombie.GetComponent<ZombieBase>().SetValue(mapData.playerMovementPoint, mapData.zombieDetectionRange);
            zombiesList.Add(zombie);
        }
    }

    public void SpawnTutorialZombie()
    {
        var tile = GetTileFromCoords(new Coords(0, -2));

        var spawnPos = ((GameObject)tile.GameEntity).transform.position;
        spawnPos.y += 0.6f;

        var zombie = Instantiate(mapPrefab.items[(int)EMabPrefab.Zombie].prefab, spawnPos,
            Quaternion.Euler(0, Random.Range(0, 360), 0), zombiesTransform);
        zombie.name = "Tutorial Zombie";
        zombie.GetComponent<ZombieBase>().Init(tile);

        zombiesList.Add(zombie);

        // 튜토리얼 이동
        //zombie.GetComponent<ZombieBase>().MoveTargetCoroutine(player.TileController.Model);
    }

    public void SpawnStructureZombies(List<TileBase> tiles)
    {
        var randomInt = Random.Range(0, tiles.Count);
        var tile = tiles[randomInt];

        var spawnPos = tile.transform.position;
        spawnPos.y += 0.6f;

        var zombie = Instantiate(mapPrefab.items[(int)EMabPrefab.Zombie].prefab, spawnPos,
            Quaternion.Euler(0, Random.Range(0, 360), 0), zombiesTransform);

        zombie.name = "Structure Zombie";
        zombie.GetComponent<ZombieBase>().Init(tile.GetComponent<TileController>().Model);
        zombie.GetComponent<ZombieBase>().Stun();

        zombiesList.Add(zombie);
    }

    public void DefalutMouseOverState(TileController tileController)
    {
        if (tileController != null && !selectedTiles.Contains(tileController))
        {
            SelectBorder(tileController, ETileState.Unable);
        }
    }

    public void TilePathFinder(TileController tileController, int num = 0)
    {
        if (num == 0)
            num = player.MoveRange;

        int moveRange = 0;

        if (tileController.Model != player.TileController.Model)
        {
            foreach (Coords coords in AStar.FindPath(player.TileController.Model.Coords, tileController.Model.Coords))
            {
                if (moveRange == player.MoveRange)
                    break;

                GameObject border = (GameObject)GetTileFromCoords(coords).GameEntity;
                border.GetComponent<Borders>().GetNormalBorder()?.SetActive(true);
                pathTiles.Add(TileToTileController(GetTileFromCoords(coords)));
                moveRange++;
            }
        }

        if (moveRange == num || tileController.gameObject.GetComponent<TileBase>().Structure?.IsAccessible == false)
            tileController.GetComponent<Borders>().GetUnAvailableBorder()?.SetActive(true);
        else if (moveRange != num)
            tileController.GetComponent<Borders>().GetAvailableBorder()?.SetActive(true);
    }

    public void TilePathFinderSurroundings(TileController tileController)
    {
        var neighborTiles = hexaMap.Map.GetTilesInRange(player.TileController.Model, player.MoveRange);

        var neighborController = neighborTiles
            .Select(x => ((GameObject)x.GameEntity).GetComponent<TileController>()).ToList();

        for (var index = 0; index < neighborController.Count; index++)
        {
            var value = neighborController[index];
            selectedTiles.Add(value);
            SelectBorder(value, ETileState.None);
        }

        if (tileController.gameObject.GetComponent<TileBase>().Structure?.IsAccessible == false)
        {
            SelectBorder(tileController, ETileState.Unable);
        }
        else if (neighborTiles.Contains(tileController.Model) == false)
        {
            SelectBorder(tileController, ETileState.Unable);
        }
        else if (tileController.gameObject.GetComponent<TileBase>().CurZombies != null)
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

    public void DisturbtorPathFinder(TileController tileController)
    {
        if (hexaMap.Map.GetTilesInRange(player.TileController.Model, 1).Contains(tileController.Model))
        {
            disturbtor.transform.position =
                ((GameObject)tileController.Model.GameEntity).transform.position + Vector3.up;
            disturbtor.GetComponent<Distrubtor>().DirectionObjectOff();
            SelectBorder(tileController, ETileState.Moveable);

            foreach (var item in player.TileController.Model.Neighbours.Where(
                         item => item.Value == tileController.Model))
            {
                disturbtor.GetComponent<Distrubtor>().GetDirectionObject(item.Key).SetActive(true);
            }
        }
    }

    public bool PlayerCanMoveCheck()
    {
        if (player.MoveRange != 0)
            return true;
        else
            return false;
    }

    public bool SelectPlayerMovePoint(TileController tileController)
    {
        if (tileController.GetComponent<Borders>().GetEtileState() == ETileState.Moveable
            && player.TileController.Model != tileController.Model)
        {
            SavePlayerMovePath(tileController);
            return true;
        }
        else
            return false;
    }

    public void SelectTileForDisturbtor(TileController tileController)
    {
        if (!GetTileBorder(tileController, ETileState.Moveable).activeInHierarchy)
            return;

        foreach (var item in player.TileController.Model.Neighbours.Where(item => item.Value == tileController.Model))
        {
            InstallDisturbtor(tileController, item.Key);
        }
    }

    public void SelectTileForExplorer(TileController tileController)
    {
        if (!GetTileBorder(tileController, ETileState.Moveable).activeInHierarchy)
            return;

        InstallExplorer(tileController);
    }

    public void SavePlayerMovePath(TileController tileController)
    {
        targetTileController = tileController;

        player.UpdateMovePath(AStar.FindPath(player.TileController.Model.Coords, tileController.Model.Coords));

        DeselectAllBorderTiles();
        //isPlayerSelected = false;
    }

    public void DeletePlayerMovePath()
    {
        player.UpdateMovePath(null);
        DeselectAllBorderTiles();
    }

    public bool IsMovePathSaved()
    {
        if (player.MovePath != null)
            return true;
        else
            return false;
    }

    TileController TileToTileController(Tile tile)
    {
        return ((GameObject)tile.GameEntity).GetComponent<TileController>();
    }

    public void PreparingDisturbtor(bool set)
    {
        List<Tile> nearthTiles = GetTilesInRange(player.TileController.Model, 1);

        if (set)
        {
            for (int i = 0; i < nearthTiles.Count; i++)
            {
                SelectBorder(TileToTileController(nearthTiles[i]), ETileState.Target);
            }

            GenerateExampleDisturbtor();

            //isDronePrepared = true;
        }
        else
        {
            for (int i = 0; i < nearthTiles.Count; i++)
            {
                DeselectBorder(TileToTileController(nearthTiles[i]));
            }

            Destroy(disturbtor);

            //isDronePrepared = false;
        }
    }

    void GenerateExampleDisturbtor()
    {
        disturbtor = Instantiate(mapPrefab.items[(int)EMabPrefab.Disturbtor].prefab,
            player.transform.position + Vector3.up * 1.5f, Quaternion.Euler(0, -90, 0));
        disturbtor.transform.parent = mapTransform;
        disturbtor.GetComponentInChildren<MeshRenderer>().material.DOFade(50, 0);
    }

    void InstallDisturbtor(TileController tileController, CompassPoint direction)
    {
        if (hexaMap.Map.GetTilesInRange(player.TileController.Model, 1).Contains(tileController.Model))
            return;
        disturbtor.GetComponent<Distrubtor>().Set(tileController.Model, direction);
        disturbtor.GetComponent<Distrubtor>().DirectionObjectOff();

        List<Tile> nearthTiles = GetTilesInRange(player.TileController.Model, 1);
        for (int i = 0; i < nearthTiles.Count; i++)
        {
            DeselectBorder(TileToTileController(nearthTiles[i]));
        }

        //isDisturbtor = true;
    }

    public void PreparingExplorer(bool set)
    {
        if (set)
        {
            GenerateExampleExplorer();
            //isDronePrepared = true;
            //isDisturbtor = false;
        }
        else
        {
            Destroy(explorer);
            //isDronePrepared = false;
        }
    }

    void GenerateExampleExplorer()
    {
        explorer = Instantiate(mapPrefab.items[(int)EMabPrefab.Explorer].prefab,
            player.transform.position + Vector3.up * 1.5f, Quaternion.identity);
        explorer.transform.parent = mapTransform;

        explorer.GetComponentInChildren<MeshRenderer>().material.DOFade(50, 0);
        explorer.GetComponent<Explorer>().Set(player.TileController.Model);
    }

    void InstallExplorer(TileController tileController)
    {
        if (player.TileController.Model != tileController.Model)
            return;

        explorer.GetComponent<Explorer>().Targetting(tileController.Model);
        explorer.GetComponent<Explorer>().Move();

        //isDisturbtor = false;
    }

    public IEnumerator NextDay()
    {
        
        bool zombieActEnd = false;

        // 플레이어 이동
        if (player.MovePath != null)
        {
            yield return StartCoroutine(player.MoveToTarget(targetTileController));
        }
        else
        {
            DeselectAllBorderTiles();
        }

        // 교란기
        if (disturbtor != null)
            disturbtor.GetComponent<Distrubtor>().Move();

        // 탐사기
        if (explorer != null)
            StartCoroutine(explorer.GetComponent<Explorer>().Move());

        // 좀비 행동
        for (var index = 0; index < zombiesList.Count; index++)
        {
            var zombie = zombiesList[index];

            zombie.GetComponent<ZombieBase>().DetectionAndAct();

            if (index == zombiesList.Count - 1)
                zombieActEnd = true;
        }

        yield return new WaitUntil(() => zombieActEnd);
        yield return new WaitForSeconds(1f);
        CheckSumZombies();

        // 이동 거리 충전
        player.SetHealth(true);
        player.TileEffectCheck();
        OcclusionCheck(player.TileController.Model);
    }

    public void CheckSumZombies()
    {
        List<ZombieBase> zombieBases = zombiesList.Select(x => x.GetComponent<ZombieBase>()).ToList();
        List<ZombieBase> removeZombies = new List<ZombieBase>();

        for (int i = 0; i < zombieBases.Count; i++)
        {
            for (int j = i + 1; j < zombieBases.Count; j++)
            {
                var firstZombies = zombieBases[i];
                var secondZombies = zombieBases[j];

                if (firstZombies.zombieData.count == 0 || secondZombies.zombieData.count == 0)
                    continue;

                if (firstZombies.curTile == secondZombies.curTile)
                {
                    firstZombies.SumZombies(secondZombies);
                    removeZombies.Add(secondZombies);
                }
            }
        }

        for (int i = 0; i < removeZombies.Count(); i++)
        {
            var item = removeZombies[i];
            zombiesList.Remove(item.gameObject);
            Destroy(item.gameObject);
        }
    }

    void SelectBorder(TileController tileController, ETileState state)
    {
        switch (state)
        {
            case ETileState.None:
                tileController.GetComponent<Borders>().GetNormalBorder().SetActive(true);
                break;
            case ETileState.Moveable:
                tileController.GetComponent<Borders>().GetAvailableBorder().SetActive(true);
                break;
            case ETileState.Unable:
                tileController.GetComponent<Borders>().GetUnAvailableBorder().SetActive(true);
                break;
            case ETileState.Target:
                tileController.GetComponent<Borders>().GetDisturbanceBorder().SetActive(true);
                break;
        }

        selectedTiles.Add(tileController);
    }

    void DeselectBorder(TileController tileController)
    {
        tileController.GetComponent<Borders>().OffAllBorders();

        if (selectedTiles.Contains(tileController))
            selectedTiles.Remove(tileController);
    }

    public bool CheckPlayerInStructureTile(TileController tileController)
    {
        var structure = tileController.gameObject.GetComponent<TileBase>().Structure;

        if (structure != null)
        {
            if (tileController.gameObject.GetComponent<TileBase>().Structure.IsAccessible)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
            return false;
    }

    void ClearTiles(List<TileController> tiles)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            TileController tile = tiles[i];
            DeselectBorder(tile);
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

    GameObject GetTileBorder(TileController tileController, ETileState state)
    {
        switch (state)
        {
            case ETileState.None:
                return tileController.GetComponent<Borders>().GetNormalBorder();
            case ETileState.Moveable:
                return tileController.GetComponent<Borders>().GetAvailableBorder();
            case ETileState.Unable:
                return tileController.GetComponent<Borders>().GetUnAvailableBorder();
        }

        return null;
    }

    /// <summary>
    /// 같은 그룹 타일이 모여있는 곳에 마우스를 올리면 그룹 전체를 선택을 하게 해주는 함수. 현재 사용 중이지 않다.
    /// </summary>
    /// <param name="tile"></param>
    void SelectMetaLandform(TileController tile)
    {
        // Select metalandform of a tile
        var metaLandformTiles = tile
            .Model
            .Landform
            .MetaLandform
            .Tiles
            .Select(t => t.GameEntity)
            .Cast<GameObject>()
            .Select(g => g.GetComponent<TileController>())
            .ToList();

        var tileToUnselect = selectedTiles.Except(metaLandformTiles).ToList();
        var tileToSelect = metaLandformTiles.Except(selectedTiles).ToList();

        tileToSelect.ForEach(t => SelectBorder(t, ETileState.Unable));
        tileToUnselect.ForEach(t => DeselectBorder(t));
    }

    public Tile GetTileFromCoords(Coords coords)
    {
        return hexaMap.Map.GetTileFromCoords(coords);
    }

    public List<Tile> GetTilesInRange(Tile tile, int num)
    {
        return hexaMap.Map.GetTilesInRange(tile, num);
    }

    public Tile GetPlayerLocationTile()
    {
        return player.TileController.Model;
    }

    public bool isDisturbtorInstall()
    {
        if (disturbtor != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CalculateDistanceToPlayer(Tile tile, int range)
    {
        var searchTiles = hexaMap.Map.GetTilesInRange(tile, range);

        return searchTiles.Exists(x => x == player.TileController.Model);
    }

    public Distrubtor CalculateDistanceToDistrubtor(Tile tile, int range)
    {
        var searchTiles = hexaMap.Map.GetTilesInRange(tile, range);

        if (disturbtor == null)
            return null;

        foreach (var item in searchTiles)
        {
            if (disturbtor.GetComponent<Distrubtor>().currentTile == item)
            {
                return disturbtor.GetComponent<Distrubtor>();
            }
        }

        return null;
    }

    // 시야 바꾸기
    public bool CheckPlayersView(TileController tileController)
    {
        var getTiles = GetTilesInRange(player.TileController.Model, 2);

        if (player.TileController == tileController)
            return true;

        if (getTiles.Contains(tileController.Model))
        {
            return true;
        }
        else
            return false;
    }

    public bool CheckZombies()
    {
        var playerNearthTiles = GetTilesInRange(player.TileController.Model, 2);

        for (int i = 0; i < zombiesList.Count; i++)
        {
            GameObject item = zombiesList[i];
            if (playerNearthTiles.Contains(item.GetComponent<ZombieBase>().curTile))
            {
                return true;
            }
        }

        return false;
    }

    public void GenerateTower()
    {
        // 경계선으로부터 2칸 이내 범위 
        // List<int> selectedTiles = RandomTileSelect(ObjectSpawnDistanceCalculate(2),
        //     EObjectSpawnType.ExcludeEntites, 1);

        var tilelist = new List<Tile>();

        // 튜토리얼 용 위치 고정
        Tile tile = GetTileFromCoords(new Coords(0, 2));

        tilelist.Add(tile);

        List<Tile> neighborList = SetNeighborStructure(tilelist);

        var spawnPos = ((GameObject)tile.GameEntity).transform.position;
        spawnPos.y += 0.31f;

        var tower = Instantiate(mapPrefab.items[(int)EMabPrefab.Tower].prefab, spawnPos, Quaternion.Euler(0, 90, 0),
            objectsTransform);

        tower.GetComponent<StructureObject>().Init(tile);

        ((GameObject)tile.GameEntity).GetComponent<TileBase>().SpawnQuestStructure(neighborList, tower);
    }

    public void Generate7TileStructure(Coords _coords)
    {
        // 경계선으로부터 2칸 이내 범위 
        // List<int> selectedTiles = RandomTileSelect(ObjectSpawnDistanceCalculate(2),
        //     EObjectSpawnType.ExcludeEntites, 1);

        var tilelist = new List<Tile>();

        GameObject structureObject = mapPrefab.items[(int)EMabPrefab.Production].prefab;

        // 튜토리얼 용 위치 고정
        Tile tile = GetTileFromCoords(_coords);

        tilelist.Add(tile);

        tilelist.Add(tile.Neighbours[CompassPoint.N]);
        tilelist.Add(tile.Neighbours[CompassPoint.S]);
        tilelist.Add(tile.Neighbours[CompassPoint.NE]);
        tilelist.Add(tile.Neighbours[CompassPoint.SE]);
        tilelist.Add(tile.Neighbours[CompassPoint.NW]);
        tilelist.Add(tile.Neighbours[CompassPoint.SW]);
        
        List<Tile> neighborList = SetNeighborStructure(tilelist);

        var spawnPos = ((GameObject)tile.GameEntity).transform.position;
        spawnPos.y += 0.2f;

        var structure = Instantiate(structureObject, spawnPos, Quaternion.Euler(0, 180, 0),
            objectsTransform);

        structure.GetComponent<StructureObject>().Init(tile);

        structure.name = "Production";

        for (var index = 0; index < tilelist.Count; index++)
        {
            var tileBase = ((GameObject)tilelist[index].GameEntity).GetComponent<TileBase>();
            tileBase.SpawnNormalStructure(neighborList, tilelist, structure, "생산 라인");

            var position = tileBase.transform.position;
            position.y = ((GameObject)tile.GameEntity).transform.position.y;
            tileBase.transform.position = position;
        }
    }

    public void Generate3TileStructure(Coords _coords)
    {
        // 경계선으로부터 2칸 이내 범위 
        // List<int> selectedTiles = RandomTileSelect(ObjectSpawnDistanceCalculate(2),
        //     EObjectSpawnType.ExcludeEntites, 1);

        var tilelist = new List<Tile>();

        GameObject structureObject = mapPrefab.items[(int)EMabPrefab.Army].prefab;

        // 튜토리얼 용 위치 고정
        Tile tile = GetTileFromCoords(_coords);

        tilelist.Add(tile);
        
        tilelist.Add(tile.Neighbours[CompassPoint.NE]);
        tilelist.Add(tile.Neighbours[CompassPoint.SE]);
        
        List<Tile> neighborList = SetNeighborStructure(tilelist);

        var spawnPos = ((GameObject)tile.GameEntity).transform.position;
        spawnPos.y += 0.2f;

        var structure = Instantiate(structureObject, spawnPos, Quaternion.Euler(0, 120, 0),
            objectsTransform);

        structure.GetComponent<StructureObject>().Init(tile);

        structure.name = "Army";

        for (var index = 0; index < tilelist.Count; index++)
        {
            var tileBase = ((GameObject)tilelist[index].GameEntity).GetComponent<TileBase>();
            tileBase.SpawnNormalStructure(neighborList, tilelist, structure, "군사 기지");

            var position = tileBase.transform.position;
            position.y = ((GameObject)tile.GameEntity).transform.position.y;
            tileBase.transform.position = position;
        }
    }

    public void SpawnSpecialItemRandomTile(List<TileBase> tileBases)
    {
        int randomInt = Random.Range(0, tileBases.Count);
        var randomTile = tileBases[randomInt];

        if (randomTile.Structure == null)
            Debug.Log("비어있음");

        randomTile.AddSpecialItem();
    }

    List<Tile> ObjectSpawnDistanceCalculate(int range)
    {
        var tileList = GetAllTiles();

        Tile lastIndex = tileList.Last();

        int biggerInt = Math.Abs(lastIndex.Coords.X) > Math.Abs(lastIndex.Coords.Y)
            ? Math.Abs(lastIndex.Coords.X)
            : Math.Abs(lastIndex.Coords.Y);

        List<Tile> excludeTileList = GetTilesInRange(GetTileFromCoords(new Coords(0, 0)), biggerInt - range);

        return excludeTileList;
    }

    List<Tile> SetNeighborStructure(List<Tile> tiles)
    {
        List<Tile> neighborTiles = new List<Tile>();

        for (int i = 0; i < tiles.Count; i++)
        {
            var tile = tiles[i];
            var list = tile.Neighbours.Select(kvp => kvp.Value).ToList();
            neighborTiles.AddRange(list);
        }

        neighborTiles = neighborTiles.Distinct().ToList();

        for (var index = 0; index < neighborTiles.Count; index++)
        {
            var tile = neighborTiles[index];
            ((GameObject)tile.GameEntity).GetComponent<TileBase>().SetNeighborStructure();
        }

        return neighborTiles;
    }

    public StructureBase SensingStructure()
    {
        var tileList = player.TileController.Model.Neighbours;

        foreach (var item in tileList)
        {
            var tileBase = ((GameObject)item.Value.GameEntity).GetComponent<TileBase>();

            if (tileBase.Structure != null)
                return tileBase.Structure;
        }

        return null;
    }

    public bool SensingSignalTower()
    {
        var structure = SensingStructure();

        if (structure is Tower)
            return true;
        else
            return false;
    }

    public List<int> RandomTileSelect(EObjectSpawnType type, int choiceNum = 1)
    {
        var tiles = GetAllTiles();

        List<int> selectTileNumber = new List<int>();

        int randomInt = 0;
        // 플레이어와 겹치지 않는 랜덤 타일 뽑기.
        while (selectTileNumber.Count != choiceNum)
        {
            randomInt = UnityEngine.Random.Range(0, tiles.Count);

            if (ConditionalBranch(type, tiles[randomInt]) == true)
            {
                if (selectTileNumber.Contains(randomInt) == false)
                {
                    selectTileNumber.Add(randomInt);
                    preemptiveTiles.Add(tiles[randomInt]);
                }
            }
        }

        return selectTileNumber;
    }

    public List<int> RandomTileSelect(List<Tile> tiles, EObjectSpawnType type, int choiceNum = 1)
    {
        List<int> selectTileNumber = new List<int>();
        int randomInt;

        if (tiles == null || tiles.Count == 0)
            Debug.Log("비어있음");

        // 플레이어와 겹치지 않는 랜덤 타일 뽑기.
        while (selectTileNumber.Count != choiceNum)
        {
            randomInt = Random.Range(0, tiles.Count);

            if (ConditionalBranch(type, tiles[randomInt]))
            {
                if (selectTileNumber.Contains(randomInt) == false)
                {
                    selectTileNumber.Add(randomInt);
                    preemptiveTiles.Add(tiles[randomInt]);
                }
            }
        }

        return selectTileNumber;
    }

    bool ConditionalBranch(EObjectSpawnType type, Tile tile)
    {
        if (CheckTileType(tile, "LandformPlain") == false)
        {
            return false;
        }

        switch (type)
        {
            case EObjectSpawnType.ExcludePlayer:
                if (player.TileController.Model != tile)
                    return true;
                else
                    return false;

            case EObjectSpawnType.IncludePlayer:
                return true;

            case EObjectSpawnType.ExcludeEntites:
                if (!preemptiveTiles.Contains(tile))
                    return true;
                else
                    return false;

            case EObjectSpawnType.IncludeEntites:
                if (player.TileController.Model != tile)
                    return true;
                else
                    return false;

            default:
                break;
        }

        return false;
    }

    bool CheckTileType(Tile tile, string type)
    {
        if (tile.Landform.GetType().Name == type)
            return true;
        else
            return false;
    }

    public void OcclusionCheck(Tile _targetTile)
    {
        sightTiles = GetTilesInRange(_targetTile, 5);
        sightTiles.Add(_targetTile);

        List<StructureObject> structureObjects = objectsTransform.GetComponentsInChildren<StructureObject>().ToList();

        for (int i = 0; i < structureObjects.Count; i++)
        {
            StructureObject item = structureObjects[i];
            if (sightTiles.Contains(item.CurTile) == false)
                item.gameObject.SetActive(false);
            else
                item.gameObject.SetActive(true);
        }

        var allTiles = GetAllTiles();

        for (int i = 0; i < allTiles.Count; i++)
        {
            Tile item = allTiles[i];

            if (sightTiles.Contains(item) == false)
                ((GameObject)item.GameEntity).SetActive(false);
            else
                ((GameObject)item.GameEntity).SetActive(true);
        }
    }

    public void SightCheckInit()
    {
        OcclusionCheck(GetTileFromCoords(new Coords(0, 0)));
    }

    public List<Tile> GetSightTiles()
    {
        var list = GetTilesInRange(player.TileController.Model, 2);
        return list;
    }

    public void InputMapData(MapData _mapData)
    {
        mapData = _mapData;
    }
}