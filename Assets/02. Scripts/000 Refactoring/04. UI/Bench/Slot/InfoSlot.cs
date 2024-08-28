using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoSlot : SlotBase
{
    [SerializeField] TextMeshProUGUI nameTMP;

    public override void SetItem(ItemBase _item)
    {
        base.SetItem(_item);

        nameTMP.text = Item.data.Korean;
    }

    public override void ResetItem()
    {
        base.ResetItem();

        nameTMP.text = string.Empty;
    }
}
