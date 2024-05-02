using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DECK", menuName = "EquipItems/Item_Deck")]
public class Item_Deck : ItemBase
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
