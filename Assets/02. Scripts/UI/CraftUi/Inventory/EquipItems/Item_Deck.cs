using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DECK", menuName = "EquipItems/Item_Deck")]
public class Item_Deck : ItemBase
{
    int beforeDurabillity = 0;

    public override void Equip()
    {
        beforeDurabillity = App.instance.GetMapManager().mapController.Player.Durability;
        App.instance.GetMapManager().mapController.Player.Durability += (int)data.value1;
        UIManager.instance.GetUpperController().UpdateDurabillity();
    }

    public override bool CheckMeetCondition()
    {
        return (App.instance.GetMapManager().mapController.Player.Durability <= beforeDurabillity);
    }
}
