using UnityEngine;

public class InstallExplorer : InstallBase
{
    protected override string SetString()
    {
        return App.Data.Game.GetString("STR_DISTRUBTOR_DESC");
    }

    protected override void OnClickEvent()
    {
        if (App.Manager.UI.GetPanel<InventoryPanel>().CheckFindorUsage())
        {
            if (App.Manager.Map.CheckCanInstallDrone())
            {
                Debug.Log("탐색기 설치 가능");

                App.Manager.Map.mapCtrl.droneCtrl.PreparingExplorer();
            }
        }
    }
}