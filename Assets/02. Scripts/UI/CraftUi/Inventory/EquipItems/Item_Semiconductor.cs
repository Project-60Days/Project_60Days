using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SEMICONDUCTOR", menuName = "EquipItems/Item_Semiconductor")]
public class Item_Semiconductor : ItemBase
{
    int beforeDurabillity = 0;

    public override void Equip()
    {
        beforeDurabillity = App.instance.GetMapManager().mapController.Player.Durability;
        App.instance.GetMapManager().mapController.Player.Durability += (int)data.value1;
        UIManager.instance.GetUpperController().IncreaseDurabillityAnimation();
    }

    public override bool CheckMeetCondition()
    {
        return (App.instance.GetMapManager().mapController.Player.Durability <= beforeDurabillity);
    }
}
