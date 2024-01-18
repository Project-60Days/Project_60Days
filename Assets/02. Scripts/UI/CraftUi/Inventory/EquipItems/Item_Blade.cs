using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BLADE", menuName = "EquipItems/Item_Blade")]
public class Item_Blade : ItemBase
{
    public override void Equip()
    {
        App.instance.GetMapManager().mapController.Player.Durability += 15;
        UIManager.instance.GetUpperController().UpdateDurabillity();
    }

    public override void UnEquip()
    {
        App.instance.GetMapManager().mapController.Player.Durability -= 15;
        UIManager.instance.GetUpperController().UpdateDurabillity();
    }
}
