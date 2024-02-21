using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Hexamap;

public class MapManager : ManagementBase
{
    [SerializeField] private ETileMouseState mouseState;

    public MapUiController UIController => uiController;
    public MapController Controller => mapController;
    public ResourceManager ResourceManager => resourceManager;

    private MapUiController uiController;
    private MapController mapController;
    private ResourceManager resourceManager;
    private Camera mainCamera;
    private MapCamera mapCineCamera;
    private TileInitInfo curTileInfo;
    private TileInitInfo cameraTarget;
    private TileBase structureTileBase;
    private StructureBase curStructure;

    private bool canInteractableMouse;
    private bool canMovePlayer;
    private bool preparesDrone;
    private bool isDisturbtor;
    private bool isMovingCamera;
    private bool isTundraTile;

    [Header("밸런스 테스트 용")] [Space(5f)] [SerializeField]
    private MapData mapData;

    public void StartMapManager()
    {
        StartCoroutine(GetAdditiveSceneObjects());
    }

    private void Update()
    {
        CheckMouseState();

        if (mouseState != ETileMouseState.Nothing)
        {
            if (isMovingCamera)
            {
                GetCameraCenterTile();
            }

            MouseOverEvents();
        }
    }

    /// <summary>
    /// 다른 Additive Scene에서 필요한 오브젝트들을 가져오는 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetAdditiveSceneObjects()
    {
        yield return new WaitForEndOfFrame();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        uiController = GameObject.FindGameObjectWithTag("MapUi").GetComponent<MapUiController>();
        mapController = GameObject.FindGameObjectWithTag("MapController").GetComponent<MapController>();

        mapController.InputMapData(mapData);

        yield return new WaitUntil(() => mapController != null);
        mapController.StartMapController();
        mapController.SightCheckInit();

        resourceManager = GameObject.FindGameObjectWithTag("Resource").GetComponent<ResourceManager>();
        mapCineCamera = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<MapCamera>();

        mapCineCamera.StartGetMapInfo();
        AllowMouseEvent(true);
    }

    private void MouseOverEvents()
    {
        // UI 위에 마우스가 있을 때 타일 선택 없애고 함수 종료
        if (EventSystem.current.IsPointerOverGameObject())
        {
            mapController.DeselectAllBorderTiles();
            return;
        }

        // 타일 레이어 마스크
        int layerMaskTile = 1 << LayerMask.NameToLayer("Tile");

        // 마우스 위치에 Ray를 쏴 타일 위에 올라가 있는지 확인
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, layerMaskTile))
        {
            mapController.DeselectAllBorderTiles();

            if (hit.transform.parent.TryGetComponent(out TileInitInfo _tileInfo))
            {
                CheckTile(_tileInfo);
                MouseClickEvents(_tileInfo);
            }
        }
        else
        {
            mapController.DeselectAllBorderTiles();
            uiController.TileInfoSwitch(false);
        }
    }

    /// <summary>
    /// 타일을 체크하고 마우스 상태에 따라 다른 동작을 수행하는 함수
    /// </summary>
    /// <param name="_tileInfo"></param>
    private void CheckTile(TileInitInfo _tileInfo)
    {
        // 플레이어 시야에서 벗어나면 타일 정보 UI를 끔
        if (mapController.CheckPlayersView(_tileInfo) == false)
        {
            uiController.TileInfoSwitch(false);
            return;
        }

        switch (mouseState)
        {
            case ETileMouseState.CanClickTile:
                mapController.DefaultMouseState(_tileInfo);
                if (_tileInfo != curTileInfo)
                {
                    uiController.TileInfoSwitch(false);
                }

                break;

            case ETileMouseState.CanPlayerMove:
                mapController.PlayerPathFinder(_tileInfo);
                mapController.AddTileList(_tileInfo);
                break;

            case ETileMouseState.DronePrepared:
                if (isDisturbtor)
                {
                    mapController.DisturbtorPathFinder(_tileInfo);
                }
                else
                {
                    mapController.ExplorerPathFinder(_tileInfo);
                }

                break;

            default:
                break;
        }

        curTileInfo = _tileInfo;
    }

    private void MouseClickEvents(TileInitInfo _tileInfo)
    {
        int onlyLayerMaskPlayer = 1 << LayerMask.NameToLayer("Player");

        if (Input.GetMouseButtonDown(0))
        {
            // 플레이어를 클릭한 경우
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, onlyLayerMaskPlayer))
            {
                if (preparesDrone == false)
                {
                    canMovePlayer = mapController.PlayerCanMoveCheck();
                }
            }

            if (mouseState == ETileMouseState.CanClickTile)
            {
                _tileInfo.TileBase.TileInfoUpdate();
                uiController.TileInfoSwitch(true);
            }
            else if (canMovePlayer)
            {
                if (mapController.SelectPlayerMovePoint(_tileInfo))
                {
                    // 정확한 위치로 수정 필요 등장 위치 애매함
                    uiController.OnPlayerMovePoint(_tileInfo.transform);
                    canMovePlayer = false;
                }
            }
            else if (preparesDrone)
            {
                if (isDisturbtor)
                {
                    mapController.SelectTileForDisturbtor(_tileInfo);
                }
                else
                {
                    mapController.SelectTileForExplorer(_tileInfo);
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            mapController.DeselectAllBorderTiles();

            if (canMovePlayer)
            {
                canMovePlayer = false;
            }

            // 목적지 정한 이후 취소 가능

            if (preparesDrone)
            {
                if (isDisturbtor)
                {
                    mapController.PreparingDistrubtor(false);
                }
                else
                {
                    mapController.PreparingExplorer(false);
                }
            }

            MoveCancel();
        }
        else if (Input.GetMouseButton(2))
        {
            isMovingCamera = true;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            isMovingCamera = false;
        }
    }

    private void CheckMouseState()
    {
        if (canInteractableMouse == false)
        {
            mouseState = ETileMouseState.Nothing;
        }
        else if (canMovePlayer == false && !preparesDrone == false)
        {
            mouseState = ETileMouseState.CanClickTile;
        }
        else if (canMovePlayer)
        {
            mouseState = ETileMouseState.CanPlayerMove;
        }
        else if (preparesDrone)
        {
            mouseState = ETileMouseState.DronePrepared;
        }
    }

    public IEnumerator NextDayCoroutine()
    {
        yield return StartCoroutine(mapController.NextDay());
        resourceManager.GetResource(mapController.Player.TileInitInfo);
        uiController.OffPlayerMovePoint();

        CheckRoutine();
    }

    public bool CheckCanInstallDrone()
    {
        if (mouseState == ETileMouseState.CanClickTile)
        {
            return true;
        }

        return false;
    }

    public void AllowMouseEvent(bool isAllow)
    {
        canInteractableMouse = isAllow;
        canMovePlayer = false;
        preparesDrone = false;
        isDisturbtor = false;
    }

    public override EManagerType GetManagemetType()
    {
        return EManagerType.MAP;
    }

    public void SetMapCameraPriority(bool _set)
    {
        mapCineCamera.SetPrioryty(_set);
    }

    public void CheckRoutine()
    {
        CheckZombies();
        CheckStructureNeighbor();

        // 튜토리얼 네트워크 칩
        // if(isVisitNoneTile == false)
        // {
        //     TutorialTileCheck();
        // }

        AllowMouseEvent(true);
    }

    public void CheckZombies()
    {
        if (mapController.CheckZombies())
        {
            UIManager.instance.GetAlertController().SetAlert("caution", true);
        }
        else
            return;
    }

    /// <summary>
    /// 현재 타일이 구조물 인접타일인지 확인
    /// </summary>
    public void CheckStructureNeighbor()
    {
        var structure = mapController.SensingStructure();
        if (structure != null)
        {
            if (structure is Tower)
                if (UIManager.instance.GetInventoryController().CheckNetCardUsage() == false)
                    return;

            if (structure.IsUse == false)
                UIManager.instance.GetPageController().SetSelectPage("structureSelect", structure);
        }
        else
        {
            return;
        }
    }

    public void CheckLandformPlayMusic()
    {
        var curTile = mapController.Player.TileInitInfo.GetComponent<TileBase>();

        switch (curTile.TileData.English)
        {
            case "None":
                App.instance.GetSoundManager().PlayBGM("Ambience_City");
                break;
            case "Jungle":
                App.instance.GetSoundManager().PlayBGM("Ambience_Jungle");
                break;
            case "Desert":
                App.instance.GetSoundManager().PlayBGM("Ambience_Desert");
                break;
            case "Tundra":
                App.instance.GetSoundManager().PlayBGM("Ambience_Tundra");
                break;
        }
    }

    public void NormalStructureResearch(StructureBase structure)
    {
        int randomNumber = UnityEngine.Random.Range(1, 4);

        if (randomNumber == 3)
            mapController.SpawnStructureZombies(structure.Colleagues);

        // 플레이어 체력 0으로 만들어서 경로 선택 막기
        if (isTundraTile)
        {
            UIManager.instance.GetPageController().SetResultPage("SEARCH_TUNDRA", false);
            mapController.Player.SetHealth(false);
        }

        // 경로 삭제
        MoveCancel();

        structure.structureModel.GetComponent<StructureFade>().FadeIn();
        structure.Colleagues.ForEach(tile => tile.ResourceUpdate(true));

        mapController.SpawnSpecialItemRandomTile(structure.Colleagues);
        curStructure = structure;
    }

    public void ResearchCancel(StructureBase structure)
    {
        Debug.Log("조사 취소!");
    }

    public void MoveCancel()
    {
        if (mapController.IsMovePathSaved() == false)
            return;

        uiController.OffPlayerMovePoint();
        mapController.DeletePlayerMovePath();
    }

    // public void TutorialTileCheck()
    // {
    //     if (mapController.Player.TileController.GetComponent<TileBase>().TileData.English == "None")
    //     {
    //         UIManager.instance.GetInventoryController().AddItemByItemCode("ITEM_NETWORKCHIP");
    //         isVisitNoneTile = true;
    //     }
    // }

    public bool SensingSignalTower()
    {
        return mapController.SensingSignalTower();
    }

    public bool SignalTowerQuestCheck()
    {
        if (curStructure == null)
            return false;

        if (curStructure.VisitDay != UIManager.instance.GetNoteController().dayCount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 카메라 정중앙 좌표를 반환하는 함수
    public void GetCameraCenterTile()
    {
        Vector3 centerPos = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
        Ray ray = Camera.main.ScreenPointToRay(centerPos);

        int onlyLayerMaskTile = 1 << LayerMask.NameToLayer("Tile");

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, onlyLayerMaskTile))
        {
            var target = hit.transform.parent.GetComponent<TileInitInfo>();

            if (target == null)
            {
                return;
            }

            if (cameraTarget != target)
            {
                cameraTarget = target;
                mapController.OcclusionCheck(cameraTarget.Model);
            }
        }
    }

    public void TundraTileCheck()
    {
        isTundraTile = true;
    }

    public void EtherResourceCheck()
    {
        var resources = resourceManager.GetLastResources();

        if (resources.Count == 0 || resources == null)
            return;

        if (resources.Find(x => x.ItemBase.data.Code == "ITEM_GAS") != null)
        {
            Debug.Log("에테르 디버프");
            UIManager.instance.GetPageController().SetResultPage("ACIDENT_ETHER", false);
            mapController.Player.SetHealth(false);
        }
        else
        {
            return;
        }
    }

    public bool IsJungleTile(TileInitInfo tileInitInfo)
    {
        if (tileInitInfo.GetComponent<TileBase>().TileType == ETileType.Jungle)
            return true;
        else
        {
            return false;
        }
    }

    public void SetIsDronePrepared(bool _isDronePrepared, string type)
    {
        preparesDrone = _isDronePrepared;

        if (type == "Distrubtor")
            isDisturbtor = true;
        else
            isDisturbtor = false;
    }

    public void InvocationExplorers()
    {
        mapController.InvocationExplorers();
    }
}