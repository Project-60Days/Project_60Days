using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SHAPEMETAL", menuName = "EquipItems/Item_Shapemetal")]
public class Item_Shapemetal : ItemBase
{
    int beforeDay = 0;
    int beforeDurabillity = 0;

    public override void Equip()
    {
        beforeDay = App.Manager.Game.dayCount;
        beforeDurabillity = App.Manager.Map.mapCtrl.Player.Durability;

        App.Manager.Map.mapCtrl.Player.Durability += (int)data.value1;
        App.Manager.Map.mapCtrl.Player.ClockUntil((int)data.value2);
        App.Manager.UI.GetPanel<UpperPanel>().IncreaseDurabillityAnimation();
    }

    public override bool CheckMeetCondition()
    {
        return (App.Manager.Game.dayCount - beforeDay >= 6 && App.Manager.Map.mapCtrl.Player.Durability <= beforeDurabillity) ;
    }
}
