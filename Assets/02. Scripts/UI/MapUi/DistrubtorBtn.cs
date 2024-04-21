using UnityEngine;

public class DistrubtorBtn : MapBtnBase
{
    protected override void OnClickEvent()
    {
        if (App.Manager.UI.GetPanel<InventoryPanel>().CheckDistrubtorUsage())
        {
            Debug.Log("교란기 있음");
            if (App.Manager.Map.CheckCanInstallDrone())
            {
                Debug.Log("교란기 설치 가능");
                App.Manager.Map.mapController.PreparingDistrubtor(true);
            }
        }
        else
        {
            return;
        }
    }
}