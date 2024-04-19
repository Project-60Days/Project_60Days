using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BLADE", menuName = "EquipItems/Item_Blade")]
public class Item_Blade : ItemBase
{
    int beforeDurabillity = 0;

    public override void Equip()
    {
        beforeDurabillity = App.Manager.Map.mapController.Player.Durability;
        App.Manager.Map.mapController.Player.Durability += (int)data.value1;
        UIManager.instance.GetUpperController().IncreaseDurabillityAnimation();
    }

    public override bool CheckMeetCondition()
    {
        return (App.Manager.Map.mapController.Player.Durability <= beforeDurabillity);
    }
}
