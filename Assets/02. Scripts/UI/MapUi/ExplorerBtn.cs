using UnityEngine;

public class ExplorerBtn : MapBtnBase
{
    protected override void OnClickEvent()
    {
        if (App.Manager.UI.GetPanel<InventoryPanel>().CheckFindorUsage())
        {
            Debug.Log("탐색기 있음");
            if (App.Manager.Map.CheckCanInstallDrone())
            {
                Debug.Log("탐색기 설치 가능");

                App.Manager.Map.mapController.PreparingExplorer(true);
            }
        }
        else
        {
            return;
        }
    }
}