using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SHAPEMETAL", menuName = "EquipItems/Item_Shapemetal")]
public class Item_Shapemetal : ItemDefense
{
    private int beforeDay;

    public override void Equip()
    {
        beforeDay = App.Manager.Game.DayCount;

        App.Data.Test.SetCloaking();
    }

    public override void UnEquip()
    { 
        App.Data.Test.UnsetCloaking();
    }

    public override bool CheckMeetCondition()
        => App.Manager.Game.DayCount - beforeDay >= (int)Data.value2 && App.Manager.Game.Durability <= beforeDurabillity;
}
