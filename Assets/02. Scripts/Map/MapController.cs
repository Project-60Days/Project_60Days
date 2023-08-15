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

public enum ETileState
{
    None,
    Moveable,
    Unable,
    Target
}

public enum EMabPrefab
{
    Player,
    Zombie,
    Disturbtor,
    Explorer
}

public class MapController : Singleton<MapController>
{
    #region 변수


    [Header("컴포넌트")]
    [Space(5f)]

    [SerializeField] HexamapController map;
    [SerializeField] ResourceManager resourceManager;
    [SerializeField] CsFogWar fogOfWar;
    [SerializeField] Transform zombiesTransform;
    [SerializeField] Transform mapTransform;
    [SerializeField] MapPrefabSO mapPrefab;

    [Header("카메라")]
    [Space(5f)]
    [SerializeField] Camera mapCamera;

    List<TileController> selectedTiles = new List<TileController>();
    List<GameObject> zombiesList = new List<GameObject>();

    List<Coords> movePath;

    Player player;
    GameObject disturbtor;
    GameObject explorer;
    GameObject currentUI;

    ArrowToMove arrow;
    NoteAnim noteAnim;
    TileController targetTile;

    int currentHealth;
    int maxHealth = 1;

    bool isUIOn;

    public bool UIOn
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
            if (player.Tile.Model == item)
            {
                return true;
            }
        }
        return false;
    }

    public bool isTutorialUiOn()
    {
        if (currentUI == null)
            return false;

        return currentUI.transform.parent.parent.GetComponent<TileInfo>().TutorialTile;
    }

    public void OffCurrentUI()
    {
        currentUI.SetActive(false);
        isUIOn = false;
        currentUI = null;
    }

    public Disturbtor CalculateDistanceToDistrubtor(Tile tile, int range)
    {
        var searchTiles = map.Map.GetTilesInRange(tile, range);

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

        var zombie = Instantiate(mapPrefab.items[(int)EMabPrefab.Zombie].prefab, spawnPos, Quaternion.Euler(0, -90, 0), zombiesTransform);
        zombie.name = "Zombie " + 1;
        zombie.GetComponent<ZombieSwarm>().Init(tile);

        zombiesList.Add(zombie);

        zombie.GetComponent<ZombieSwarm>().MoveTargetCoroutine(player.Tile.Model);
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
        return player.Tile.Model;
    }
    #endregion

    void Start()
    {
        StartCoroutine(GetAdditiveSceneObjects());
        GenerateMap();

        currentHealth = maxHealth;
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

        GenerateMapObjects();
        mapTransform.position = Vector3.forward * 200f;
    }

    void Update()
    {
        if (!isPlayerMoving && !noteAnim.GetIsOpen())
        {
            MouseOverEvents();
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
        FischlWorks_FogWar.CsFogWar.instance.InitializeMapControllerObjects(player.gameObject, 5);

        
        DeselectAllBorderTiles();
    }


    void SpawnPlayer()
    {
        player.Tile = TileToTileController(map.Map.GetTileFromCoords(new Coords(0, 0)));
        targetTile = player.Tile;

        Vector3 spawnPos = player.Tile.transform.position;
        spawnPos.y += 0.7f;

        var playerObject = Instantiate(mapPrefab.items[(int)EMabPrefab.Player].prefab, spawnPos, Quaternion.Euler(0, -90, 0));
        player = playerObject.GetComponent<Player>();
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
                && player.Tile.Model != tileList[randomInt])
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


    /// <summary>
    /// Raytracing을 통해 마우스 현재 위치에 맞는 타일의 정보를 가져오거나, 타일의 하위 오브젝트를 활성화시키는 함수.
    /// </summary>
    void MouseOverEvents()
    {
        RaycastHit hit;
        TileController tileController;

        Ray ray = mapCamera.ScreenPointToRay(Input.mousePosition);
        int onlyLayerMaskTile = 1 << LayerMask.NameToLayer("Tile");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile))
        {
            tileController = hit.transform.parent.GetComponent<TileController>();

            DeselectAllBorderTiles();

            if (!isPlayerSelected)
            {
                DefalutMouseOverState(tileController);
            }
            else if (isPlayerCanMove)
            {
                TilePathFinder(tileController, currentHealth);
                selectedTiles.Add(tileController);
            }
            else if (isDisturbanceSet)
            {
                DisturbtorPathFinder(tileController);
            }
            else if (isExplorerSet)
            {
                TilePathFinder(tileController, 5);
            }
        }
        else
        {
            DeselectAllBorderTiles();

            if (isUIOn)
            {
                currentUI.SetActive(false);
                isUIOn = false;
            }
        }

        MouseClickEvents();
    }

    void DefalutMouseOverState(TileController tileController)
    {
        if (tileController != null && !selectedTiles.Contains(tileController))
        {
            SelectBorder(tileController, ETileState.Unable);

            if (isUIOn)
            {
                currentUI.SetActive(false);
                isUIOn = false;
            }
        }
    }

    void TilePathFinder(TileController tileController, int num)
    {
        int rangeOfMotion = 0;

        if (tileController.Model != player.Tile.Model)
        {
            foreach (Coords coords in AStar.FindPath(playerLocationTileController.Model.Coords, tileController.Model.Coords))
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

    void DisturbtorPathFinder(TileController tileController)
    {
        if (map.Map.GetTilesInRange(playerLocationTileController.Model, 1).Contains(tileController.Model))
        {
            disturbtor.transform.position = ((GameObject)tileController.Model.GameEntity).transform.position + Vector3.up;
            disturbtor.GetComponent<Disturbtor>().DirectionObjectOff();
            SelectBorder(tileController, ETileState.Moveable);

            foreach (var item in playerLocationTileController.Model.Neighbours.Where(item => item.Value == tileController.Model))
            {
                disturbtor.GetComponent<Disturbtor>().GetDirectionObject(item.Key).SetActive(true);
            }
        }
    }

    void MouseClickEvents()
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
                    if (GetTileBorder(tileController, ETileState.Moveable).activeInHierarchy 
                        && playerLocationTileController.Model != tileController.Model)
                    {
                        SavePlayerMovePath(tileController);
                    }
                }
                else if (isDisturbanceSet)
                {
                    if (map.Map.GetTilesInRange(playerLocationTileController.Model, 1).Contains(tileController.Model) 
                        && GetTileBorder(tileController, ETileState.Moveable).activeInHierarchy)
                    {
                        foreach (var item in playerLocationTileController.Model.Neighbours.Where(item => item.Value == tileController.Model))
                        {
                            InstallDisturbtor(tileController, item.Key);
                        }
                    }
                }
                else if (isExplorerSet)
                {
                    if (GetTileBorder(tileController, ETileState.Moveable).activeInHierarchy 
                        && playerLocationTileController.Model != tileController.Model)
                    {
                        InstallExplorer(tileController);
                    }
                }

            }
        }

        // 우클릭 시 선택 취소
        if (Input.GetMouseButtonDown(1))
        {
            DeselectAllBorderTiles();

            if (isPlayerCanMove)
            {
                isPlayerSelected = false;
                isPlayerCanMove = false;
            }
            else if (isDisturbanceSet)
            {
                DisturbtorSetting(false);
            }
            else if (isExplorerSet)
            {
                ExplorerSettting(false);
            }
        }
    }

    void SavePlayerMovePath(TileController tileController)
    {
        targetTile = tileController;
        movePath = AStar.FindPath(playerLocationTileController.Model.Coords, tileController.Model.Coords);
        arrow.OnEffect(tileController.transform);

        isPlayerSelected = false;
        isPlayerCanMove = false;

        DeselectAllBorderTiles();
    }

    IEnumerator MovePlayer(Vector3 lastTargetPos, float time = 0.4f)
    {
        isPlayerMoving = true;

        DeselectAllBorderTiles();

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
        //Player로 이동
        //PlayerSightUpdate?.Invoke(playerLocationTileController.Model);
        // MapManager로 이동
        //resourceManager.GetResource(playerLocationTileController);
        //arrow.OffEffect();
    }

    TileController TileToTileController(Tile tile)
    {
        return ((GameObject)tile.GameEntity).GetComponent<TileController>();
    }

    #region UI 씬 버튼 관련
    public void DisturbtorSetting(bool set)
    {
        List<Tile> nearthTiles = GetTilesInRange(playerLocationTileController.Model, 1);

        if (set)
        {
            for (int i = 0; i < nearthTiles.Count; i++)
            {
                SelectBorder(TileToTileController(nearthTiles[i]), ETileState.Target);
            }

            GenerateDisturbtor();

            isDisturbanceSet = true;
            isPlayerSelected = true;
        }
        else
        {
            for (int i = 0; i < nearthTiles.Count; i++)
            {
                DeselectBorder(TileToTileController(nearthTiles[i]));
            }

            Destroy(disturbtor);

            isDisturbanceSet = false;
            isPlayerSelected = false;
        }
    }

    void GenerateDisturbtor()
    {
        disturbtor = Instantiate(mapPrefab.items[(int)EMabPrefab.Disturbtor].prefab, player.transform.position + Vector3.up * 1.5f, Quaternion.Euler(0, -90, 0));
        disturbtor.transform.parent = mapTransform;
        disturbtor.GetComponentInChildren<MeshRenderer>().material.DOFade(50, 0);
    }

    void InstallDisturbtor(TileController tileController, CompassPoint direction)
    {
        disturbtor.GetComponent<Disturbtor>().Set(tileController.Model, direction);
        disturbtor.GetComponent<Disturbtor>().DirectionObjectOff();

        List<Tile> nearthTiles = GetTilesInRange(playerLocationTileController.Model, 1);
        for (int i = 0; i < nearthTiles.Count; i++)
        {
            DeselectBorder(TileToTileController(nearthTiles[i]));
        }

        isPlayerSelected = false;
        isDisturbanceSet = false;
        isDisturbanceInstall = true;
    }

    public void ExplorerSettting(bool set)
    {
        if (set)
        {
            GenerateExplorer();
            isExplorerSet = true;
            isPlayerSelected = true;
        }
        else
        {
            Destroy(explorer);
            isExplorerSet = false;
            isPlayerSelected = false;
        }
    }

    void GenerateExplorer()
    {
        explorer = Instantiate(mapPrefab.items[(int)EMabPrefab.Explorer].prefab, player.transform.position + Vector3.up * 1.5f, Quaternion.identity);
        explorer.transform.parent = mapTransform;

        explorer.GetComponentInChildren<MeshRenderer>().material.DOFade(50, 0);
        explorer.GetComponent<Explorer>().Set(playerLocationTileController.Model);
    }

    void InstallExplorer(TileController tileController)
    {
        explorer.GetComponent<Explorer>().Targetting(tileController.Model);
        explorer.GetComponent<Explorer>().Move();

        isPlayerSelected = false;
        isExplorerSet = false;
    }

    public void NextDay()
    {
        playerLocationTileController = targetTile;

        if (movePath != null)
        {
            StartCoroutine(MovePlayer(targetTile.transform.position));
        }
        else
        {
            isPlayerMoving = true;
            DeselectAllBorderTiles();

            currentHealth = 0;
            //PlayerSightUpdate?.Invoke(playerLocationTileController.Model);
            arrow.OffEffect();

            // MapManager로 이동
            //resourceManager.GetResource(targetTile);
        }

        currentHealth = maxHealth;

        if (disturbtor != null)
            disturbtor.GetComponent<Disturbtor>().Move();

        if (explorer != null)
            StartCoroutine(explorer.GetComponent<Explorer>().Move());

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

    #region 타일 선택 경계선 관련

    void SelectBorder(TileController tileController, ETileState state)
    {
        switch (state) 
        {
            case ETileState.None:
                tileController.GetComponent<Borders>().GetNormalBorder()?.SetActive(true);
                break;
            case ETileState.Moveable:
                tileController.GetComponent<Borders>().GetAvailableBorder()?.SetActive(true);
                break;
            case ETileState.Unable:
                tileController.GetComponent<Borders>().GetUnAvailableBorder()?.SetActive(true);
                break;
            case ETileState.Target:
                tileController.GetComponent<Borders>().GetDisturbanceBorder()?.SetActive(true);
                break;
        }
        selectedTiles.Add(tileController);
    }

    void DeselectBorder(TileController tileController)
    {
        tileController.GetComponent<Borders>().OffAllBorders();

        if(selectedTiles.Contains(tileController))
            selectedTiles.Remove(tileController);
    }

    void DeselectAllBorderTiles()
    {
        if (selectedTiles == null)
            return;

        foreach (var tile in selectedTiles)
        {
            if (tile == null)
                continue;
            DeselectBorder(tile);
        }

        selectedTiles.Clear();
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
    #endregion
}
