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

public class MapController : Singleton<MapController>
{
    [Header("컴포넌트")]
    [Space(5f)]
    [SerializeField] HexamapController hexaMap;
    [SerializeField] csFogWar fogOfWar;

    [Header("트랜스폼")]
    [Space(5f)]
    [SerializeField] Transform zombiesTransform;
    [SerializeField] Transform mapTransform;
    [SerializeField] Transform mapParentTransform;

    [Header("프리팹")]
    [Space(5f)]
    [SerializeField] MapPrefabSO mapPrefab;

    List<TileController> selectedTiles = new List<TileController>();
    List<TileController> pathTiles = new List<TileController>();
    List<GameObject> zombiesList = new List<GameObject>();

    Player player;
    public Player Player
    {
        get { return player; }
    }

    [Header("테스트")]
    [Space(5f)]
    GameObject disturbtor;
    GameObject explorer;


    [SerializeField] MapTestManager mapTest;
    [SerializeField] bool isTest;

    TileController targetTileController;

    public TileController TargetPointTile
    {
        get { return targetTileController; }
    }

    private void Start()
    {
        if (!isTest)
            App.instance.GetMapManager().GetAdditiveSceneObjectsCoroutine();
        else
            mapTest.TestStart();
    }

    public void GenerateMap()
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

        GenerateMapObjects();
        mapParentTransform.position = Vector3.forward * 200f;
    }

    public void RegenerateMap()
    {
        Destroy(player);

        foreach (var item in zombiesList)
        {
            Destroy(item.gameObject);
        }

        zombiesList.Clear();
        GenerateMap();
    }

    /// <summary>
    /// 맵에서 스폰되는 오브젝트들에 대한 초기화를 하는 함수이다.
    /// 플레이어, 좀비, 안개를 생성하고, 플레이어의 위치를 리소스 매니저에게 전달한다.
    /// </summary>
    void GenerateMapObjects()
    {
        SpawnPlayer();
        if (!isTest)
        {
            App.instance.GetDataManager().gameData.TryGetValue("Data_MinCount_ZombieObject", out GameData min);
            App.instance.GetDataManager().gameData.TryGetValue("Data_MaxCount_ZombieObject", out GameData max);
            SpawnZombies((int)UnityEngine.Random.Range(min.value, max.value));
            SpawnTestZombie();
        }
        else
        {
            SpawnZombies((int)UnityEngine.Random.Range(0, 30));
            SpawnTestZombie();
        }

        fogOfWar.transform.position = player.transform.position;
        csFogWar.instance.levelMidPoint = player.transform;
        csFogWar.instance.InitializeMapControllerObjects(player.gameObject, 4.5f);
        DeselectAllBorderTiles();
    }

    void SpawnPlayer()
    {
        Vector3 spawnPos = TileToTileController(hexaMap.Map.GetTileFromCoords(new Coords(0, 0))).transform.position;
        spawnPos.y += 0.7f;

        var playerObject = Instantiate(mapPrefab.items[(int)EMabPrefab.Player].prefab, spawnPos, Quaternion.Euler(0, -90, 0));
        player = playerObject.GetComponent<Player>();
        player.transform.parent = mapParentTransform;

        player.UpdateCurrentTile(TileToTileController(hexaMap.Map.GetTileFromCoords(new Coords(0, 0))));
        targetTileController = player.TileController;
        StartCoroutine(FloatingAnimation());
    }

    IEnumerator FloatingAnimation()
    {
        yield return new WaitUntil(() => player != null);
        player.StartFloatingAnimation();
    }

    void SpawnZombies(int zombiesNumber)
    {
        int randomInt;
        var tileList = hexaMap.Map.Tiles;
        List<int> selectTileNumber = new List<int>();

        // 플레이어와 겹치지 않는 랜덤 타일 뽑기.
        while (selectTileNumber.Count != zombiesNumber)
        {
            randomInt = UnityEngine.Random.Range(0, tileList.Count);

            if (tileList[randomInt].Landform.GetType().Name == "LandformPlain"
                && player.TileController.Model != tileList[randomInt])
            {
                if (!selectTileNumber.Contains(randomInt))
                    selectTileNumber.Add(randomInt);
            }
        }

        // 오브젝트 생성.
        for (int i = 0; i < selectTileNumber.Count; i++)
        {
            var tile = tileList[selectTileNumber[i]];
            var spawnPos = ((GameObject)tile.GameEntity).transform.position;
            spawnPos.y += 0.7f;

            var zombie = Instantiate(mapPrefab.items[(int)EMabPrefab.Zombie].prefab, spawnPos, Quaternion.Euler(0, -90, 0), zombiesTransform);
            zombie.name = "Zombie " + (i + 1);
            zombie.GetComponent<ZombieSwarm>().Init(tile);
            zombiesList.Add(zombie);
        }
    }

    public void SpawnTestZombie()
    {
        var tile = GetTileFromCoords(new Coords(0, -1));

        var spawnPos = ((GameObject)tile.GameEntity).transform.position;
        spawnPos.y += 0.7f;

        var zombie = Instantiate(mapPrefab.items[(int)EMabPrefab.Zombie].prefab, spawnPos, Quaternion.Euler(0, -90, 0), zombiesTransform);
        zombie.name = "Zombie " + 1;
        zombie.GetComponent<ZombieSwarm>().Init(tile);

        zombiesList.Add(zombie);

        zombie.GetComponent<ZombieSwarm>().MoveTargetCoroutine(player.TileController.Model);
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
            num = player.HealthPoint;

        int rangeOfMotion = 0;

        if (tileController.Model != player.TileController.Model)
        {
            foreach (Coords coords in AStar.FindPath(player.TileController.Model.Coords, tileController.Model.Coords))
            {
                if (rangeOfMotion == player.HealthPoint)
                    break;

                GameObject border = (GameObject)GetTileFromCoords(coords).GameEntity;
                border.GetComponent<Borders>().GetNormalBorder()?.SetActive(true);
                pathTiles.Add(TileToTileController(GetTileFromCoords(coords)));
                rangeOfMotion++;
            }
        }

        if (rangeOfMotion != num)
            tileController.GetComponent<Borders>().GetAvailableBorder()?.SetActive(true);
        else
            tileController.GetComponent<Borders>().GetUnAvailableBorder()?.SetActive(true);
    }

    public void AddSelectedTilesList(TileController tileController)
    {
        selectedTiles.Add(tileController);
    }

    public void DisturbtorPathFinder(TileController tileController)
    {
        if (hexaMap.Map.GetTilesInRange(player.TileController.Model, 1).Contains(tileController.Model))
        {
            disturbtor.transform.position = ((GameObject)tileController.Model.GameEntity).transform.position + Vector3.up;
            disturbtor.GetComponent<Disturbtor>().DirectionObjectOff();
            SelectBorder(tileController, ETileState.Moveable);

            foreach (var item in player.TileController.Model.Neighbours.Where(item => item.Value == tileController.Model))
            {
                disturbtor.GetComponent<Disturbtor>().GetDirectionObject(item.Key).SetActive(true);
            }
        }
    }

    public bool PlayerCanMoveCheck()
    {
        if (player.HealthPoint != 0)
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
        disturbtor = Instantiate(mapPrefab.items[(int)EMabPrefab.Disturbtor].prefab, player.transform.position + Vector3.up * 1.5f, Quaternion.Euler(0, -90, 0));
        disturbtor.transform.parent = mapTransform;
        disturbtor.GetComponentInChildren<MeshRenderer>().material.DOFade(50, 0);
    }

    void InstallDisturbtor(TileController tileController, CompassPoint direction)
    {
        if (hexaMap.Map.GetTilesInRange(player.TileController.Model, 1).Contains(tileController.Model))
            return;
        disturbtor.GetComponent<Disturbtor>().Set(tileController.Model, direction);
        disturbtor.GetComponent<Disturbtor>().DirectionObjectOff();

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
        explorer = Instantiate(mapPrefab.items[(int)EMabPrefab.Explorer].prefab, player.transform.position + Vector3.up * 1.5f, Quaternion.identity);
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
        if (player.MovePath != null)
        {
            yield return StartCoroutine(player.MoveToTarget(targetTileController));
        }
        else
        {
            DeselectAllBorderTiles();
        }

        if (disturbtor != null)
            disturbtor.GetComponent<Disturbtor>().Move();

        if (explorer != null)
            StartCoroutine(explorer.GetComponent<Explorer>().Move());

        foreach (var zombie in zombiesList)
        {
            zombie.GetComponent<ZombieSwarm>().DetectionAndAct();
        }

        player.HealthCharging();
    }

    public void CheckSumZombies()
    {
        List<Tile> tiles = new List<Tile>();

        foreach (var item in zombiesList)
            tiles.Add(item.GetComponent<ZombieSwarm>().curTile);

        var result = tiles.GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(x => x.Key)
            .Distinct()
            .ToList();

        foreach (var item in result)
        {
            var num = tiles.IndexOf(item);
            for (int i = num + 1; i < tiles.Count; i++)
            {
                if (tiles[num] == tiles[i])
                {
                    var secondZombieSwarm = zombiesList[i].GetComponent<ZombieSwarm>();
                    zombiesList[num].GetComponent<ZombieSwarm>().SumZombies(secondZombieSwarm);
                    zombiesList.RemoveAt(i);
                    Destroy(secondZombieSwarm.gameObject, 0.5f);
                }
            }
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

        foreach (var item in searchTiles)
        {
            if (player.TileController.Model == item)
            {
                return true;
            }
        }
        return false;
    }

    public Disturbtor CalculateDistanceToDistrubtor(Tile tile, int range)
    {
        var searchTiles = hexaMap.Map.GetTilesInRange(tile, range);

        if (disturbtor == null)
            return null;

        foreach (var item in searchTiles)
        {
            if (disturbtor.GetComponent<Disturbtor>().currentTile == item)
            {
                return disturbtor.GetComponent<Disturbtor>();
            }
        }
        return null;
    }

    // 시야 바꾸기
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
        var playerNearthTiles = GetTilesInRange(player.TileController.Model, 1);

        for (int i = 0; i < zombiesList.Count; i++)
        {
            GameObject item = zombiesList[i];
            if (playerNearthTiles.Contains(item.GetComponent<ZombieSwarm>().curTile))
            {
                return true;
            }
        }

        return false;
    }
}
