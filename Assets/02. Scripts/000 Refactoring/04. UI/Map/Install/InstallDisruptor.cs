using UnityEngine;

public class InstallDisruptor : InstallBase
{
    protected override string GetString() => App.Data.Game.GetString("STR_DISRUPTOR_DESC");

    protected override string GetItemCode() => "ITEM_DISRUPTOR";
    protected override DroneType GetDroneType() => DroneType.Disruptor;
}