using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GPS", menuName = "EquipItems/Item_GPS")]
public class Item_GPS : ItemBase
{
    int beforeDay = 0;

    public override void Equip()
    {
        beforeDay = App.Manager.UI.GetPanel<NotePanel>().dayCount;

        App.Manager.Map.mapCtrl.Player.AddSightRange((int)data.value1);
    }

    public override bool CheckMeetCondition()
    {
        return (App.Manager.UI.GetPanel<NotePanel>().dayCount - beforeDay == 1);
    }
}