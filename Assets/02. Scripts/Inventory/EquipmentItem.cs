using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
public class EquipmentItem : ItemBase
{
    ItemType thisType = ItemType.Consumption;

    public EquipmentItem()
    {
        itemType = thisType;
    }
}
