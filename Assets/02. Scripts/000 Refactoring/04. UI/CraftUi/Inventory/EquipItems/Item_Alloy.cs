using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ALLOY", menuName = "EquipItems/Item_Alloy")]
public class Item_Alloy : ItemBase
{
    int beforeDurabillity = 0;

    public override void Equip()
    {
        beforeDurabillity = App.Manager.Game.durability;

        App.Manager.Game.durability += (int)data.value1;
        App.Manager.UI.GetPanel<UpperPanel>().IncreaseDurabillityAnimation();
    }

    public override bool CheckMeetCondition()
    {
        return (App.Manager.Game.durability <= beforeDurabillity);
    }
}
