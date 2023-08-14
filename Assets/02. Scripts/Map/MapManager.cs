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

public enum TileState
{
    None,
    Moveable,
    Unable,
    Target
}

public class MapManager : ManagementBase
{
    #region 변수
    public static Action<Tile> PlayerSightUpdate;

    [Header("컴포넌트")]
    [Space(5f)]

    [SerializeField] HexamapController map;
    [SerializeField] ResourceManager resourceManager;
    [SerializeField] CsFogWar fogOfWar;
    [SerializeField] Text statText;
    [SerializeField] Transform zombiesTransform;
    [SerializeField] Transform mapTransform;

    [Header("프리팹")]
    [Space(5f)]
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject zombiePrefab;
    [SerializeField] GameObject disturbanceMachinePrefab;
    [SerializeField] GameObject explorerMachinePrefab;


    [Header("카메라")]
    [Space(5f)]
    [SerializeField] Camera mapCamera;

    List<TileController> selectedTiles = new List<TileController>();
    List<GameObject> disrubtorBorders = new List<GameObject>();
    List<GameObject> zombiesList = new List<GameObject>();

    List<Coords> selectedPathTiles;
    List<Coords> movePath;

    GameObject player;
    GameObject disturbanceMachine;
    GameObject explorerMachine;
    GameObject currentUI;

    ArrowToMove arrow;
    NoteAnim noteAnim;
    Tile playerLocationTile;
    Tile prevPlayerLocationTile;
    TileController targetTile;

    int currentHealth;
    int maxHealth = 1;

    bool isUIOn;

    public bool UiOn
    {
        get 
        { 
            return isUIOn; 
        }
    }

    bool isBaseOn;

    bool isPlayerMoving;
    bool isPlayerCanMove;
    bool isPlayerSelected;
    
    bool isExplorerSet;
    bool isDisturbanceSet;
    bool isDisturbanceInstall;

    public bool DisturbanceInstall
    {
        get 
        { 
            return isDisturbanceInstall; 
        }
    }

    #endregion

    #region 외부 호출 함수들

    public bool CalculateDistanceToPlayer(Tile tile, int range)
    {
        var searchTiles = map.Map.GetTilesInRange(tile, range);

        foreach (var item in searchTiles)
        {
            if (playerLocationTile == item)
            {
                return true;
            }
        }
        return false;
    }

    public bool isTutorialUiOn()
    {
        if (isUIOn)
            return currentUI.transform.parent.parent.GetComponent<TileInfo>().isTutorialTile;

        return false;
    }

    public void CurrentUIEmptying()
    {
        currentUI.SetActive(false);
        isUIOn = false;
        currentUI = null;
    }

    public DisturbanceMachine CalculateDistanceToDistrubtor(Tile tile, int range)
    {
        var searchTiles = map.Map.GetTilesInRange(tile, range);

        if (disturbanceMachine == null)
            return null;

        foreach (var item in searchTiles)
        {
            if (disturbanceMachine.GetComponent<DisturbanceMachine>().currentTile == item)
            {
                return disturbanceMachine.GetComponent<DisturbanceMachine>();
            }
        }
        return null;
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

    public bool CheckSelected()
    {
        if (isPlayerSelected || isPlayerMoving || isDisturbanceSet || isExplorerSet)
        {
            return false;
        }
        return true;
    }

    public void AllowMouseEvent()
    {
        isPlayerMoving = false;
    }

    public void SpawnTutorialZombie()
    {
        var tile = GetTileFromCoords(new Coords(0, -3));

        var spawnPos = ((GameObject)tile.GameEntity).transform.position;
        spawnPos.y += 0.7f;

        var zombie = Instantiate(zombiePrefab, spawnPos, Quaternion.Euler(0, -90, 0), zombiesTransform);
        zombie.name = "Zombie " + 1;
        zombie.GetComponent<ZombieSwarm>().Init(tile);

        zombiesList.Add(zombie);

        zombie.GetComponent<ZombieSwarm>().MoveTargetCoroutine(playerLocationTile);
    }

    public Tile GetTileFromCoords(Coords coords)
    {
        return map.Map.GetTileFromCoords(coords);
    }

    public List<Tile> GetTilesInRange(Tile tile, int num)
    {
        return map.Map.GetTilesInRange(tile, num);
    }

    public Tile GetPlayerLocationTile()
    {
        return playerLocationTile;
    }
    #endregion

    void Start()
    {
        StartCoroutine(GetAdditiveSceneObjects());
        GenerateMap();

        currentHealth = maxHealth;
        mapTransform.position = Vector3.forward * 200f;
    }

    IEnumerator GetAdditiveSceneObjects()
    {
        yield return new WaitForEndOfFrame();
        mapCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        noteAnim = GameObject.FindGameObjectWithTag("NoteAnim").GetComponent<NoteAnim>();
        arrow = GameObject.FindGameObjectWithTag("MapUi").transform.GetChild(0).transform.Find("Map_Arrow").GetComponent<ArrowToMove>();
    }

    void GenerateMap()
    {
        map.Destroy();

        var timeBefore = DateTime.Now;

        map.Generate();

        double timeSpent = (DateTime.Now - timeBefore).TotalSeconds;

        map.Draw();

        // Add some noise to Y position of tiles
        FastNoise _fastNoise = new FastNoise();
        _fastNoise.SetFrequency(0.1f);
        _fastNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
        _fastNoise.SetSeed(map.Map.Seed);
        foreach (Tile tile in map.Map.Tiles)
        {
            var noiseY = _fastNoise.GetValue(tile.Coords.X, tile.Coords.Y);
            (tile.GameEntity as GameObject).transform.position += new Vector3(0, noiseY * 2, 0);
        }

        // Output stats
        statText.text = $"Map generated in {timeSpent.ToString("0.000")} seconds.";
        //Debug.Log($"Seed : { Hexamap.Map.Seed }");

        GenerateMapObjects();
    }

    void Update()
    {
        if (!isPlayerMoving && !noteAnim.GetIsOpen())
        {
            MouseEvent();
        }
    }

    void RegenerateMap()
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
        App.instance.GetDataManager().gameData.TryGetValue("Data_MinCount_ZombieObject", out GameData min);
        App.instance.GetDataManager().gameData.TryGetValue("Data_MaxCount_ZombieObject", out GameData max);

        SpawnPlayer();
        //SpawnZombies((int)UnityEngine.Random.Range(min.value, max.value));

        fogOfWar.transform.position = player.transform.position;
        FischlWorks_FogWar.CsFogWar.instance.InitializeMapControllerObjects(player, 5);

        resourceManager.SetTile(playerLocationTile);
        StartCoroutine(DelaySightGetInfo());
        DeselectAllTile();
    }


    void SpawnPlayer()
    {
        playerLocationTile = map.Map.GetTileFromCoords(new Coords(0, 0));
        prevPlayerLocationTile = playerLocationTile;
        targetTile = ((GameObject)playerLocationTile.GameEntity).GetComponent<TileController>();

        Vector3 spawnPos = ((GameObject)playerLocationTile.GameEntity).transform.position;
        spawnPos.y += 0.7f;

        player = Instantiate(playerPrefab, spawnPos, Quaternion.Euler(0, -90, 0));
        player.transform.parent = mapTransform;
    }

    void SpawnZombies(int zombiesNumber)
    {
        int randomInt;
        var tileList = map.Map.Tiles;
        List<int> selectTileNumber = new List<int>();

        // 플레이어와 겹치지 않는 랜덤 타일 뽑기.
        while (selectTileNumber.Count != zombiesNumber)
        {
            randomInt = UnityEngine.Random.Range(0, tileList.Count);

            if (tileList[randomInt].Landform.GetType().Name == "LandformPlain"
                && playerLocationTile != tileList[randomInt])
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

            var zombie = Instantiate(zombiePrefab, spawnPos, Quaternion.Euler(0, -90, 0), zombiesTransform);
            zombie.name = "Zombie " + (i + 1);
            zombie.GetComponent<ZombieSwarm>().Init(tile);
            zombiesList.Add(zombie);
        }
    }

    /// <summary>
    /// 플레이어가 서 있는 타일의 위치를 갱신할 때마다 그 타일의 정보를 넘겨주는 이벤트 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator DelaySightGetInfo()
    {
        // AdditiveScene 딜레이 용
        yield return new WaitForEndOfFrame();
        PlayerSightUpdate?.Invoke(playerLocationTile);
    }

    /// <summary>
    /// Raytracing을 통해 마우스 현재 위치에 맞는 타일의 정보를 가져오거나, 타일의 하위 오브젝트를 활성화시키는 함수.
    /// </summary>
    void MouseEvent()
    {
        RaycastHit hit;
        TileController tileController;

        Ray ray = mapCamera.ScreenPointToRay(Input.mousePosition);
        int onlyLayerMaskTile = 1 << LayerMask.NameToLayer("Tile");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile))
        {
            tileController = hit.transform.parent.GetComponent<TileController>();

            DeselectAllTile();
            DeselectAllPathTile();

            if (!isPlayerSelected)
            {
                AnyCondition(tileController);
            }
            else if (isPlayerCanMove)
            {
                PathFinder(tileController, currentHealth);
                selectedTiles.Add(tileController);
            }
            else if (isDisturbanceSet)
            {
                DisturbancePathFinder(tileController);
            }
            else if (isExplorerSet)
            {
                PathFinder(tileController, 5);
            }
        }
        else
        {
            DeselectAllTile();
            DeselectAllPathTile();

            if (isUIOn)
            {
                currentUI.SetActive(false);
                isUIOn = false;
            }
        }

        MouseClickTile();
    }

    void AnyCondition(TileController tileController)
    {
        if (tileController != null && !selectedTiles.Contains(tileController))
        {
            SelectBorder(tileController, TileState.Unable);

            if (isUIOn)
            {
                currentUI.SetActive(false);
                isUIOn = false;
            }
        }
    }

    void PathFinder(TileController tileController, int num)
    {
        int rangeOfMotion = 0;

        if (tileController.Model != playerLocationTile)
        {
            foreach (Coords coords in AStar.FindPath(playerLocationTile.Coords, tileController.Model.Coords))
            {
                if (rangeOfMotion == currentHealth)
                    break;

                GameObject border = (GameObject)GetTileFromCoords(coords).GameEntity;
                border.GetComponent<Borders>().GetNormalBorder()?.SetActive(true);
                rangeOfMotion++;
            }
        }

        if (rangeOfMotion != num)
            tileController.GetComponent<Borders>().GetAvailableBorder()?.SetActive(true);
        else
            tileController.GetComponent<Borders>().GetUnAvailableBorder()?.SetActive(true);
    }

    void DisturbancePathFinder(TileController tileController)
    {
        if (map.Map.GetTilesInRange(playerLocationTile, 1).Contains(tileController.Model))
        {
            disturbanceMachine.transform.position = ((GameObject)tileController.Model.GameEntity).transform.position + Vector3.up;
            disturbanceMachine.GetComponent<DisturbanceMachine>().DirectionObjectOff();
            SelectBorder(tileController, TileState.Moveable);

            foreach (var item in playerLocationTile.Neighbours.Where(item => item.Value == tileController.Model))
            {
                disturbanceMachine.GetComponent<DisturbanceMachine>().GetDirectionObject(item.Key).SetActive(true);
            }
        }
    }

    void MouseClickTile()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        RaycastHit hit;
        Ray ray = mapCamera.ScreenPointToRay(Input.mousePosition);

        int onlyLayerMaskPlayer = 1 << LayerMask.NameToLayer("Player");
        int onlyLayerMaskTile = 1 << LayerMask.NameToLayer("Tile");

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskPlayer) 
                && !isPlayerCanMove && !isDisturbanceSet && !isExplorerSet && currentHealth != 0)
            {
                isPlayerSelected = true;
                isPlayerCanMove = true;
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile) )
            {
                TileController tileController = hit.transform.parent.GetComponent<TileController>();

                if (!isPlayerSelected)
                {
                    currentUI = GetUi(tileController);
                    currentUI.SetActive(true);
                    isUIOn = true;
                }
                else if (isPlayerCanMove)
                {
                    if (GetTileBorder(tileController, TileState.Moveable).activeInHierarchy && playerLocationTile != tileController.Model)
                    {
                        SavePlayerMovePath(tileController);
                    }
                }
                else if (isDisturbanceSet)
                {
                    if (map.Map.GetTilesInRange(playerLocationTile, 1).Contains(tileController.Model) 
                        && GetTileBorder(tileController, TileState.Moveable).activeInHierarchy)
                    {
                        foreach (var item in playerLocationTile.Neighbours.Where(item => item.Value == tileController.Model))
                        {
                            SetCompleteDisturbanceMachine(tileController, item.Key);
                        }
                    }
                }
                else if (isExplorerSet)
                {
                    if (GetTileBorder(tileController, TileState.Moveable).activeInHierarchy && playerLocationTile != tileController.Model)
                    {
                        SetCompleteExplorerMachine(tileController);
                    }
                }

            }
        }

        // 우클릭 시 선택 취소
        if (Input.GetMouseButtonDown(1))
        {
            DeselectAllTile();
            DeselectAllPathTile();

            if (isPlayerCanMove)
            {
                isPlayerSelected = false;
                isPlayerCanMove = false;
            }
            else if (isDisturbanceSet)
            {
                DisturbanceMachineSetting(false);
            }
            else if (isExplorerSet)
            {
                ExplorerMachineSettting(false);
            }
        }
    }

    void SavePlayerMovePath(TileController tileController)
    {
        targetTile = tileController;
        movePath = AStar.FindPath(playerLocationTile.Coords, tileController.Model.Coords);
        arrow.OnEffect(tileController.transform);

        isPlayerSelected = false;
        isPlayerCanMove = false;

        DeselectAllTile();
        DeselectAllPathTile();
    }

    IEnumerator MovePlayer(Vector3 lastTargetPos, float time = 0.4f)
    {
        isPlayerMoving = true;
        DeselectAllTile();
        DeselectAllPathTile();

        Tile targetTile;
        Vector3 targetPos;

        foreach (var item in movePath)
        {
            targetTile = map.Map.GetTileFromCoords(item);
            if (targetTile == null)
                break;

            targetPos = ((GameObject)targetTile.GameEntity).transform.position;
            targetPos.y += 0.5f;

            player.transform.DOMove(targetPos, time);
            currentHealth--;
            yield return new WaitForSeconds(time);
        }

        lastTargetPos.y += 0.5f;
        yield return player.transform.DOMove(lastTargetPos, time);
        yield return new WaitForSeconds(time);

        movePath.Clear();
        currentHealth = 0;
        PlayerSightUpdate?.Invoke(playerLocationTile);
        resourceManager.GetResource();
        arrow.OffEffect();
    }

    TileController TileToTileController(Tile tile)
    {
        return ((GameObject)tile.GameEntity).GetComponent<TileController>();
    }

    #region UI 씬 버튼 관련
    public void DisturbanceMachineSetting(bool set)
    {
        List<Tile> nearthTiles = GetTilesInRange(playerLocationTile, 1);

        if (set)
        {
            for (int i = 0; i < nearthTiles.Count; i++)
            {
                SelectBorder(TileToTileController(nearthTiles[i]), TileState.Target);
            }
            GenerateDisturbanceMachine();
            isDisturbanceSet = true;
            isPlayerSelected = true;
        }
        else
        {
            for (int i = 0; i < nearthTiles.Count; i++)
            {
                UnSelectBorder(TileToTileController(nearthTiles[i]));
            }
            Destroy(disturbanceMachine);
            isDisturbanceSet = false;
            isPlayerSelected = false;
        }
    }

    void GenerateDisturbanceMachine()
    {
        disturbanceMachine = Instantiate(disturbanceMachinePrefab, player.transform.position + Vector3.up * 1.5f, Quaternion.Euler(0, -90, 0));
        disturbanceMachine.transform.parent = mapTransform;
        disturbanceMachine.GetComponentInChildren<MeshRenderer>().material.DOFade(50, 0);
    }

    public void ExplorerMachineSettting(bool set)
    {
        if (set)
        {
            GenerateExplorerMachine();
            isExplorerSet = true;
            isPlayerSelected = true;
        }
        else
        {
            Destroy(explorerMachine);
            isExplorerSet = false;
            isPlayerSelected = false;
        }
    }

    void GenerateExplorerMachine()
    {
        explorerMachine = Instantiate(explorerMachinePrefab, player.transform.position + Vector3.up * 1.5f, Quaternion.identity);
        explorerMachine.transform.parent = mapTransform;
        explorerMachine.GetComponentInChildren<MeshRenderer>().material.DOFade(50, 0);
        explorerMachine.GetComponent<Explorer>().Set(playerLocationTile);
    }

    void SetCompleteDisturbanceMachine(TileController tileController, CompassPoint direction)
    {
        disturbanceMachine.GetComponent<DisturbanceMachine>().Set(tileController.Model, direction);
        disturbanceMachine.GetComponent<DisturbanceMachine>().DirectionObjectOff();

        List<Tile> nearthTiles = GetTilesInRange(playerLocationTile, 1);

        for (int i = 0; i < nearthTiles.Count; i++)
        {
            UnSelectBorder(TileToTileController(nearthTiles[i]));
        }

        isPlayerSelected = false;
        isDisturbanceSet = false;
        isDisturbanceInstall = true;
    }

    void SetCompleteExplorerMachine(TileController tileController)
    {
        explorerMachine.GetComponent<Explorer>().Targetting(tileController.Model);
        explorerMachine.GetComponent<Explorer>().Move();

        isPlayerSelected = false;
        isExplorerSet = false;

        DeselectAllPathTile();
    }

    public void NextDay()
    {
        resourceManager.SetTile(targetTile.Model);
        prevPlayerLocationTile = playerLocationTile;
        playerLocationTile = targetTile.Model;

        if (movePath != null)
        {
            StartCoroutine(MovePlayer(targetTile.transform.position));
        }
        else
        {
            isPlayerMoving = true;
            DeselectAllTile();
            DeselectAllPathTile();

            currentHealth = 0;
            PlayerSightUpdate?.Invoke(playerLocationTile);
            arrow.OffEffect();

            if (prevPlayerLocationTile == playerLocationTile)
                resourceManager.GetResource();
        }

        currentHealth = maxHealth;

        if (disturbanceMachine != null)
            disturbanceMachine.GetComponent<DisturbanceMachine>().Move();

        if (explorerMachine != null)
            StartCoroutine(explorerMachine.GetComponent<Explorer>().Move());

        foreach (var zombie in zombiesList)
        {
            zombie.GetComponent<ZombieSwarm>().DetectionAndAct();
        }
    }

    public GameObject GetUi(TileController tile)
    {
        GameObject tileGO = (GameObject)tile.Model.GameEntity;

        if (tileGO != null && tile.Model.Landform.GetType().Name != "LandformWorldLimit")
            return tileGO.GetComponent<TileInfo>().GetUiObject();

        return null;
    }

    public GameObject GetUi(Tile tile)
    {
        GameObject tileGO = (GameObject)tile.GameEntity;

        if (tileGO != null && tile.Landform.GetType().Name != "LandformWorldLimit")
            return tileGO.transform.Find("Canvas").Find("TileInfo").gameObject;

        return null;
    }

    #endregion

    #region 선택 타일 관련
    void DeselectAllTile()
    {
        if (selectedTiles == null)
            return;

        foreach (var t in selectedTiles)
        {
            if (t == null)
                continue;
            UnSelectBorder(t);
        }
        selectedTiles.Clear();
    }

    void DeselectAllPathTile()
    {
        if (selectedPathTiles == null)
            return;

        foreach (var t in selectedPathTiles)
        {
            UnSelectBorder(TileToTileController(GetTileFromCoords(t)));
        }
        selectedPathTiles.Clear();
    }

    void UnselectTile(TileController tile)
    {
        UnSelectBorder(tile);
        selectedTiles.Remove(tile);
    }


    /// <summary>
    /// 같은 그룹 타일이 모여있는 곳에 마우스를 올리면 그룹선택을 하게 해주는 함수. 현재 사용 중이지 않다.
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

        tileToSelect.ForEach(t => SelectBorder(t, TileState.Unable));
        tileToUnselect.ForEach(t => UnselectTile(t));
    }

    void SelectBorder(TileController tileController, TileState state)
    {
        switch (state) 
        {
            case TileState.None:
                tileController.GetComponent<Borders>().GetNormalBorder()?.SetActive(true);
                break;
            case TileState.Moveable:
                tileController.GetComponent<Borders>().GetAvailableBorder()?.SetActive(true);
                break;
            case TileState.Unable:
                tileController.GetComponent<Borders>().GetUnAvailableBorder()?.SetActive(true);
                break;
            case TileState.Target:
                tileController.GetComponent<Borders>().GetDisturbanceBorder()?.SetActive(true);
                break;
        }
        selectedTiles.Add(tileController);
    }

    void UnSelectBorder(TileController tileController)
    {
        tileController.GetComponent<Borders>().OffAllBorders();
    }

    GameObject GetTileBorder(TileController tileController, TileState state)
    {
        switch (state)
        {
            case TileState.None:
                return tileController.GetComponent<Borders>().GetNormalBorder();
            case TileState.Moveable:
                return tileController.GetComponent<Borders>().GetAvailableBorder();
            case TileState.Unable:
                return tileController.GetComponent<Borders>().GetUnAvailableBorder();
        }

        return null;
    }

    public override EManagerType GetManagemetType()
    {
        return EManagerType.MAP;
    }
    #endregion
}
