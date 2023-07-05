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
    public Tile playerLocationTile;


    [Header("카메라 설정")]
    [Space(5f)]
    [SerializeField] int MaxYPosition = 70;
    [SerializeField] int MinYPosition = 5;
    [SerializeField] int MinXRotation = 30;
    [SerializeField] int MaxXRotation = 90;
    [SerializeField] int ScrollSpeed = 50;
    [SerializeField] int MoveSpeed = 50;

    [Header("게임 씬 관련")]
    [Space(5f)]
    public bool isBaseOn;

    List<TileController> selectedTiles = new List<TileController>();
    List<GameObject> disrubtorBorders = new List<GameObject>();
    List<GameObject> zombiesList = new List<GameObject>();
    List<Coords> selectPath;
    List<Coords> movePath;

    TMP_Text textHealth;
    [SerializeField] Camera mapCamera;
    public GameObject player;
    GameObject distrubtorObject;
    GameObject explorerObject;
    GameObject currentUI;

    int health;
    int maxHealth = 3;

    bool isSelected;
    bool isUIOn;
    bool isDisturbance;
    bool isExplorer;
    bool playerCanMove;
    #endregion

    void Start()
    {
        StartCoroutine(GetUISceneObjects());

        GenerateMap();
        BaseActiveSet(true);
        health = maxHealth;
    }

    IEnumerator GetUISceneObjects()
    {
        yield return new WaitForEndOfFrame();
        textHealth = GameObject.FindGameObjectWithTag("MapUi").transform.Find("Hp_Text").GetComponent<TMP_Text>();
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

    void Update()
    {
        if (textHealth != null)
            textHealth.text = "체력: " + health;

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

        // 맵 테스트 용
        CameraMoveInputKey();
        TileSelectWithRaycast();

        /*
        // 게임 씬 용
        if (!isPlayerMove && !isBaseOn)
            TileSelectWithRaycast();
        */
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

        DataManager.instance.gameData.TryGetValue("Data_MinCount_ZombieObject", out GameData min);
        DataManager.instance.gameData.TryGetValue("Data_MaxCount_ZombieObject", out GameData max);

        int rand = (int)UnityEngine.Random.Range(min.value, max.value);

        rand = 5;
        Debug.Log("좀비의 수: " + rand);

        SpawnPlayer();
        SpawnZombies(rand);
        unselectAllTile();
        FischlWorks_FogWar.csFogWar.instance.InitializeMapControllerObjects(player, 5);

        var mapCameraPos = mapCamera.transform.position;

        mapCamera.transform.position = new Vector3(player.transform.position.x, mapCameraPos.y, player.transform.position.z - 12f);
        mapCamera.transform.parent = player.transform;

        // 게임 씬 연결 시
        //hexamapTr.position += new Vector3(0, 0, 200);
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

        // 정가운데 좌표로 고정
        playerLocationTile = Hexamap.Map.GetTileFromCoords(new Coords(0, 0));

        Vector3 spawnPos = ((GameObject)playerLocationTile.GameEntity).transform.position;
        spawnPos.y += 0.5f;

        player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        player.transform.parent = hexamapTr;
    }

    void SpawnZombies(int zombiesNumber)
    {
        int randomInt;
        var tileList = Hexamap.Map.Tiles;
        List<int> selectTileNumber = new List<int>();

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

        for (int i = 0; i < selectTileNumber.Count; i++)
        {
            var tile = tileList[selectTileNumber[i]];

            var spawnPos = ((GameObject)tile.GameEntity).transform.position;
            spawnPos.y += 0.7f;

            var zombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity, zombiesTr);
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
            movement += Vector3.forward * MoveSpeed * Time.deltaTime;
        }
        else if (vertical < 0) // Bottom
        {
            movement += Vector3.back * MoveSpeed * Time.deltaTime;
        }

        if (horizontal > 0) // Right
        {
            movement += Vector3.right * MoveSpeed * Time.deltaTime;
        }
        else if (horizontal < 0) // Left
        {
            movement += Vector3.left * MoveSpeed * Time.deltaTime;
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
            mapCamera.transform.eulerAngles = new Vector3(newXRot, 0, 0);

        mapCamera.transform.position = movement;

    }

    void TileSelectWithRaycast()
    {
        // Mouse raytracing to select tiles
        RaycastHit hit;
        Ray ray = mapCamera.ScreenPointToRay(Input.mousePosition);
        int onlyLayerMaskPlayer = 1 << LayerMask.NameToLayer("Player");
        int onlyLayerMaskTile = 1 << LayerMask.NameToLayer("Tile");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile) && !isSelected)
        {
            Transform objectHit = hit.transform;
            TileController tile = objectHit.parent.GetComponent<TileController>();

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
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile) && playerCanMove)
        {
            Transform objectHit = hit.transform;
            TileController tile = objectHit.parent.GetComponent<TileController>();
            var hpCount = 0;

            unselectAllTile();
            unselectAllPathTile();

            if (tile.Model != playerLocationTile)
            {
                selectPath = AStar.FindPath(playerLocationTile.Coords, tile.Model.Coords);

                foreach (var item in selectPath)
                {
                    if (hpCount == health)
                        break;

                    Tile targetTile = Hexamap.Map.GetTileFromCoords(item);
                    var tileBorder = getTileBorder(targetTile, "Border");
                    tileBorder?.SetActive(true);
                    hpCount++;
                }
            }

            if (hpCount == health)
                selectTile(tile, "BorderNo");
            else
                selectTile(tile, "BorderYes");
        }
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile) && isDisturbance)
        {
            // 교란기 설치 위치 선택
            Transform objectHit = hit.transform;
            TileController tile = objectHit.parent.GetComponent<TileController>();

            if (Hexamap.Map.GetTilesInRange(playerLocationTile, 1).Contains(tile.Model))
            {
                unselectAllTile();
                DroneDirectionObejctOff(distrubtorObject);
                var borderDummy = getTileBorder(tile.Model, "BorderTarget");
                selectTile(tile, "BorderYes");
                distrubtorObject.transform.position = ((GameObject)tile.Model.GameEntity).transform.position + Vector3.up;
                foreach (var item in playerLocationTile.Neighbours)
                {
                    if (item.Value == tile.Model)
                    {
                        distrubtorObject.transform.GetChild(1).GetChild((int)item.Key).gameObject.SetActive(true);
                    }
                }
            }
        }
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile) && isExplorer)
        {
            // 교란기 설치 위치 선택
            Transform objectHit = hit.transform;
            TileController tile = objectHit.parent.GetComponent<TileController>();

            var hpCount = 0;

            unselectAllTile();
            unselectAllPathTile();

            if (tile.Model != playerLocationTile)
            {
                selectPath = AStar.FindPath(playerLocationTile.Coords, tile.Model.Coords);

                foreach (var item in selectPath)
                {
                    if (hpCount == 5)
                        break;

                    Tile targetTile = Hexamap.Map.GetTileFromCoords(item);
                    var tileBorder = getTileBorder(targetTile, "Border");
                    tileBorder?.SetActive(true);
                    hpCount++;
                }

                if (hpCount == 5)
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


        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray2 = mapCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskPlayer) && !playerCanMove && health != 0 && !isDisturbance && !isExplorer)
                {
                    isSelected = true;
                    playerCanMove = true;
                }
                else if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile) && !isSelected)
                {
                    Transform objectHit = hit.transform;
                    TileController tile = objectHit.parent.GetComponent<TileController>();

                    currentUI = GetUi(tile);
                    currentUI.transform.position = mapCamera.WorldToScreenPoint(objectHit.position);

                    currentUI.transform.position += new Vector3(300, 150, 0);

                    currentUI.SetActive(true);

                    isUIOn = true;
                }
                else if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile) && playerCanMove)
                {
                    Transform objectHit = hit.transform;
                    TileController tile = objectHit.parent.GetComponent<TileController>();
                    if (getTileBorder(tile, "BorderYes").activeInHierarchy)
                    {
                        movePath = AStar.FindPath(playerLocationTile.Coords, tile.Model.Coords);
                        // 플레이어 선택한 칸으로 이동
                        StartCoroutine(PlayerMove(tile.transform.position));
                        playerLocationTile = tile.Model;
                    }
                    else
                    {
                        return;
                    }
                }
                else if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile) && isDisturbance)
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
                else if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile) && isExplorer)
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
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (playerCanMove)
            {
                unselectAllTile();
                unselectAllPathTile();
                isSelected = false;
                playerCanMove = false;
            }
        }
    }

    public void DistrubtorBorderActiveOn()
    {
        List<Tile> nearthTiles;
        nearthTiles = GetTilesInRange(playerLocationTile, 1);

        for (int i = 0; i < nearthTiles.Count; i++)
        {
            GameObject border = getTileBorder(nearthTiles[i], "BorderTarget");
            border?.SetActive(true);
        }

        distrubtorObject = Instantiate(distrubtorPrefab, player.transform.position + Vector3.up * 1.5f, Quaternion.identity);
        distrubtorObject.transform.parent = hexamapTr;
        distrubtorObject.GetComponentInChildren<MeshRenderer>().material.DOFade(50, 0);
        isDisturbance = true;
        isSelected = true;
    }


    public void ExplorerBorderActiveOn()
    {
        explorerObject = Instantiate(explorerPrefab, player.transform.position + Vector3.up * 1.5f, Quaternion.identity);
        explorerObject.transform.parent = hexamapTr;
        explorerObject.GetComponentInChildren<MeshRenderer>().material.DOFade(50, 0);
        explorerObject.GetComponent<Explorer>().Set(playerLocationTile);
        isExplorer = true;
        isSelected = true;
    }

    IEnumerator PlayerMove(Vector3 lastTargetPos, float time = 0.5f)
    {
        Tile targetTile;
        Vector3 targetPos;

        foreach (var item in movePath)
        {
            targetTile = Hexamap.Map.GetTileFromCoords(item);
            if (targetTile == null)
                break;

            targetPos = ((GameObject)targetTile.GameEntity).transform.position;
            targetPos.y += 0.5f;
            player.transform.DOMove(targetPos, time);
            health--;
            textHealth.text = "체력: " + health;
            yield return new WaitForSeconds(time);
        }
        lastTargetPos.y += 0.5f;
        yield return player.transform.DOMove(lastTargetPos, time);
        textHealth.text = "체력: " + health;

        isSelected = false;
        playerCanMove = false;
        unselectAllPathTile();
        movePath.Clear();
        health = 0;
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

    public void NextDay()
    {
        health = maxHealth;
        CheckSumZombies();

        if (distrubtorObject != null)
            distrubtorObject.GetComponent<Distrubtor>().Move();

        if (explorerObject != null)
            StartCoroutine(explorerObject.GetComponent<Explorer>().Move());

        foreach (var item in zombiesList)
        {
            item.GetComponent<ZombieSwarm>().Detection();
        }
    }

    #region 선택 타일 관련

    private void unselectAllTile()
    {
        foreach (var t in selectedTiles)
        {
            if (t == null)
                continue;
            BordersOff(t);
        }
        selectedTiles.Clear();
    }

    private void unselectAllPathTile()
    {
        if (selectPath == null)
            return;

        foreach (var t in selectPath)
        {
            BordersOff(Hexamap.Map.GetTileFromCoords(t));
        }
        selectPath.Clear();
    }

    private void unselectTile(TileController tile)
    {
        BordersOff(tile);
        selectedTiles.Remove(tile);
    }

    private void selectMetaLandform(TileController tile)
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

    private void selectTile(TileController tile, string name)
    {
        GameObject border = getTileBorder(tile.Model, name);

        border.SetActive(true);
        selectedTiles.Add(tile);
    }

    private GameObject getTileBorder(TileController tile, string name)
    {
        GameObject tileGO = (GameObject)tile.Model.GameEntity;

        if (tileGO != null && tile.Model.Landform.GetType().Name != "LandformWorldLimit")
            return tileGO.transform.Find(name).gameObject;

        return null;
    }

    private GameObject getTileBorder(Tile tile, string name)
    {
        GameObject tileGO = (GameObject)tile.GameEntity;

        if (tileGO != null && tile.Landform.GetType().Name != "LandformWorldLimit")
            return tileGO.transform.Find(name).gameObject;

        return null;
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

    private void BordersOff(TileController tile)
    {
        GameObject border1 = getTileBorder(tile, "Border");
        GameObject border2 = getTileBorder(tile, "BorderYes");
        GameObject border3 = getTileBorder(tile, "BorderNo");

        border1?.SetActive(false);
        border2?.SetActive(false);
        border3?.SetActive(false);
    }

    private void BordersOff(Tile tile)
    {
        GameObject border1 = getTileBorder(tile, "Border");
        GameObject border2 = getTileBorder(tile, "BorderYes");
        GameObject border3 = getTileBorder(tile, "BorderNo");
        GameObject border4 = getTileBorder(tile, "BorderTarget");

        border1?.SetActive(false);
        border2?.SetActive(false);
        border3?.SetActive(false);
        border4?.SetActive(false);
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

    void DistrubtorSettingSuccess()
    {
        DroneDirectionObejctOff(distrubtorObject);

        List<Tile> nearthTiles = GetTilesInRange(playerLocationTile, 1);

        for (int i = 0; i < nearthTiles.Count; i++)
        {
            GameObject border = getTileBorder(nearthTiles[i], "BorderTarget");
            border?.SetActive(false);
        }

        isSelected = false;
        isDisturbance = false;
    }

    void ExplorerSettingSuccess()
    {
        isSelected = false;
        isDisturbance = false;
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
}
