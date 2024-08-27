using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HARDSKIN", menuName = "EquipItems/Item_Hardskin")]
public class Item_Hardskin : ItemBase
{
    int beforeDurabillity = 0;

    public override void Equip()
    {
        beforeDurabillity = App.Manager.Game.durability;
        App.Manager.Game.durability += (int)data.value1;
        App.Manager.UI.GetPanel<UpperPanel>().PlayDurabilityAnim();
    }

    public override bool CheckMeetCondition()
    {
        return (App.Manager.Game.durability <= beforeDurabillity);
    }
}
