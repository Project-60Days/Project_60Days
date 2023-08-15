using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : ManagementBase
{
    Camera mapCamera;
    MapController mapGenerator;
    MapUiController mapUiController;
    ResourceManager resourceManager;

    // Player 스크립트로 옮기기
    int currentHealth;
    int maxHealth = 1;

    IEnumerator GetAdditiveSceneObjects()
    {
        yield return new WaitForEndOfFrame();

    }

    public void CreateMap()
    {
        StartCoroutine(GetAdditiveSceneObjects());
    }
    public override EManagerType GetManagemetType()
    {
        throw new System.NotImplementedException();
    }

/*
    void MouseOverEvent()
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

    void MouseClickEvent()
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
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, onlyLayerMaskTile))
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
                        && player.TileController.Model != tileController.Model)
                    {
                        SavePlayerMovePath(tileController);
                    }
                }
                else if (isDisturbanceSet)
                {
                    if (map.Map.GetTilesInRange(player.TileController.Model, 1).Contains(tileController.Model)
                        && GetTileBorder(tileController, ETileState.Moveable).activeInHierarchy)
                    {
                        foreach (var item in player.TileController.Model.Neighbours.Where(item => item.Value == tileController.Model))
                        {
                            InstallDisturbtor(tileController, item.Key);
                        }
                    }
                }
                else if (isExplorerSet)
                {
                    if (GetTileBorder(tileController, ETileState.Moveable).activeInHierarchy
                        && player.TileController.Model != tileController.Model)
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
*/
}
