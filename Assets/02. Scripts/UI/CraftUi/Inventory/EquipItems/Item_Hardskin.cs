using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HARDSKIN", menuName = "EquipItems/Item_Hardskin")]
public class Item_Hardskin : ItemBase
{
    int beforeDurabillity = 0;

    public override void Equip()
    {
        beforeDurabillity = App.instance.GetMapManager().Controller.Player.Durability;
        App.instance.GetMapManager().Controller.Player.Durability += (int)data.value1;
        UIManager.instance.GetUpperController().UpdateDurabillity();
    }

    public override bool CheckMeetCondition()
    {
        return (App.instance.GetMapManager().Controller.Player.Durability <= beforeDurabillity);
    }
}
