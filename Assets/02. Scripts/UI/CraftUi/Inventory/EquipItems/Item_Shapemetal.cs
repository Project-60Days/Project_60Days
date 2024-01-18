using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SHAPEMETAL", menuName = "EquipItems/Item_Shapemetal")]
public class Item_Shapemetal : ItemBase
{
    public override void Equip()
    {
        App.instance.GetMapManager().mapController.Player.Durability += 25;
        UIManager.instance.GetUpperController().UpdateDurabillity();
    }

    public override void UnEquip()
    {
        App.instance.GetMapManager().mapController.Player.Durability -= 25;
        UIManager.instance.GetUpperController().UpdateDurabillity();
    }
}
