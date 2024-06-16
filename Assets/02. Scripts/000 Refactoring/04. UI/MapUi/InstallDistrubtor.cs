using UnityEngine;

public class InstallDistrubtor : InstallBase
{
    protected override string GetString()
    {
        return App.Data.Game.GetString("STR_EXPLORER_DESC");
    }

    protected override void OnClickEvent()
    {
        if (App.Manager.UI.GetPanel<InventoryPanel>().CheckDistrubtorUsage())
        {
            if (App.Manager.Map.CanClick)
            {
                Debug.Log("교란기 설치 가능");
                App.Manager.Map.GetUnit<DroneUnit>().Prepare(DroneType.Disruptor);
            }
        }
    }
}