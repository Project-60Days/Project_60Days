using UnityEngine;

public class InstallExplorer : InstallBase
{
    protected override string GetString() => App.Data.Game.GetString("STR_EXPLORER_DESC");

    protected override string GetItemCode() => "ITEM_EXPLORER";
    protected override DroneType GetDroneType() => DroneType.Explorer;
}