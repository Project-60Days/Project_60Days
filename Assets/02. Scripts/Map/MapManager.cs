using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Hexamap;

public class MapManager : Manager
{
    public MapController mapCtrl;
    public EnemyCtrl enemyCtrl;
    public ResourceCtrl resourceCtrl;
    public ArrowCtrl arrowCtrl;
    public bool mouseIntreractable;

    [SerializeField] ETileMouseState mouseState;
    public MapCamCtrl cameraCtrl;

    Camera mainCamera;
    TileController curTileController;

    bool canPlayerMove;
    bool isDronePrepared;
    bool isDisturbtorPrepared;
    bool isCameraMove;
    bool isTundraTile;

    private TileController cameraTarget;

    public MapData data { get; private set; }

    private void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        data = App.Manager.Test.mapData;

        arrowCtrl.Init();
        mapCtrl.Init();
        cameraCtrl.Init();
        InitValue();
    }

    void Update()
    {
        SetETileMoveState();

        if (mouseState != ETileMouseState.Nothing)
        {
            if (isCameraMove)
                GetCameraCenterTile();

            MouseOverEvents();
        }
    }

    void MouseOverEvents()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            mapCtrl.DeselectAllBorderTiles();
            return;
        }

        RaycastHit hit;
        TileController tileController;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        int onlyLayerMaskTile = 1 << LayerMask.NameToLayer("Tile");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile))
        {
            tileController = hit.transform.parent.GetComponent<TileController>();

            mapCtrl.DeselectAllBorderTiles();

            if (!mapCtrl.CheckPlayersView(tileController))
            {
                App.Manager.UI.GetPanel<MapPanel>().TileInfo(false);
                return;
            }
            
            switch (mouseState)
            {
                case ETileMouseState.CanClick:
                    mapCtrl.DefaultMouseOverState(tileController);

                    if (tileController != curTileController)
                        App.Manager.UI.GetPanel<MapPanel>().TileInfo(false);
                    break;

                case ETileMouseState.CanPlayerMove:
                    mapCtrl.TilePathFinderSurroundings(tileController);
                    mapCtrl.AddSelectedTilesList(tileController);
                    break;

                case ETileMouseState.DronePrepared:
                    if (isDisturbtorPrepared)
                    {
                        mapCtrl.DisturbtorPathFinder(tileController);
                    }
                    else
                    {
                        mapCtrl.ExplorerPathFinder(tileController, 5);
                    }

                    break;
            }

            curTileController = tileController;
        }
        else
        {
            mapCtrl.DeselectAllBorderTiles();
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
                    canPlayerMove = mapCtrl.CheckPlayerCanMove();
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
                    if (mapCtrl.SelectPlayerMovePoint(tileController))
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
                        mapCtrl.SelectTileForDisturbtor(tileController);
                    }
                    else
                    {
                        mapCtrl.SelectTileForExplorer(tileController);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            mapCtrl.DeselectAllBorderTiles();

            if (canPlayerMove)
            {
                canPlayerMove = false;
            }

            // 목적지 정한 이후 취소 가능

            if (isDronePrepared)
            {
                if (isDisturbtorPrepared)
                {
                    mapCtrl.PreparingDistrubtor(false);
                }
                else
                {
                    mapCtrl.PreparingExplorer(false);
                }
            }

            MovePathDelete();
        }

        if (Input.GetMouseButton(2))
        {
            isCameraMove = true;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            isCameraMove = false;
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

    public IEnumerator NextDay()
    {
        yield return StartCoroutine(mapCtrl.NextDay());
        resourceCtrl.GetResource(mapCtrl.Player.TileController);
        arrowCtrl.ReInit();
        
        CheckRoutine();
    }

    public bool CheckCanInstallDrone()
    {
        if (mouseState == ETileMouseState.CanClick)
        {
            return true;
        }

        return false;
    }

    public void InitValue()
    {
        mouseIntreractable = true;
        canPlayerMove = false;
        isDronePrepared = false;
        isDisturbtorPrepared = false;
    }

    public void CheckRoutine()
    {
        CheckZombies();
        CheckStructureNeighbor();
    
        InitValue();
    }

    public void CheckZombies()
    {
        bool isActive;

        if (enemyCtrl.CheckZombies())
            isActive = true;
        else
            isActive = false;

        App.Manager.UI.GetPanel<FixedPanel>().SetAlert(AlertType.Caution, isActive);
    }

    /// <summary>
    /// 현재 타일이 구조물 인접타일인지 확인
    /// </summary>
    public void CheckStructureNeighbor()
    {
        var structure = mapCtrl.SensingStructure();
        if (structure != null)
        {
            if (structure is Tower)
                if (App.Manager.UI.GetPanel<InventoryPanel>().CheckNetCardUsage() == false) return;

            if (structure.isUse == false)
                App.Manager.UI.GetPanel<PagePanel>().SetSelectPage("structureSelect", structure);
        }
    }

    public string GetLandformBGM(TileBase _tile) => _tile.tileData.English switch
    {
        "None" => "Ambience_City",
        "Jungle" => "Ambience_Jungle",
        "Desert" => "Ambience_Desert",
        "Tundra" => "Ambience_Tundra",
        _ => "Ambience_City",
    };

    public void NormalStructureResearch(StructureBase structure)
    {
        int randomNumber = UnityEngine.Random.Range(1, 4);

        if (randomNumber == 3)
            enemyCtrl.SpawnStructureZombies(structure.colleagues);

        // 플레이어 체력 0으로 만들어서 경로 선택 막기
        if (isTundraTile)
        {
            App.Manager.UI.GetPanel<PagePanel>().SetResultPage("SEARCH_TUNDRA", false);
            mapCtrl.Player.SetHealth(false);
        }

        // 경로 삭제
        MovePathDelete();

        structure.structureModel.GetComponent<StructureFade>().FadeIn();
        structure.colleagues.ForEach(tile => tile.ResourceUpdate(true));

        mapCtrl.SpawnSpecialItemRandomTile(structure.colleagues);
    }

    public void MovePathDelete()
    {
        if (mapCtrl.IsMovePathSaved() == false)
            return;

        arrowCtrl.ArrowOff();
        mapCtrl.DeletePlayerMovePath();
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
                mapCtrl.OcclusionCheck(cameraTarget.Model);
            }
        }
    }
    
    public void TundraTileCheck()
    {
        isTundraTile = true;
    }

    public void EtherResourceCheck()
    {
        var resources = resourceCtrl.GetLastResources();

        if (resources.Count == 0 || resources == null)
            return;
        
        if(resources.Find(x=> x.Item.Code == "ITEM_GAS") != null)
        {
            Debug.Log("에테르 디버프");
            App.Manager.UI.GetPanel<PagePanel>().SetResultPage("ACIDENT_ETHER", false);
            mapCtrl.Player.SetHealth(false);
        }
        else
        {
            return;
        }
    }

    public bool IsJungleTile(TileController _tileController)
        => _tileController.GetComponent<TileBase>().TileType == ETileType.Jungle;

    public void SetIsDronePrepared(bool _isDronePrepared, string type)
    {
        isDronePrepared = _isDronePrepared;
        
        if(type == "Distrubtor")
            isDisturbtorPrepared = true;
        else
            isDisturbtorPrepared = false;
    }
}