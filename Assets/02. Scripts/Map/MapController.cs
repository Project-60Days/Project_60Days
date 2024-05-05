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
using Random = UnityEngine.Random;

public class MapController : MonoBehaviour
{
    [Header("컴포넌트")] [Space(5f)] [SerializeField]
    HexamapController hexaMap;

    [SerializeField] Transform mapTransform;
    [SerializeField] Transform mapParentTransform;
    [SerializeField] Transform objectsTransform;

    public PlayerCtrl playerCtrl;
    [SerializeField] EnemyCtrl enemyCtrl;
    public TileController tileCtrl;
    public DroneCtrl droneCtrl;


    [Header("프리팹")] [Space(5f)] [SerializeField]
    MapPrefabSO mapPrefab;

    [Header("안개")] [Space(5f)]
    public csFogWar fog;

    List<TileController> selectedTiles = new List<TileController>();

    List<Tile> preemptiveTiles = new List<Tile>();
    List<TileController> pathTiles = new List<TileController>();

    List<Tile> sightTiles = new List<Tile>();

    TileController targetTileController;
    bool isLoadingComplete;

    public bool LoadingComplete => isLoadingComplete;

    MapData data;

    public TileController TargetPointTile
    {
        get { return targetTileController; }
    }

    public void Init()
    {
        data = App.Manager.Map.data;

        StartCoroutine(GenerateMap());
        SightCheckInit();
    }

    public IEnumerator GenerateMap()
    {
        hexaMap.Destroy();

        hexaMap.Generate();

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



    /// <summary>
    /// 맵에서 스폰되는 오브젝트들에 대한 초기화를 하는 함수이다.
    /// 플레이어, 좀비, 안개를 생성하고, 플레이어의 위치를 리소스 매니저에게 전달한다.
    /// </summary>
    IEnumerator GenerateMapObjects()
    {
        playerCtrl.SpawnPlayer();
        targetTileController = tileCtrl;
        preemptiveTiles.Add(tileCtrl.Model);

        foreach (var item in GetTilesInRange(4))
        {
            preemptiveTiles.Add(item);
        }

        GenerateTower();
        Generate3TileStructure(new Coords(0, 0));
        Generate7TileStructure(new Coords(0, 0));

        var tileList = GetAllTiles();
        var selectedTiles = RandomTileSelect(EObjectSpawnType.ExcludePlayer, data.zombieCount);
        enemyCtrl.SpawnZombies(tileList, selectedTiles);

        playerCtrl.SpawnFog();
        DeselectAllBorderTiles();

        StartCoroutine(RandomTileResource(data.resourcePercent));
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

        OcclusionCheck(tileCtrl.Model);
    }

    public List<Tile> GetAllTiles() => hexaMap.Map.Tiles.Where(x => ((GameObject)x.GameEntity).CompareTag("Tile")).ToList();

    public void DefaultMouseOverState(TileController tileController)
    {
        if (LandformCheck(tileController) == false)
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

                if (LandformCheck(tile) == false)
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
        var neighborTiles = hexaMap.Map.GetTilesInRange(tileCtrl.Model, playerCtrl.PlayerMoveRange);

        var neighborController = neighborTiles
            .Select(x => ((GameObject)x.GameEntity).GetComponent<TileController>()).ToList();

        for (var index = 0; index < neighborController.Count; index++)
        {
            var value = neighborController[index];

            if (LandformCheck(value) == false)
                continue;

            selectedTiles.Add(value);
            SelectBorder(value, ETileState.None);
        }

        if (tileController.gameObject.GetComponent<TileBase>().structure?.isAccessible == false
            || LandformCheck(tileController) == false)
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
            && LandformCheck(tileController))
        {
            SavePlayerMovePath(tileController);
            return true;
        }
        else
            return false;
    }

    public void SavePlayerMovePath(TileController tileController)
    {
        targetTileController = tileController;

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

    public IEnumerator NextDay()
    {
        playerCtrl.player.ChangeClockBuffDuration();
        // 플레이어 이동
        if (playerCtrl.IsMovePathSaved())
        {
            yield return StartCoroutine(playerCtrl.player.ActionDecision(targetTileController));
        }
        else
        {
            DeselectAllBorderTiles();
        }

        droneCtrl.ReInit();

        // 좀비 행동
        enemyCtrl.MoveEnemy();

        yield return new WaitForSeconds(1f);
        enemyCtrl.CheckSumZombies();

        // 이동 거리 충전
        playerCtrl.player.SetHealth(true);
        playerCtrl.player.TileEffectCheck();

        OcclusionCheck(tileCtrl.Model);
    }

    public void SelectBorder(TileController tileController, ETileState state)
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

                break;
        }

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
            case ETileState.Target:
                return tileController.GetComponent<Borders>().GetDisturbanceBorder();
        }

        return null;
    }

    public Tile GetTileFromCoords(Coords coords)
    {
        return hexaMap.Map.GetTileFromCoords(coords);
    }

    public List<Tile> GetTilesInRange(int num, Tile tile = null)
    {
        if (tile == null) 
        {
            return hexaMap.Map.GetTilesInRange(tileCtrl.Model, num);
        }
        return hexaMap.Map.GetTilesInRange(tile, num);
    }

    public bool CalculateDistanceToPlayer(Tile tile, int range)
    {
        var searchTiles = hexaMap.Map.GetTilesInRange(tile, range);

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

    public void GenerateTower()
    {
        // 경계선으로부터 2칸 이내 범위 
        // List<int> selectedTiles = RandomTileSelect(ObjectSpawnDistanceCalculate(2),
        //     EObjectSpawnType.ExcludeEntites, 1);

        var tilelist = new List<Tile>();

        // 튜토리얼 용 위치 고정
        Tile tile = GetTileFromCoords(new Coords(1, 3));

        tilelist.Add(tile);

        List<Tile> neighborList = SetNeighborStructure(tilelist);

        var spawnPos = ((GameObject)tile.GameEntity).transform.position;
        spawnPos.y += 0.31f;

        var tower = Instantiate(mapPrefab.prefabs[(int)EMabPrefab.Tower], spawnPos, Quaternion.Euler(0, 90, 0),
            objectsTransform);

        tower.GetComponent<StructureObject>().Init(tile);

        ((GameObject)tile.GameEntity).GetComponent<TileBase>().SpawnQuestStructure(neighborList, tower);

        preemptiveTiles.Add(tile);
    }

    public void Generate7TileStructure(Coords _coords)
    {
        //경계선으로부터 5칸 이내 범위 
        var boundaryTiles = ObjectSpawnDistanceCalculate(8);
        List<int> selectNumber = RandomTileSelect(boundaryTiles, EObjectSpawnType.ExcludeEntites, 1);

        var tilelist = new List<Tile>();

        GameObject structureObject = mapPrefab.prefabs[(int)EMabPrefab.Production];

        // 튜토리얼 용 위치 고정
        //Tile tile = GetTileFromCoords(_coords);

        Tile tile = boundaryTiles[selectNumber[0]];

        tilelist.Add(tile);
        tilelist.Add(tile.Neighbours[CompassPoint.N]);
        tilelist.Add(tile.Neighbours[CompassPoint.S]);
        tilelist.Add(tile.Neighbours[CompassPoint.NE]);
        tilelist.Add(tile.Neighbours[CompassPoint.SE]);
        tilelist.Add(tile.Neighbours[CompassPoint.NW]);
        tilelist.Add(tile.Neighbours[CompassPoint.SW]);

        foreach (var item in tilelist)
        {
            preemptiveTiles.Add(item);
        }

        List<Tile> neighborList = SetNeighborStructure(tilelist);

        var spawnPos = ((GameObject)tile.GameEntity).transform.position;
        spawnPos.y += 0.2f;

        var structure = Instantiate(structureObject, spawnPos, Quaternion.Euler(0, 180, 0),
            objectsTransform);

        structure.GetComponent<StructureObject>().Init(tile);

        structure.name = "Production";

        StructureInfo structureInfo = new StructureInfo(neighborList, tilelist, structure, EStructure.Production);

        for (var index = 0; index < tilelist.Count; index++)
        {
            var tileBase = ((GameObject)tilelist[index].GameEntity).GetComponent<TileBase>();
            tileBase.SpawnNormalStructure(structureInfo);

            var position = tileBase.transform.position;
            position.y = ((GameObject)tile.GameEntity).transform.position.y;
            tileBase.transform.position = position;
        }
    }

    public void Generate3TileStructure(Coords _coords)
    {
        //경계선으로부터 5칸 이내 범위 
        var boundaryTiles = ObjectSpawnDistanceCalculate(7);
        List<int> selectNumber = RandomTileSelect(boundaryTiles, EObjectSpawnType.ExcludeEntites, 1);

        var tilelist = new List<Tile>();

        GameObject structureObject = mapPrefab.prefabs[(int)EMabPrefab.Army];

        Tile tile = boundaryTiles[selectNumber[0]];

        // 튜토리얼 용 위치 고정
        //Tile tile = GetTileFromCoords(_coords);

        tilelist.Add(tile);
        tilelist.Add(tile.Neighbours[CompassPoint.NW]);
        tilelist.Add(tile.Neighbours[CompassPoint.SW]);

        foreach (var item in tilelist)
        {
            preemptiveTiles.Add(item);
        }

        List<Tile> neighborList = SetNeighborStructure(tilelist);

        var spawnPos = ((GameObject)tile.GameEntity).transform.position;
        spawnPos.y += 0.5f;

        var structure = Instantiate(structureObject, spawnPos, Quaternion.Euler(0, 90, 0),
            objectsTransform);

        structure.GetComponent<StructureObject>().Init(tile);

        structure.name = "Army";
        StructureInfo structureInfo = new StructureInfo(neighborList, tilelist, structure, EStructure.Army);

        for (var index = 0; index < tilelist.Count; index++)
        {
            var tileBase = ((GameObject)tilelist[index].GameEntity).GetComponent<TileBase>();
            tileBase.SpawnNormalStructure(structureInfo);

            var position = tileBase.transform.position;
            position.y = ((GameObject)tile.GameEntity).transform.position.y;
            tileBase.transform.position = position;
        }
    }

    public void SpawnSpecialItemRandomTile(List<TileBase> tileBases)
    {
        int randomInt = Random.Range(0, tileBases.Count);
        var randomTile = tileBases[randomInt];

        if (randomTile.structure == null)
            Debug.Log("비어있음");

        randomTile.AddSpecialItem();
    }

    List<Tile> ObjectSpawnDistanceCalculate(int range)
    {
        var tileList = GetAllTiles();

        int biggerInt = 0;
        int maxInt = 0;

        for (int i = 0; i < tileList.Count; i++)
        {
            Tile lastIndex = tileList[i];

            biggerInt = Math.Abs(lastIndex.Coords.X) > Math.Abs(lastIndex.Coords.Y)
                ? Math.Abs(lastIndex.Coords.X)
                : Math.Abs(lastIndex.Coords.Y);

            if (biggerInt > maxInt)
                maxInt = biggerInt;
        }

        List<Tile> excludeTileList = GetTilesInRange(maxInt - range, GetTileFromCoords(new Coords(0, 0)));
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
        }

        return neighborTiles;
    }

    public StructureBase SensingStructure()
    {
        var tileList = tileCtrl.Model.Neighbours;

        foreach (var item in tileList)
        {
            if (LandformCheck(TileToTileController(item.Value)) == false)
                continue;

            var tileBase = ((GameObject)item.Value.GameEntity).GetComponent<TileBase>();

            if (tileBase.structure != null)
                return tileBase.structure;
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

    public bool SensingProductionStructure()
    {
        var structure = SensingStructure();

        if (structure is ProductionStructure)
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

    public List<int> RandomTileSelect(List<Tile> tiles, EObjectSpawnType type, int choiceNum = 1)
    {
        List<int> selectTileNumber = new List<int>();

        if (tiles == null || tiles.Count == 0)
        {
            selectTileNumber.Add(5);
            return selectTileNumber;
        }

        // 플레이어와 겹치지 않는 랜덤 타일 뽑기.
        while (selectTileNumber.Count != choiceNum)
        {
            int randomInt = Random.Range(0, tiles.Count);

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

    public void UpdateCurrentTile(TileController tileController)
    {
        tileCtrl = tileController;
        Player.PlayerSightUpdate?.Invoke();
    }

    bool ConditionalBranch(EObjectSpawnType type, Tile tile)
    {
        // landform rocks도 거르면 건물 잔해도 거를 수 있음
        if (LandformCheck(TileToTileController(tile)) == false)
        {
            return false;
        }

        switch (type)
        {
            case EObjectSpawnType.ExcludePlayer:
                if (tileCtrl.Model != tile)
                    return true;
                else
                    return false;

            case EObjectSpawnType.IncludePlayer:
                return true;

            case EObjectSpawnType.ExcludeEntites:
                if (preemptiveTiles.Contains(tile) == false)
                    return true;
                else
                    return false;

            case EObjectSpawnType.IncludeEntites:
                if (tileCtrl.Model != tile)
                    return true;
                else
                    return false;

            default:
                break;
        }

        return false;
    }

    public bool CheckTileType(Tile tile, string type)
    {
        if (tile.Landform.GetType().Name == type)
            return true;
        else
            return false;
    }

    public void OcclusionCheck(Tile _targetTile)
    {
        sightTiles = GetTilesInRange(5, _targetTile);
        sightTiles.Add(_targetTile);

        List<StructureObject> structureObjects =
            objectsTransform.GetComponentsInChildren<StructureObject>(true).ToList();

        for (int i = 0; i < structureObjects.Count; i++)
        {
            StructureObject item = structureObjects[i];

            if (sightTiles.Contains(item.CurTile) == false)
            {
                item.gameObject.SetActive(false);
            }
            else
            {
                item.gameObject.SetActive(true);
            }
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

    public List<Tile> GetPlayerSightTiles()
    {
        var list = GetTilesInRange(2);
        return list;
    }

    public List<Tile> GetSightTiles(Tile tile)
    {
        var list = GetTilesInRange(2, tile);
        return list;
    }

    public bool LandformCheck(TileController tileController)
    {
        if (CheckTileType(tileController.Model, "LandformPlain") ||
            CheckTileType(tileController.Model, "LandformRocks"))
        {
            return true;
        }

        return false;
    }
}