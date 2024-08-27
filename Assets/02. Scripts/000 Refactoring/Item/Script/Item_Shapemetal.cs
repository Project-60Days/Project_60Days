using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SHAPEMETAL", menuName = "EquipItems/Item_Shapemetal")]
public class Item_Shapemetal : ItemDefense
{
    private int beforeDay;

    public override void Equip()
    {
        base.Equip();

        beforeDay = App.Manager.Game.dayCount;

        App.Data.Test.SetCloaking((int)data.value2);
    }

    public override bool CheckMeetCondition()
        => App.Manager.Game.dayCount - beforeDay >= 6 && App.Manager.Game.durability <= beforeDurabillity;
}
