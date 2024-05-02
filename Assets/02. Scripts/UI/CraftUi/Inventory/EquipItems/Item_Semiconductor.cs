using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SEMICONDUCTOR", menuName = "EquipItems/Item_Semiconductor")]
public class Item_Semiconductor : ItemBase
{
    int beforeDurabillity = 0;

    public override void Equip()
    {
        beforeDurabillity = App.Manager.Map.mapController.Player.Durability;
        App.Manager.Map.mapController.Player.Durability += (int)data.value1;
        App.Manager.UI.GetPanel<UpperPanel>().IncreaseDurabillityAnimation();
    }

    public override bool CheckMeetCondition()
    {
        return (App.Manager.Map.mapController.Player.Durability <= beforeDurabillity);
    }
}
