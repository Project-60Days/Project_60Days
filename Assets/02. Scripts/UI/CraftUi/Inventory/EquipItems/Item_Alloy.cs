using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ALLOY", menuName = "EquipItems/Item_Alloy")]
public class Item_Alloy : ItemBase
{
    int beforeDurabillity = 0;

    public override void Equip()
    {
        beforeDurabillity = App.Manager.Map.mapController.Player.Durability;

        App.Manager.Map.mapController.Player.Durability += (int)data.value1;
        App.Manager.UI.GetUpperController().IncreaseDurabillityAnimation();
    }

    public override bool CheckMeetCondition()
    {
        return (App.Manager.Map.mapController.Player.Durability <= beforeDurabillity);
    }
}
