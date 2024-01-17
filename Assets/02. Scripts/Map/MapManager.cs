using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Hexamap;
using Yarn.Compiler;

[System.Serializable]
public class MapData
{
    [Tooltip("자원 등장 확률")] public int resourcePercent;
    
    [Tooltip("시야")] public int fogSightRange;

    [Tooltip("이동 거리")] public int playerMovementPoint;

    [Tooltip("좀비 감지 범위")] public int zombieDetectionRange;

    [Tooltip("등장 좀비 수")] public int zombieCount;
}

public class MapManager : ManagementBase
{
    public MapUiController mapUIController;
    public MapController mapController;
    public ResourceManager resourceManager;
    public bool mouseIntreractable;

    [SerializeField] ETileMouseState mouseState;

    [Header("밸런스 테스트 용")] [Space(5f)] [SerializeField]
    MapData mapData;

    Camera mainCamera;
    MapCamera mapCineCamera;
    TileController curTileController;
    StructureBase curStructure;

    bool canPlayerMove;
    bool isDronePrepared;
    bool isDisturbtorPrepared;
    bool isVisitNoneTile;

    void Update()
    {
        SetETileMoveState();

        if (mouseState != ETileMouseState.Nothing)
        {
            MouseOverEvents();
        }
    }

    IEnumerator GetAdditiveSceneObjects()
    {
        yield return new WaitForEndOfFrame();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        mapUIController = GameObject.FindGameObjectWithTag("MapUi").GetComponent<MapUiController>();
        mapController = GameObject.FindGameObjectWithTag("MapController").GetComponent<MapController>();

        mapController.InputMapData(mapData);

        yield return new WaitUntil(() => mapController != null);
        StartCoroutine(mapController.GenerateMap());
        mapCineCamera = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<MapCamera>();

        AllowMouseEvent(true);
        resourceManager = GameObject.FindGameObjectWithTag("Resource").GetComponent<ResourceManager>();
        StartCoroutine(mapCineCamera.GetMapInfo());
    }

    public void GetAdditiveSceneObjectsCoroutine()
    {
        StartCoroutine(GetAdditiveSceneObjects());
    }

    void MouseOverEvents()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            mapController.DeselectAllBorderTiles();
            return;
        }

        RaycastHit hit;
        TileController tileController;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        int onlyLayerMaskTile = 1 << LayerMask.NameToLayer("Tile");


        if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile))
        {
            tileController = hit.transform.parent.GetComponent<TileController>();

            mapController.DeselectAllBorderTiles();

            if (!mapController.CheckPlayersView(tileController))
            {
                mapUIController.FalseTileInfo();
                return;
            }

            switch (mouseState)
            {
                case ETileMouseState.CanClick:
                    mapController.DefalutMouseOverState(tileController);

                    if (tileController != curTileController)
                        mapUIController.FalseTileInfo();
                    break;

                case ETileMouseState.CanPlayerMove:
                    mapController.TilePathFinderSurroundings(tileController);
                    mapController.AddSelectedTilesList(tileController);
                    break;

                case ETileMouseState.DronePrepared:
                    if (isDisturbtorPrepared)
                    {
                        mapController.DisturbtorPathFinder(tileController);
                    }
                    else
                    {
                        mapController.TilePathFinder(tileController, 5);
                    }

                    break;
            }

            curTileController = tileController;
        }
        else
        {
            mapController.DeselectAllBorderTiles();
            mapUIController.FalseTileInfo();
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
                    canPlayerMove = mapController.PlayerCanMoveCheck();
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile))
            {
                TileController tileController = hit.transform.parent.GetComponent<TileController>();

                if (!canPlayerMove && !isDronePrepared)
                {
                    tileController.GetComponent<TileBase>().TileInfoUpdate();
                    mapUIController.TrueTileInfo();
                    //Debug.Log(hit.transform.parent.GetComponent<TileInfo>().GetStructureName());
                }
                else if (canPlayerMove)
                {
                    if (mapController.SelectPlayerMovePoint(tileController))
                    {
                        mapUIController.OnPlayerMovePoint(tileController.transform);
                        canPlayerMove = false;
                    }
                    else
                        return;
                }
                else if (isDronePrepared)
                {
                    if (isDisturbtorPrepared)
                    {
                        mapController.SelectTileForDisturbtor(tileController);
                    }
                    else
                    {
                        mapController.SelectTileForExplorer(tileController);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            mapController.DeselectAllBorderTiles();

            if (canPlayerMove)
            {
                canPlayerMove = false;
            }

            // 목적지 정한 이후 취소 가능

            if (isDronePrepared)
            {
                if (isDisturbtorPrepared)
                {
                    mapController.PreparingDisturbtor(false);
                }
                else
                {
                    mapController.PreparingExplorer(false);
                }
            }

            MovePathDelete();
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

    public IEnumerator NextDayCoroutine()
    {
        yield return StartCoroutine(mapController.NextDay());
        resourceManager.GetResource(mapController.Player.TileController);
        mapUIController.OffPlayerMovePoint();

        CheckRoutine();
    }

    public bool CheckCanInstallDrone()
    {
        if (mouseState == ETileMouseState.CanClick)
        {
            return false;
        }

        return true;
    }

    public void AllowMouseEvent(bool isAllow)
    {
        canPlayerMove = false;
        isDronePrepared = false;
        isDisturbtorPrepared = false;
        mouseIntreractable = isAllow;
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
        if (structure != null && structure.IsUse == false)
        {
            UIManager.instance.GetPageController().SetSelectPage("structureSelect", structure);
        }
        else
        {
            //Debug.Log("근처에 구조물이 없습니다.");
            return;
        }
    }

    public void CheckLandformPlayMusic()
    {
        var curTile = mapController.Player.TileController.GetComponent<TileBase>();

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

    public void ResearchStart(StructureBase structure)
    {
        // 자원 보이게

        // 좀비 생성
        mapController.SpawnStructureZombie(structure.NeighborTiles);

        // 플레이어 체력 0으로 만들어서 경로 선택 막기
        mapController.Player.SetHealth(false);

        // 경로 삭제
        MovePathDelete();

        curStructure = structure;
        curStructure.SetIsUse(true);
    }

    public void ResearchCancel()
    {
        Debug.Log("조사 취소!");
    }

    public void MovePathDelete()
    {
        if (mapController.IsMovePathSaved() == false)
            return;

        mapUIController.OffPlayerMovePoint();
        mapController.DeletePlayerMovePath();
    }

    public void TutorialTileCheck()
    {
        if (mapController.Player.TileController.GetComponent<TileBase>().TileData.English == "None")
        {
            UIManager.instance.GetInventoryController().AddItemByItemCode("ITEM_NETWORKCHIP");
            isVisitNoneTile = true;
        }
    }

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
    
    public void ToolUIOpen()
    {
        mapUIController.SetDisturbtorButtonInteractable(true);
        mapUIController.SetExplorerButtonInteractable(true);
    }
}