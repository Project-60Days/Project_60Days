using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GENERATOR", menuName = "EquipItems/Item_Generator")]
public class Item_Generator : ItemBase
{
    int beforeDay;

    public override void Equip()
    {
        beforeDay = App.Manager.UI.GetPanel<NotePanel>().dayCount;
    }

    public override void DayEvent()
    {
        if ((App.Manager.UI.GetPanel<NotePanel>().dayCount - beforeDay) % data.value1 == 0)
        {
            for (int i = 0; i < data.value2; i++)
                App.Manager.UI.GetInventoryController().AddItemByItemCode("ITEM_PLASMA");
        }
    }
}