using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BLADE", menuName = "EquipItems/Item_Blade")]
public class Item_Blade : ItemBase
{
    int beforeDurabillity = 0;

    public override void Equip()
    {
        beforeDurabillity = App.Manager.Map.mapCtrl.Player.Durability;
        App.Manager.Map.mapCtrl.Player.Durability += (int)data.value1;
        App.Manager.UI.GetPanel<UpperPanel>().IncreaseDurabillityAnimation();
    }

    public override bool CheckMeetCondition()
    {
        return (App.Manager.Map.mapCtrl.Player.Durability <= beforeDurabillity);
    }
}
