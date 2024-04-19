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

    [SerializeField] csFogWar fogOfWar;

    [Header("트랜스폼")] [Space(5f)] [SerializeField]
    Transform zombiesTransform;

    [SerializeField] Transform mapTransform;
    [SerializeField] Transform mapParentTransform;
    [SerializeField] Transform objectsTransform;
    [SerializeField] private GameObject arrowPrefab;
    private GameObject arrow;

    
    [Header("프리팹")] [Space(5f)] [SerializeField]
    MapPrefabSO mapPrefab;

    List<TileController> selectedTiles = new List<TileController>();
    List<TileController> droneSelectedTiles = new List<TileController>();
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

    List<GameObject> distrubtors = new List<GameObject>();
    GameObject curDistrubtor;

    List<GameObject> explorers = new List<GameObject>();
    GameObject curExplorer;

    TileController targetTileController;
    bool isLoadingComplete;

    public bool LoadingComplete => isLoadingComplete;

    public TileController TargetPointTile
    {
        get { return targetTileController; }
    }

    private void Start()
    {
        App.Manager.Map.GetAdditiveSceneObjectsCoroutine();
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
        App.Data.gameData.TryGetValue("Data_MinCount_ZombieObject", out GameData min);
        App.Data.gameData.TryGetValue("Data_MaxCount_ZombieObject", out GameData max);

        SpawnPlayer();

        GenerateTower();
        Generate3TileStructure(new Coords(0, 0));
        Generate7TileStructure(new Coords(0, 0));

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

        //player.TileEffectCheck();

        foreach (var item in GetTilesInRange(player.TileController.Model, 4))
        {
            preemptiveTiles.Add(item);
        }
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
        if (tileController.Model != player.TileController.Model)
        {
            foreach (Coords coords in AStar.FindPath(player.TileController.Model.Coords, tileController.Model.Coords))
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

            if (moveRange != num && tileController.gameObject.GetComponent<TileBase>().Structure?.IsAccessible == false)
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
        var neighborTiles = hexaMap.Map.GetTilesInRange(player.TileController.Model, player.MoveRange);

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

        if (tileController.gameObject.GetComponent<TileBase>().Structure?.IsAccessible == false
            || LandformCheck(tileController) == false)
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
        if (droneSelectedTiles.Contains(tileController))
        {
            curDistrubtor.transform.position =
                ((GameObject)tileController.Model.GameEntity).transform.position + Vector3.up;

            curDistrubtor.GetComponent<Distrubtor>().DirectionObjectOff();

            if (LandformCheck(tileController))
                SelectBorder(tileController, ETileState.Moveable);

            foreach (var item in player.TileController.Model.Neighbours.Where(
                         item => item.Value == tileController.Model))
            {
                curDistrubtor.GetComponent<Distrubtor>().GetDirectionObject(item.Key).SetActive(true);
            }
        }
        else
        {
            SelectBorder(tileController, ETileState.Unable);
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
            && player.TileController.Model != tileController.Model
            && LandformCheck(tileController))
        {
            SavePlayerMovePath(tileController);
            return true;
        }
        else
            return false;
    }

    public void SelectTileForDisturbtor(TileController tileController)
    {
        if (LandformCheck(tileController) == false)
            return;

        if (tileController.GetComponent<Borders>().GetEtileState() == ETileState.Moveable
            && player.TileController.Model != tileController.Model)
        {
            foreach (var item in player.TileController.Model.Neighbours.Where(
                         item => item.Value == tileController.Model))
            {
                Debug.Log("설치 시작");
                InstallDistrubtor(tileController, item.Key);
            }
        }
    }

    public void SelectTileForExplorer(TileController tileController)
    {
        if (tileController.GetComponent<Borders>().GetEtileState() == ETileState.Moveable
            && player.TileController.Model != tileController.Model)
        {
            InstallExplorer(tileController);
        }
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

    public void PreparingDistrubtor(bool set)
    {
        if (set)
        {
            var neighborTiles = hexaMap.Map.GetTilesInRange(player.TileController.Model, 1);

            var neighborController = neighborTiles
                .Select(x => ((GameObject)x.GameEntity).GetComponent<TileController>()).ToList();

            for (var index = 0; index < neighborController.Count; index++)
            {
                var value = neighborController[index];
                if (LandformCheck(value) == false)
                    continue;
                droneSelectedTiles.Add(value);
                SelectTargetBorder(value);
            }

            GenerateExampleDisturbtor();
            App.Manager.Map.SetIsDronePrepared(true, "Distrubtor");
        }
        else
        {
            distrubtors.Remove(curDistrubtor);
            App.Manager.Map.SetIsDronePrepared(false, "Distrubtor");
            UIManager.instance.GetInventoryController().AddItemByItemCode("ITEM_DISTURBE");
            Destroy(curDistrubtor);
            DeselectAllTargetTiles();
        }
    }

    void GenerateExampleDisturbtor()
    {
        Debug.Log("예시 교란기");

        curDistrubtor = Instantiate(mapPrefab.items[(int)EMabPrefab.Disturbtor].prefab,
            player.transform.position + Vector3.up * 1.5f, Quaternion.Euler(0, 90, 0));

        curDistrubtor.transform.parent = mapTransform;
        curDistrubtor.GetComponentInChildren<MeshRenderer>(true).material.DOFade(50, 0);
        distrubtors.Add(curDistrubtor);
    }

    void InstallDistrubtor(TileController tileController, CompassPoint direction)
    {
        curDistrubtor.GetComponent<Distrubtor>().Set(tileController.Model, direction);
        curDistrubtor.GetComponent<Distrubtor>().DirectionObjectOff();

        for (int i = 0; i < droneSelectedTiles.Count; i++)
        {
            DeselecTargetBorder(droneSelectedTiles[i]);
        }

        App.Manager.Map.SetIsDronePrepared(false, "Distrubtor");
    }

    public void PreparingExplorer(bool set)
    {
        if (set)
        {
            GenerateExampleExplorer();
            App.Manager.Map.SetIsDronePrepared(true, "Explorer");
        }
        else
        {
            explorers.Remove(curExplorer);
            App.Manager.Map.SetIsDronePrepared(false, "Explorer");
            UIManager.instance.GetInventoryController().AddItemByItemCode("ITEM_FINDOR");
            Destroy(curExplorer);
        }
    }

    void GenerateExampleExplorer()
    {
        curExplorer = Instantiate(mapPrefab.items[(int)EMabPrefab.Explorer].prefab,
            player.transform.position + Vector3.up * 1.5f, Quaternion.Euler(0, 90, 0));

        curExplorer.transform.parent = mapTransform;

        curExplorer.GetComponentInChildren<MeshRenderer>().material.DOFade(50, 0);
        curExplorer.GetComponent<Explorer>().Set(player.TileController.Model);

        explorers.Add(curExplorer);
    }

    void InstallExplorer(TileController tileController)
    {
        curExplorer.GetComponent<Explorer>().Targeting(tileController.Model);
        curExplorer.GetComponent<Explorer>().Move();

        App.Manager.Map.SetIsDronePrepared(false, "");
    }

    public IEnumerator NextDay()
    {
        bool zombieActEnd = false;

        player.ChangeClockBuffDuration();
        // 플레이어 이동
        if (player.MovePath != null)
        {
            yield return StartCoroutine(player.ActionDecision(targetTileController));
        }
        else
        {
            DeselectAllBorderTiles();
        }

        // 교란기
        if (distrubtors.Count > 0 && distrubtors != null)
        {
            for (int i = 0; i < distrubtors.Count; i++)
            {
                distrubtors[i].GetComponent<Distrubtor>().Move();
            }
        }

        // 탐사기
        if (explorers.Count > 0 && explorers != null)
        {
            for (int i = 0; i < explorers.Count; i++)
            {
                StartCoroutine(explorers[i].GetComponent<Explorer>().Move());
            }
        }


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

                break;
        }

        selectedTiles.Add(tileController);
    }

    void SelectTargetBorder(TileController tileController)
    {
        tileController.GetComponent<Borders>().GetDisturbanceBorder().SetActive(true);
        droneSelectedTiles.Add(tileController);
    }

    void DeselectNormalBorder(TileController tileController)
    {
        tileController.GetComponent<Borders>().OffNormalBorder();

        if (selectedTiles.Contains(tileController))
            selectedTiles.Remove(tileController);
    }

    void DeselecTargetBorder(TileController tileController)
    {
        tileController.GetComponent<Borders>().OffTargetBorder();

        if (droneSelectedTiles.Contains(tileController))
            droneSelectedTiles.Remove(tileController);
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

    public void DeselectAllTargetTiles()
    {
        if (droneSelectedTiles == null)
            return;

        for (int i = 0; i < droneSelectedTiles.Count; i++)
        {
            TileController tile = droneSelectedTiles[i];
            DeselecTargetBorder(tile);
        }

        droneSelectedTiles.Clear();
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
        tileToUnselect.ForEach(t => DeselectNormalBorder(t));
    }

    public Tile GetTileFromCoords(Coords coords)
    {
        return hexaMap.Map.GetTileFromCoords(coords);
    }

    public List<Tile> GetTilesInRange(Tile tile, int num)
    {
        return hexaMap.Map.GetTilesInRange(tile, num);
    }

    public bool CalculateDistanceToPlayer(Tile tile, int range)
    {
        var searchTiles = hexaMap.Map.GetTilesInRange(tile, range);

        return searchTiles.Exists(x => x == player.TileController.Model);
    }

    public Distrubtor CalculateDistanceToDistrubtor(Tile tile, int range)
    {
        var searchTiles = hexaMap.Map.GetTilesInRange(tile, range);

        if (distrubtors.Count <= 0)
            return null;

        for (var i = 0; i < searchTiles.Count; i++)
        {
            var item = searchTiles[i];

            for (var index = 0; index < distrubtors.Count; index++)
            {
                var distrubtor = distrubtors[index];

                if (distrubtor.GetComponent<Distrubtor>().currentTile == item)
                    return distrubtor.GetComponent<Distrubtor>();
            }
        }

        return null;
    }

    public bool CheckPlayersView(TileController tileController)
    {
        var getTiles = GetTilesInRange(player.TileController.Model, 3);

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
        Tile tile = GetTileFromCoords(new Coords(1, 3));

        tilelist.Add(tile);

        List<Tile> neighborList = SetNeighborStructure(tilelist);

        var spawnPos = ((GameObject)tile.GameEntity).transform.position;
        spawnPos.y += 0.31f;

        var tower = Instantiate(mapPrefab.items[(int)EMabPrefab.Tower].prefab, spawnPos, Quaternion.Euler(0, 90, 0),
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

        GameObject structureObject = mapPrefab.items[(int)EMabPrefab.Production].prefab;

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

        GameObject structureObject = mapPrefab.items[(int)EMabPrefab.Army].prefab;

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

        if (randomTile.Structure == null)
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

        List<Tile> excludeTileList = GetTilesInRange(GetTileFromCoords(new Coords(0, 0)), maxInt - range);
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
            if (LandformCheck(TileToTileController(item.Value)) == false)
                continue;

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
                if (player.TileController.Model != tile)
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
                if (player.TileController.Model != tile)
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
        sightTiles = GetTilesInRange(_targetTile, 5);
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
        var list = GetTilesInRange(player.TileController.Model, 2);
        return list;
    }

    public List<Tile> GetSightTiles(Tile tile)
    {
        var list = GetTilesInRange(tile, 2);
        return list;
    }

    public void InputMapData(MapData _mapData)
    {
        mapData = _mapData;
    }

    public void RemoveDistrubtor(Distrubtor _distrubtor)
    {
        distrubtors.Remove(_distrubtor.gameObject);
    }

    public void RemoveExplorer(Explorer _explorer)
    {
        explorers.Remove(_explorer.gameObject);
    }

    public void InvocationExplorers()
    {
        for (int i = 0; i < explorers.Count; i++)
        {
            var item = explorers[i].GetComponent<Explorer>();

            if (item.GetIsIdle())
                item.GetComponent<Explorer>().Invocation();
        }
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

    public void MovePointerOn(Vector3 _pos)
    {
        if (arrow == null)
        {
            arrow = Instantiate(arrowPrefab, _pos, Quaternion.identity);
        }
        
        _pos.y += 1f;
        arrow.transform.position = _pos;
        
        arrow.SetActive(true);
        App.Manager.Sound.PlaySFX("SFX_Map_Select_Complete");
    }
    
    public void MovePointerOff()
    {
        if (arrow == null)
        {
            arrow = Instantiate(arrowPrefab, Vector3.zero, Quaternion.identity);
        }
        
        if (arrow.activeInHierarchy)
        {
            arrow.SetActive(false);
            App.Manager.Sound.PlaySFX("SFX_Map_Select_Cancel");
        }
    }
    
    public void OnlyMovePointerOff()
    {
        if (arrow == null)
        {
            arrow = Instantiate(arrowPrefab, Vector3.zero, Quaternion.identity);
        }
        
        if (arrow.activeInHierarchy)
        {
            arrow.SetActive(false);
        }
    }
}