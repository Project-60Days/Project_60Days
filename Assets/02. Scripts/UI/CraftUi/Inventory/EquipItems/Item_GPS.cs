using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GPS", menuName = "EquipItems/Item_GPS")]
public class Item_GPS : ItemBase
{
    int beforeDay = 0;

    public override void Equip()
    {
        beforeDay = App.Manager.Game.dayCount;

        App.Manager.Map.mapCtrl.playerCtrl.player.AddSightRange((int)data.value1);
    }

    public override bool CheckMeetCondition()
    {
        return (App.Manager.Game.dayCount - beforeDay == 1);
    }
}