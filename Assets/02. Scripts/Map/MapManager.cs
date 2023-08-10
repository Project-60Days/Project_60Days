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

public enum TileState
{
    None,
    Moveable,
    Unable
}

public class MapManager : ManagementBase
{
    #region 변수
    [SerializeField] HexamapController Hexamap;
    [SerializeField] Text TextStats;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject zombiePrefab;
    [SerializeField] GameObject distrubtorPrefab;
    [SerializeField] GameObject explorerPrefab;
    [SerializeField] Transform zombiesTr;
    [SerializeField] Transform hexamapTr;
    [SerializeField] Camera mapCamera;
    [SerializeField] GameObject player;
    [SerializeField] GameObject fog;
    [SerializeField] ResourceManager resourceManager;

    public Tile playerLocationTile;
    public Tile prevTile;
    public TileController targetTile;


    [Header("카메라 설정")]
    [Space(5f)]
    [SerializeField] int MaxYPosition = 70;
    [SerializeField] int MinYPosition = 5;
    [SerializeField] int MinXRotation = 30;
    [SerializeField] int MaxXRotation = 90;
    [SerializeField] int ScrollSpeed = 50;
    [SerializeField] int MoveSpeed = 50;
    ArrowToMove arrow;

    [Header("게임 씬")]
    [Space(5f)]
    public bool isBaseOn;

    List<TileController> selectedTiles = new List<TileController>();
    List<GameObject> disrubtorBorders = new List<GameObject>();
    List<GameObject> zombiesList = new List<GameObject>();
    List<Coords> selectPath;
    List<Coords> movePath;

    TMP_Text textHealth;
    GameObject disturbanceMachine;
    GameObject explorerObject;
    GameObject currentUI;

    int health;
    int maxHealth = 1;

    bool isPlayerSelected;
    bool isPlayerCanMove;
    bool isUIOn;
    bool isDisturbanceSet;
    bool isExplorerSet;
    bool isPlayerMoving;
    bool isDisturbanceInstall;
    NoteAnim noteAnim;

    public static Action<Tile> PlayerBehavior;
    #endregion

    #region 외부 호출 함수들

    public bool IsUiOn()
    {
        return isUIOn;
    }

    public bool IsDisturbanceOn()
    {
        return isDisturbanceInstall;
    }

    public bool CalculateDistanceToPlayer(Tile tile, int range)
    {
        var searchTiles = Hexamap.Map.GetTilesInRange(tile, range);

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
        if (IsUiOn())
            return currentUI.transform.parent.parent.GetComponent<TileInfo>().isTutorialTile;

        return false;
    }

    public void CurrentUIEmptying()
    {
        currentUI.SetActive(false);
        isUIOn = false;
        currentUI = null;
    }

    public void BaseActiveSet(bool isbool)
    {
        isBaseOn = isbool;

        if (isUIOn)
        {
            currentUI.SetActive(false);
            isUIOn = false;
        }
    }

    public DisturbanceMachine CalculateDistanceToDistrubtor(Tile tile, int range)
    {
        var searchTiles = Hexamap.Map.GetTilesInRange(tile, range);

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

        var zombie = Instantiate(zombiePrefab, spawnPos, Quaternion.Euler(0, -90, 0), zombiesTr);
        zombie.name = "Zombie " + 1;
        zombie.GetComponent<ZombieSwarm>().Init(tile);

        zombiesList.Add(zombie);

        zombie.GetComponent<ZombieSwarm>().MoveTargetCoroutine(playerLocationTile);
    }

    public Tile GetTileFromCoords(Coords coords)
    {
        return Hexamap.Map.GetTileFromCoords(coords);
    }

    public List<Tile> GetTilesInRange(Tile tile, int num)
    {
        return Hexamap.Map.GetTilesInRange(tile, num);
    }
    #endregion

    void Start()
    {
        StartCoroutine(GetAdditiveSceneObjects());
        GenerateMap();
        BaseActiveSet(true);

        health = maxHealth;
        hexamapTr.position = Vector3.forward * 200f;
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
        Hexamap.Destroy();

        var timeBefore = DateTime.Now;

        Hexamap.Generate();

        double timeSpent = (DateTime.Now - timeBefore).TotalSeconds;

        Hexamap.Draw();

        // Add some noise to Y position of tiles
        FastNoise _fastNoise = new FastNoise();
        _fastNoise.SetFrequency(0.1f);
        _fastNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
        _fastNoise.SetSeed(Hexamap.Map.Seed);
        foreach (Tile tile in Hexamap.Map.Tiles)
        {
            var noiseY = _fastNoise.GetValue(tile.Coords.X, tile.Coords.Y);
            (tile.GameEntity as GameObject).transform.position += new Vector3(0, noiseY * 2, 0);
        }

        // Output stats
        TextStats.text = $"Map generated in {timeSpent.ToString("0.000")} seconds.";
        //Debug.Log($"Seed : { Hexamap.Map.Seed }");

        GenerateMapObjects();
    }

    void Update()
    {
        if (!isPlayerMoving && !isBaseOn && !noteAnim.GetIsOpen())
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

        fog.transform.position = player.transform.position;
        FischlWorks_FogWar.csFogWar.instance.InitializeMapControllerObjects(player, 5);

        resourceManager.SetTile(playerLocationTile);
        StartCoroutine(DelaySightGetInfo());
        DeselectAllTile();
    }


    void SpawnPlayer()
    {
        playerLocationTile = Hexamap.Map.GetTileFromCoords(new Coords(0, 0));
        prevTile = playerLocationTile;
        targetTile = ((GameObject)playerLocationTile.GameEntity).GetComponent<TileController>();

        Vector3 spawnPos = ((GameObject)playerLocationTile.GameEntity).transform.position;
        spawnPos.y += 0.7f;

        player = Instantiate(playerPrefab, spawnPos, Quaternion.Euler(0, -90, 0));
        player.transform.parent = hexamapTr;
    }

    void SpawnZombies(int zombiesNumber)
    {
        int randomInt;
        var tileList = Hexamap.Map.Tiles;
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

            var zombie = Instantiate(zombiePrefab, spawnPos, Quaternion.Euler(0, -90, 0), zombiesTr);
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
        PlayerBehavior?.Invoke(playerLocationTile);
    }

    /// <summary>
    /// 카메라 시점을 InputKey를 통해 변경시켜주는 함수. 현재는 카메라가 변경되어 이 함수도 수정이 필요함. 사용하지 않음. 
    /// </summary>
    void CameraMoveInputKey()
    {
        // -- Keyboard movement
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        Vector3 movement = mapCamera.transform.position;

        if (vertical > 0) // Top
        {
            movement += Vector3.left * MoveSpeed * Time.deltaTime;

        }
        else if (vertical < 0) // Bottom
        {
            movement += Vector3.right * MoveSpeed * Time.deltaTime;

        }

        if (horizontal > 0) // Right
        {
            //

            movement += Vector3.forward * MoveSpeed * Time.deltaTime;
        }
        else if (horizontal < 0) // Left
        {
            //
            movement += Vector3.back * MoveSpeed * Time.deltaTime;

        }


        // -- Mouse scrolling
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");

        if (scrollWheel > 0 && movement.y > MinYPosition)
        {
            movement += Vector3.down * ScrollSpeed * Time.deltaTime;
        }
        else if (scrollWheel < 0 && movement.y < MaxYPosition)
        {
            movement += Vector3.up * ScrollSpeed * Time.deltaTime;
        }


        float relativeY = movement.y / (MaxYPosition - MinYPosition);
        float newXRot = (MaxXRotation - MinXRotation) * relativeY + MinXRotation;

        if (newXRot < MaxXRotation && newXRot > MinXRotation)
            mapCamera.transform.eulerAngles = new Vector3(newXRot, -90, 0);

        mapCamera.transform.position = movement;

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
                PathFinder(tileController, health);
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
                if (rangeOfMotion == health)
                    break;

                GameObject border = (GameObject)GetTileFromCoords(coords).GameEntity;
                border.GetComponent<Borders>().GetBorder()?.SetActive(true);
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
        if (Hexamap.Map.GetTilesInRange(playerLocationTile, 1).Contains(tileController.Model))
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
                && !isPlayerCanMove && !isDisturbanceSet && !isExplorerSet && health != 0)
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
                    if (Hexamap.Map.GetTilesInRange(playerLocationTile, 1).Contains(tileController.Model) 
                        && GetTileBorder(tileController, TileState.Moveable).activeInHierarchy)
                    {
                        foreach (var item in playerLocationTile.Neighbours.Where(item => item.Value == tileController.Model))
                        {
                            DistrubtorSettingSuccess(tileController, item.Key);
                        }
                    }
                }
                else if (isExplorerSet)
                {
                    if (GetTileBorder(tileController, TileState.Moveable).activeInHierarchy && playerLocationTile != tileController.Model)
                    {
                        ExplorerSettingSuccess(tileController);
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
                DistrubtorBorderActiveSet(false);
            }
            else if (isExplorerSet)
            {
                ExplorerBorderActiveSet(false);
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
            targetTile = Hexamap.Map.GetTileFromCoords(item);
            if (targetTile == null)
                break;

            targetPos = ((GameObject)targetTile.GameEntity).transform.position;
            targetPos.y += 0.5f
                ;
            player.transform.DOMove(targetPos, time);
            health--;
            yield return new WaitForSeconds(time);
        }

        lastTargetPos.y += 0.5f;
        yield return player.transform.DOMove(lastTargetPos, time);
        yield return new WaitForSeconds(time);

        movePath.Clear();
        health = 0;
        PlayerBehavior?.Invoke(playerLocationTile);
        resourceManager.GetResource();
        arrow.OffEffect();
    }

    void DistrubtorSettingSuccess(TileController tileController, CompassPoint direction)
    {
        disturbanceMachine.GetComponent<DisturbanceMachine>().Set(tileController.Model, direction);
        disturbanceMachine.GetComponent<DisturbanceMachine>().DirectionObjectOff();

        List<Tile> nearthTiles = GetTilesInRange(playerLocationTile, 1);

        for (int i = 0; i < nearthTiles.Count; i++)
        {
            ((GameObject)nearthTiles[i].GameEntity).GetComponent<Borders>().GetDisturbanceBorder()?.SetActive(false);
        }

        isPlayerSelected = false;
        isDisturbanceSet = false;
        isDisturbanceInstall = true;
    }

    void ExplorerSettingSuccess(TileController tileController)
    {
        explorerObject.GetComponent<Explorer>().Targetting(tileController.Model);
        explorerObject.GetComponent<Explorer>().Move();

        isPlayerSelected = false;
        isExplorerSet = false;

        DeselectAllPathTile();
    }

    #region UI 씬 버튼 관련
    public void DistrubtorBorderActiveSet(bool set)
    {
        List<Tile> nearthTiles;
        nearthTiles = GetTilesInRange(playerLocationTile, 1);

        if (set)
        {
            for (int i = 0; i < nearthTiles.Count; i++)
            {
                GameObject border = getTileBorder(nearthTiles[i], "BorderTarget");
                border?.SetActive(true);
            }

            disturbanceMachine = Instantiate(distrubtorPrefab, player.transform.position + Vector3.up * 1.5f, Quaternion.Euler(0, -90, 0));
            disturbanceMachine.transform.parent = hexamapTr;
            disturbanceMachine.GetComponentInChildren<MeshRenderer>().material.DOFade(50, 0);

            isDisturbanceSet = true;
            isPlayerSelected = true;
        }
        else
        {
            for (int i = 0; i < nearthTiles.Count; i++)
            {
                GameObject border = getTileBorder(nearthTiles[i], "BorderTarget");
                border?.SetActive(false);

                Destroy(disturbanceMachine);
                isDisturbanceSet = false;
                isPlayerSelected = false;
            }
        }
    }

    public void ExplorerBorderActiveSet(bool set)
    {
        if (set)
        {
            explorerObject = Instantiate(explorerPrefab, player.transform.position + Vector3.up * 1.5f, Quaternion.identity);
            explorerObject.transform.parent = hexamapTr;
            explorerObject.GetComponentInChildren<MeshRenderer>().material.DOFade(50, 0);
            explorerObject.GetComponent<Explorer>().Set(playerLocationTile);
            isExplorerSet = true;
            isPlayerSelected = true;
        }
        else
        {
            Destroy(explorerObject);
            isExplorerSet = false;
            isPlayerSelected = false;
        }
    }

    public void NextDay()
    {
        resourceManager.SetTile(targetTile.Model);
        prevTile = playerLocationTile;
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

            health = 0;
            PlayerBehavior?.Invoke(playerLocationTile);
            arrow.OffEffect();
            if (prevTile == playerLocationTile)
                resourceManager.GetResource();
        }

        if (health == maxHealth)
            prevTile = playerLocationTile;

        health = maxHealth;

        if (disturbanceMachine != null)
            disturbanceMachine.GetComponent<DisturbanceMachine>().Move();

        if (explorerObject != null)
            StartCoroutine(explorerObject.GetComponent<Explorer>().Move());

        foreach (var item in zombiesList)
        {
            item.GetComponent<ZombieSwarm>().Detection();
        }
    }

    public GameObject GetUi(TileController tile)
    {
        GameObject tileGO = (GameObject)tile.Model.GameEntity;

        if (tileGO != null && tile.Model.Landform.GetType().Name != "LandformWorldLimit")
            return tileGO.transform.Find("Canvas").Find("TileInfo").gameObject;

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
            BordersOff(t);
        }
        selectedTiles.Clear();
    }

    void DeselectAllPathTile()
    {
        if (selectPath == null)
            return;

        foreach (var t in selectPath)
        {
            BordersOff(Hexamap.Map.GetTileFromCoords(t));
        }
        selectPath.Clear();
    }

    void unselectTile(TileController tile)
    {
        BordersOff(tile);
        selectedTiles.Remove(tile);
    }

    void SelectBorder(TileController tileController, TileState state)
    {
        switch (state) 
        {
            case TileState.None:
                tileController.GetComponent<Borders>().GetBorder()?.SetActive(true);
                break;
            case TileState.Moveable:
                tileController.GetComponent<Borders>().GetAvailableBorder()?.SetActive(true);
                break;
            case TileState.Unable:
                tileController.GetComponent<Borders>().GetUnAvailableBorder()?.SetActive(true);
                break;
        }
        selectedTiles.Add(tileController);
    }

    void selectMetaLandform(TileController tile)
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

        tileToSelect.ForEach(t => SelectBorder(t, "BorderNo"));
        tileToUnselect.ForEach(t => unselectTile(t));
    }

    void BordersOff(TileController tile)
    {
        GameObject border1 = GetTileBorder(tile, "Border");
        GameObject border2 = GetTileBorder(tile, "BorderYes");
        GameObject border3 = GetTileBorder(tile, "BorderNo");
        //GameObject border4 = getTileBorder(tile, "BorderTarget");

        border1?.SetActive(false);
        border2?.SetActive(false);
        border3?.SetActive(false);
        //border4?.SetActive(false);
    }

    void BordersOff(Tile tile)
    {
        GameObject border1 = getTileBorder(tile, "Border");
        GameObject border2 = getTileBorder(tile, "BorderYes");
        GameObject border3 = getTileBorder(tile, "BorderNo");
        //GameObject border4 = getTileBorder(tile, "BorderTarget");

        border1?.SetActive(false);
        border2?.SetActive(false);
        border3?.SetActive(false);
        //border4?.SetActive(false);
    }

    GameObject GetTileBorder(TileController tileController, TileState state)
    {
        switch (state)
        {
            case TileState.None:
                return tileController.GetComponent<Borders>().GetBorder();
            case TileState.Moveable:
                return tileController.GetComponent<Borders>().GetAvailableBorder();
            case TileState.Unable:
                return tileController.GetComponent<Borders>().GetUnAvailableBorder();
        }

        return null;
    }

    GameObject getTileBorder(Tile tile, string name)
    {
        GameObject tileGO = (GameObject)tile.GameEntity;

        if (tileGO != null && tile.Landform.GetType().Name != "LandformWorldLimit")
            return tileGO.transform.Find(name).gameObject;

        return null;
    }

    public override EManagerType GetManagemetType()
    {
        throw new NotImplementedException();
    }
    #endregion
}
