using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GENERATOR", menuName = "EquipItems/Item_Generator")]
public class Item_Generator : ItemBase
{
    int beforeDay;

    public override void Equip()
    {
        beforeDay = UIManager.instance.GetNoteController().dayCount;
    }

    public override void DayEvent()
    {
        if ((UIManager.instance.GetNoteController().dayCount - beforeDay) % data.value1 == 0)
        {
            for (int i = 0; i < data.value2; i++)
                UIManager.instance.GetInventoryController().AddItemByItemCode("ITEM_PLASMA");
        }
    }
}