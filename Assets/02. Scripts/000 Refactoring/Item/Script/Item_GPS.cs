using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GPS", menuName = "EquipItems/Item_GPS")]
public class Item_GPS : ItemBase
{
    private int beforeDay;

    public override void Equip()
    {
        beforeDay = App.Manager.Game.dayCount;

        App.Manager.Asset.Fog.AddRange((int)data.value1);
    }

    public override bool CheckMeetCondition()
        => App.Manager.Game.dayCount - beforeDay == 1;
}