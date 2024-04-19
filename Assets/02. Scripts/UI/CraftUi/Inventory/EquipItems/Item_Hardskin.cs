using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HARDSKIN", menuName = "EquipItems/Item_Hardskin")]
public class Item_Hardskin : ItemBase
{
    int beforeDurabillity = 0;

    public override void Equip()
    {
        beforeDurabillity = App.Manager.Map.mapController.Player.Durability;
        App.Manager.Map.mapController.Player.Durability += (int)data.value1;
        UIManager.instance.GetUpperController().IncreaseDurabillityAnimation();
    }

    public override bool CheckMeetCondition()
    {
        return (App.Manager.Map.mapController.Player.Durability <= beforeDurabillity);
    }
}
