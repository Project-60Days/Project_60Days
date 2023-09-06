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
    DronePrepared,
}

public class MapManager : ManagementBase
{
    Camera mainCamera;
    MapCamera mapCineCamera;
    MapController mapController;
    ResourceManager resourceManager;

    bool isPlayerSelected;
    bool isDronePrepared;
    bool isDisturbtor;

    public MapUiController mapUIController;
    public ETileMouseState mouseState;
    public bool interactable;


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
        mapController.GenerateMap();
        mapCineCamera = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<MapCamera>();
        StartCoroutine(mapCineCamera.GetMapInfo());
    }

    public void GetAdditiveSceneObjectsCoroutine()
    {
        StartCoroutine(GetAdditiveSceneObjects());
    }

    /// <summary>
    /// Raytracing을 통해 마우스 현재 위치에 맞는 타일의 정보를 가져오거나, 타일의 하위 오브젝트를 활성화시키는 함수.
    /// </summary>
    void MouseOverEvents()
    {
        RaycastHit hit;
        TileController tileController;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        int onlyLayerMaskTile = 1 << LayerMask.NameToLayer("Tile");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile))
        {
            tileController = hit.transform.parent.GetComponent<TileController>();

            mapController.DeselectAllBorderTiles();

            if (!mapController.CheckPlayersView(tileController))
                return;

            switch (mouseState)
            {
                case ETileMouseState.CanClick:
                    mapController.DefalutMouseOverState(tileController);
                    mapUIController.SetActiveTileInfo(false);
                    break;

                case ETileMouseState.CanPlayerMove:
                    mapController.TilePathFinder(tileController);
                    mapController.AddSelectedTilesList(tileController);
                    break;
                case ETileMouseState.DronePrepared:
                    if (isDisturbtor)
                    {
                        mapController.DisturbtorPathFinder(tileController);
                    }
                    else
                    {
                        mapController.TilePathFinder(tileController, 5);
                    }
                    break;
            }
        }
        else
        {
            mapController.DeselectAllBorderTiles();
            mapUIController.SetActiveTileInfo(false);
        }

        MouseClickEvents();
    }

    /// <summary>
    /// 플레이어 이동, 교란기, 탐색기 설치 등 마우스 클릭이벤트들을 담은 함수. Raycast 사용
    /// </summary>
    void MouseClickEvents()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        int onlyLayerMaskPlayer = 1 << LayerMask.NameToLayer("Player");
        int onlyLayerMaskTile = 1 << LayerMask.NameToLayer("Tile");

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskPlayer))
            {
                if (!isDronePrepared && !mapUIController.MovePointActivate())
                    isPlayerSelected = mapController.PlayerCanMoveCheck();
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile))
            {
                TileController tileController = hit.transform.parent.GetComponent<TileController>();

                if (!isPlayerSelected && !isDronePrepared)
                {
                    mapUIController.SetActiveTileInfo(true);
                }
                else if (isPlayerSelected)
                {

                    if (mapController.SelectPlayerMovePoint(tileController))
                    {
                        mapUIController.OnPlayerMovePoint(tileController.transform);
                        isPlayerSelected = false;
                    }
                    else
                        return;

                }
                else if (isDronePrepared)
                {
                    if (isDisturbtor)
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

        // 우클릭 시 선택 취소
        if (Input.GetMouseButtonDown(1))
        {
            mapController.DeselectAllBorderTiles();

            if (isPlayerSelected)
            {
                isPlayerSelected = false;
            }

            if (isDronePrepared)
            {
                if (isDisturbtor)
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

    /// <summary>
    /// 마우스 이벤트 컨트롤 위한 열거형 업데이트 시켜주는 함수
    /// Update에서 호출
    /// </summary>
    void SetETileMoveState()
    {
        if (!interactable)
            mouseState = ETileMouseState.Nothing;

        else if (!isPlayerSelected && !isDronePrepared)
            mouseState = ETileMouseState.CanClick;

        else if (isPlayerSelected)
            mouseState = ETileMouseState.CanPlayerMove;

        else if (isDronePrepared)
            mouseState = ETileMouseState.DronePrepared;
    }

    IEnumerator NextDayCoroutine()
    {
        yield return StartCoroutine(mapController.NextDay());
        resourceManager.GetResource(mapController.Player.TileController);
        mapUIController.OffPlayerMovePoint();
    }

    /// <summary>
    /// 행동 선택 중일 때 드론 생성 버튼 클릭 방지
    /// </summary>
    /// <returns></returns>
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
        isPlayerSelected = false;
        isDronePrepared = false;
        isDisturbtor = false;

        interactable = isAllow;
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
}
