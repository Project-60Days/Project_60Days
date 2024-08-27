using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Defense", menuName = "EquipItems/Defense")]
public class ItemDefense : ItemBase
{
    protected int beforeDurabillity;

    public override void Equip()
    {
        beforeDurabillity = App.Manager.Game.durability;

        App.Manager.Game.durability += (int)data.value1;
        App.Manager.UI.GetPanel<UpperPanel>().PlayDurabilityAnim();
    }

    public override bool CheckMeetCondition()
        => App.Manager.Game.durability <= beforeDurabillity;
}
