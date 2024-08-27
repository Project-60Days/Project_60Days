using UnityEngine;

public class InstallDisruptor : InstallBase
{
    protected override string GetString() => App.Data.Game.GetString("STR_DISRUPTOR_DESC");

    protected override string GetItemCode()
    => "ITEM_DISRUPTOR";


    protected override void OnClickEvent()
    {
        if (App.Manager.Map.CanClick)
        {
            Debug.Log("교란기 설치 가능");
            App.Manager.Map.GetUnit<DroneUnit>().Prepare(DroneType.Disruptor);
        }
    }
}