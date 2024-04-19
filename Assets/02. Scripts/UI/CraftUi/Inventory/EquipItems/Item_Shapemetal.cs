using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SHAPEMETAL", menuName = "EquipItems/Item_Shapemetal")]
public class Item_Shapemetal : ItemBase
{
    int beforeDay = 0;
    int beforeDurabillity = 0;

    public override void Equip()
    {
        beforeDay = UIManager.instance.GetNoteController().dayCount;
        beforeDurabillity = App.Manager.Map.mapController.Player.Durability;

        App.Manager.Map.mapController.Player.Durability += (int)data.value1;
        App.Manager.Map.mapController.Player.ClockUntil((int)data.value2);
        UIManager.instance.GetUpperController().IncreaseDurabillityAnimation();
    }

    public override bool CheckMeetCondition()
    {
        return (UIManager.instance.GetNoteController().dayCount - beforeDay >= 6 && App.Manager.Map.mapController.Player.Durability <= beforeDurabillity) ;
    }
}
