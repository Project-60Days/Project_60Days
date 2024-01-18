using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HARDSKIN", menuName = "EquipItems/Item_Hardskin")]
public class Item_Hardskin : ItemBase
{
    public override void Equip()
    {
        App.instance.GetMapManager().mapController.Player.Durability += 40;
        UIManager.instance.GetUpperController().UpdateDurabillity();
    }

    public override void UnEquip()
    {
        App.instance.GetMapManager().mapController.Player.Durability -= 40;
        UIManager.instance.GetUpperController().UpdateDurabillity();
    }
}
