using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ALLOY", menuName = "EquipItems/Item_Alloy")]
public class Item_Alloy : ItemBase
{
    public override void Equip()
    {
        App.instance.GetMapManager().mapController.Player.Durability += 20;
        UIManager.instance.GetUpperController().UpdateDurabillity();
    }

    public override void UnEquip()
    {
        App.instance.GetMapManager().mapController.Player.Durability -= 20;
        UIManager.instance.GetUpperController().UpdateDurabillity();
    }
}
