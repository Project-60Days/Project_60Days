using UnityEngine;

public class InstallExplorer : InstallBase
{
    protected override string GetString()
        => App.Data.Game.GetString("STR_EXPLORER_DESC");

    protected override string GetItemCode()
        => "ITEM_EXPLORER";

    protected override void OnClickEvent()
    {
        if (App.Manager.Map.CanClick)
        {
            Debug.Log("탐색기 설치 가능");
            App.Manager.Map.GetUnit<DroneUnit>().Prepare(DroneType.Explorer);
        }
    }
}