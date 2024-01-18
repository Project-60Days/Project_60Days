using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SEMICONDUCTOR", menuName = "EquipItems/Item_Semiconductor")]
public class Item_Semiconductor : ItemBase
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
