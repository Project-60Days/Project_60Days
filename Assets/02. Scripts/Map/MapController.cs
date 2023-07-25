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

public class MapController : Singleton<MapController>
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
    GameObject distrubtorObject;
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
        if (distrubtorObject != null)
            return true;
        else
            return false;
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

    public Distrubtor CalculateDistanceToDistrubtor(Tile tile, int range)
    {
        var searchTiles = Hexamap.Map.GetTilesInRange(tile, range);

        if (distrubtorObject == null)
            return null;

        foreach (var item in searchTiles)
        {
            if (distrubtorObject.GetComponent<Distrubtor>().curTile == item)
            {
                return distrubtorObject.GetComponent<Distrubtor>();
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
        List<Tile> result = new List<Tile>();

        var tileList1 = GetTilesInRange(playerLocationTile, 5);
        var tileList2 = GetTilesInRange(playerLocationTile, 4);

        result.AddRange(tileList1);
        result.AddRange(tileList2);
        result = result.Distinct().ToList();

        int randomInt = UnityEngine.Random.Range(0, result.Count);

        var tile = result[randomInt];
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

    void Update()
    {
        /*        if (textHealth != null)
                    textHealth.text = "체력: " + health;*/

        if (Input.GetKeyDown(KeyCode.R))
        {
            Destroy(player);

            foreach (var item in zombiesList)
            {
                Destroy(item.gameObject);
            }

            zombiesList.Clear();
            GenerateMap();
        }

        if (!isPlayerMoving && !isBaseOn && !noteAnim.GetIsOpen())
        {
            //CameraMoveInputKey();
            MouseOverTile();
        }
    }

    IEnumerator GetAdditiveSceneObjects()
    {
        yield return new WaitForEndOfFrame();
        mapCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        noteAnim = GameObject.FindGameObjectWithTag("NoteAnim").GetComponent<NoteAnim>();
        //textHealth = GameObject.FindGameObjectWithTag("MapUi").transform.GetChild(0).transform.Find("Hp_Text").GetComponent<TMP_Text>();
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

        // 맵 오브젝트 소환 관련

        DataManager.instance.gameData.TryGetValue("Data_MinCount_ZombieObject", out GameData min);
        DataManager.instance.gameData.TryGetValue("Data_MaxCount_ZombieObject", out GameData max);

        int rand = (int)UnityEngine.Random.Range(min.value, max.value);

        // 좀비 등장 수 고정
        // rand = 5;

        SpawnPlayer();
        //SpawnZombies(rand);
        fog.transform.position = player.transform.position;
        unselectAllTile();
        FischlWorks_FogWar.csFogWar.instance.InitializeMapControllerObjects(player, 5);
        resourceManager.SetTile(playerLocationTile);
        StartCoroutine(DelaySightGetInfo());
    }

    IEnumerator DelaySightGetInfo()
    {
        yield return new WaitForEndOfFrame();
        PlayerBehavior?.Invoke(playerLocationTile);
    }

    void SpawnPlayer()
    {
        // 랜덤 위치

        /*
        int randomInt;
        var tileList = Hexamap.Map.Tiles;

        while (true)
        {
            randomInt = UnityEngine.Random.Range(0, tileList.Count);

            if (tileList[randomInt].Landform.GetType().Name == "LandformPlain")
            {
                break;
            }
        }

        playerLocationTile = tileList[randomInt];
        */

        // 정가운데 좌표 고정
        playerLocationTile = Hexamap.Map.GetTileFromCoords(new Coords(0, 0));
        prevTile = playerLocationTile;
        targetTile = ((GameObject)playerLocationTile.GameEntity).GetComponent<TileController>();

        Vector3 spawnPos = ((GameObject)playerLocationTile.GameEntity).transform.position;
        spawnPos.y += 0.5f;

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

    void MouseOverTile()
    {
        // Mouse raytracing to select tiles
        RaycastHit hit;
        Transform objectHit;
        TileController tile;

        Ray ray = mapCamera.ScreenPointToRay(Input.mousePosition);
        int onlyLayerMaskTile = 1 << LayerMask.NameToLayer("Tile");

        // 아무 상태도 아닐 때
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile) && !isPlayerSelected)
        {
            objectHit = hit.transform;
            tile = objectHit.parent.GetComponent<TileController>();

            if (tile != null && !selectedTiles.Contains(tile))
            {
                unselectAllTile();
                selectTile(tile, "BorderNo");

                if (isUIOn)
                {
                    currentUI.SetActive(false);
                    isUIOn = false;
                }
            }
        }
        // 플레이어를 선택했을 때
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile) && isPlayerCanMove)
        {
            objectHit = hit.transform;
            tile = objectHit.parent.GetComponent<TileController>();
            var rangeOfMotion = 0;

            unselectAllTile();
            unselectAllPathTile();

            if (tile.Model != playerLocationTile)
            {
                selectPath = AStar.FindPath(playerLocationTile.Coords, tile.Model.Coords);

                foreach (var item in selectPath)
                {
                    if (rangeOfMotion == health)
                        break;

                    Tile targetTile = Hexamap.Map.GetTileFromCoords(item);
                    var tileBorder = getTileBorder(targetTile, "Border");
                    tileBorder?.SetActive(true);
                    rangeOfMotion++;
                }
            }

            if (rangeOfMotion == health)
                selectTile(tile, "BorderNo");
            else
                selectTile(tile, "BorderYes");
        }
        // 교란기 선택 시
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile) && isDisturbanceSet)
        {
            objectHit = hit.transform;
            tile = objectHit.parent.GetComponent<TileController>();

            if (Hexamap.Map.GetTilesInRange(playerLocationTile, 1).Contains(tile.Model))
            {
                unselectAllTile();
                DroneDirectionObejctOff(distrubtorObject);
                selectTile(tile, "BorderYes");
                distrubtorObject.transform.position = ((GameObject)tile.Model.GameEntity).transform.position + Vector3.up;

                foreach (var item in playerLocationTile.Neighbours)
                {
                    if (item.Value == tile.Model)
                        distrubtorObject.transform.GetChild(1).GetChild((int)item.Key).gameObject.SetActive(true);
                }
            }
        }
        // 탐색기 선택 시
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile) && isExplorerSet)
        {
            objectHit = hit.transform;
            tile = objectHit.parent.GetComponent<TileController>();
            var rangeOfMotion = 0;

            unselectAllTile();
            unselectAllPathTile();

            if (tile.Model != playerLocationTile)
            {
                selectPath = AStar.FindPath(playerLocationTile.Coords, tile.Model.Coords);

                foreach (var item in selectPath)
                {
                    if (rangeOfMotion == 5)
                        break;

                    Tile targetTile = Hexamap.Map.GetTileFromCoords(item);
                    var tileBorder = getTileBorder(targetTile, "Border");
                    tileBorder?.SetActive(true);
                    rangeOfMotion++;
                }

                if (rangeOfMotion == 5)
                    selectTile(tile, "BorderNo");
                else
                    selectTile(tile, "BorderYes");
            }
        }
        else
        {
            if (isUIOn)
            {
                currentUI.SetActive(false);
                isUIOn = false;
            }
            unselectAllTile();
            unselectAllPathTile();
        }

        MouseClickTile();
    }

    void DroneDirectionObejctOff(GameObject ddo)
    {
        var ddoArr = ddo.transform.GetChild(1).GetComponentsInChildren<Transform>();

        for (int i = 1; i < ddoArr.Length; i++)
        {
            if (ddoArr[i].gameObject.activeInHierarchy)
                ddoArr[i].gameObject.SetActive(false);
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
            // UI 너머로 타일 클릭 방지

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskPlayer) && !isPlayerCanMove && health != 0 && !isDisturbanceSet && !isExplorerSet)
            {
                isPlayerSelected = true;
                isPlayerCanMove = true;
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile) && !isPlayerSelected)
            {
                Transform objectHit = hit.transform;
                TileController tile = objectHit.parent.GetComponent<TileController>();

                currentUI = GetUi(tile);
                currentUI.SetActive(true);
                isUIOn = true;
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile) && isPlayerCanMove)
            {
                Transform objectHit = hit.transform;
                TileController tile = objectHit.parent.GetComponent<TileController>();
                if (getTileBorder(tile, "BorderYes").activeInHierarchy && playerLocationTile != tile.Model)
                {
                    // 선택한 칸으로 이동
                    movePath = AStar.FindPath(playerLocationTile.Coords, tile.Model.Coords);

                    isPlayerSelected = false;
                    isPlayerCanMove = false;
                    targetTile = tile;

                    unselectAllTile();
                    unselectAllPathTile();

                    arrow.OnEffect(tile.transform);
                }
                else
                {
                    return;
                }
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile) && isDisturbanceSet)
            {
                Transform objectHit = hit.transform;
                TileController tile = objectHit.parent.GetComponent<TileController>();
                if (Hexamap.Map.GetTilesInRange(playerLocationTile, 1).Contains(tile.Model) && getTileBorder(tile, "BorderYes").activeInHierarchy)
                {
                    // 교란기 설치
                    foreach (var item in playerLocationTile.Neighbours)
                    {
                        if (item.Value == tile.Model)
                        {
                            distrubtorObject.GetComponent<Distrubtor>().Set(tile.Model, item.Key);
                            DistrubtorSettingSuccess();
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile) && isExplorerSet)
            {
                Transform objectHit = hit.transform;
                TileController tile = objectHit.parent.GetComponent<TileController>();
                if (getTileBorder(tile, "BorderYes").activeInHierarchy && playerLocationTile != tile.Model)
                {
                    explorerObject.GetComponent<Explorer>().Targetting(tile.Model);
                    explorerObject.GetComponent<Explorer>().Move();
                    ExplorerSettingSuccess();
                    unselectAllPathTile();
                }
                else
                {
                    return;
                }
            }

        }

        // 우클릭 시 선택 취소
        if (Input.GetMouseButtonDown(1))
        {
            if (isPlayerCanMove)
            {
                unselectAllTile();
                unselectAllPathTile();
                isPlayerSelected = false;
                isPlayerCanMove = false;
            }

            if (isDisturbanceSet)
            {
                unselectAllTile();
                unselectAllPathTile();
                DistrubtorBorderActiveSet(false);
            }

            if (isExplorerSet)
            {
                unselectAllTile();
                unselectAllPathTile();
                ExplorerBorderActiveSet(false);
            }
        }
    }

    IEnumerator MovePlayer(Vector3 lastTargetPos, float time = 0.4f)
    {
        isPlayerMoving = true;
        unselectAllTile();
        unselectAllPathTile();

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
            //textHealth.text = "체력: " + health;
            yield return new WaitForSeconds(time);
        }

        lastTargetPos.y += 0.5f;
        yield return player.transform.DOMove(lastTargetPos, time);
        yield return new WaitForSeconds(time);

        movePath.Clear();
        health = 0;
        //isPlayerMoving = false;
        PlayerBehavior?.Invoke(playerLocationTile);
        resourceManager.GetResource();
        arrow.OffEffect();
    }

    void DistrubtorSettingSuccess()
    {
        DroneDirectionObejctOff(distrubtorObject);

        List<Tile> nearthTiles = GetTilesInRange(playerLocationTile, 1);

        for (int i = 0; i < nearthTiles.Count; i++)
        {
            GameObject border = getTileBorder(nearthTiles[i], "BorderTarget");
            border?.SetActive(false);
        }

        isPlayerSelected = false;
        isDisturbanceSet = false;
    }

    void ExplorerSettingSuccess()
    {
        isPlayerSelected = false;
        isExplorerSet = false;
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

            distrubtorObject = Instantiate(distrubtorPrefab, player.transform.position + Vector3.up * 1.5f, Quaternion.Euler(0, -90, 0));
            distrubtorObject.transform.parent = hexamapTr;
            distrubtorObject.GetComponentInChildren<MeshRenderer>().material.DOFade(50, 0);

            isDisturbanceSet = true;
            isPlayerSelected = true;
        }
        else
        {
            for (int i = 0; i < nearthTiles.Count; i++)
            {
                GameObject border = getTileBorder(nearthTiles[i], "BorderTarget");
                border?.SetActive(false);

                Destroy(distrubtorObject);
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
            unselectAllTile();
            unselectAllPathTile();

            health = 0;
            PlayerBehavior?.Invoke(playerLocationTile);
            arrow.OffEffect();
            if (prevTile == playerLocationTile)
                resourceManager.GetResource();
        }

        if (health == maxHealth)
            prevTile = playerLocationTile;

        health = maxHealth;

        if (distrubtorObject != null)
            distrubtorObject.GetComponent<Distrubtor>().Move();

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
    void unselectAllTile()
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

    void unselectAllPathTile()
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

    void selectTile(TileController tile, string name)
    {
        GameObject border = getTileBorder(tile.Model, name);

        border.SetActive(true);
        selectedTiles.Add(tile);
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

        tileToSelect.ForEach(t => selectTile(t, "BorderNo"));
        tileToUnselect.ForEach(t => unselectTile(t));
    }

    void BordersOff(TileController tile)
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

    GameObject getTileBorder(TileController tile, string name)
    {
        GameObject tileGO = (GameObject)tile.Model.GameEntity;

        if (tileGO != null && tile.Model.Landform.GetType().Name != "LandformWorldLimit")
            return tileGO.transform.Find(name).gameObject;

        return null;
    }

    GameObject getTileBorder(Tile tile, string name)
    {
        GameObject tileGO = (GameObject)tile.GameEntity;

        if (tileGO != null && tile.Landform.GetType().Name != "LandformWorldLimit")
            return tileGO.transform.Find(name).gameObject;

        return null;
    }
    #endregion
}
