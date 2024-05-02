using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HARDSKIN", menuName = "EquipItems/Item_Hardskin")]
public class Item_Hardskin : ItemBase
{
    int beforeDurabillity = 0;

    public override void Equip()
    {
        beforeDurabillity = App.Manager.Map.mapCtrl.Player.Durability;
        App.Manager.Map.mapCtrl.Player.Durability += (int)data.value1;
        App.Manager.UI.GetPanel<UpperPanel>().IncreaseDurabillityAnimation();
    }

    public override bool CheckMeetCondition()
    {
        return (App.Manager.Map.mapCtrl.Player.Durability <= beforeDurabillity);
    }
}
