using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Hexamap;

public enum ETileMouseState
{
    Nothing,
    CanClick,
    CanPlayerMove,
    DronePrepared
}

public class MapManager : ManagementBase
{
    public MapUiController mapUIController;
    public ETileMouseState mouseState;
    public bool mouseIntreractable;

    Camera mainCamera;
    MapCamera mapCineCamera;
    public MapController mapController;
    public ResourceManager resourceManager;
    TileController curTileController;

    bool canPlayerMove;
    bool isDronePrepared;
    bool isDisturbtorPrepared;
    bool isVisitNoneTile = false;

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
                    mapController.TilePathFinder(tileController);
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
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskPlayer))
            {
                if (!isDronePrepared && !mapUIController.MovePointActivate())
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

    public void OnTargetPointUI()
    {
        mapUIController.OnPlayerMovePoint(mapController.TargetPointTile.transform);
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
        if(isVisitNoneTile == false)
        {
            TutorialTileCheck();
        }
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
            UIManager.instance.GetPageController().SetSelectPage("structureSelect", structure);
        }
        else
        {
            Debug.Log("근처에 구조물이 없습니다.");
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

    public void ResearchStart()
    {
        Debug.Log("조사 시작!");
    }

    public void ResearchCancel()
    {
        Debug.Log("조사 취소!");
    }

    public void TutorialTileCheck()
    {
        if(mapController.Player.TileController.GetComponent<TileBase>().TileData.English == "None")
        {
            UIManager.instance.GetInventoryController().AddItemByItemCode("ITEM_NETWORKCHIP");
            isVisitNoneTile = true;
        }
    }

    public bool SensingSignalTower()
    {
        return mapController.SensingSignalTower();
    }
}