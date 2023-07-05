using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ConsumptionItem : ItemBase
{
    ItemType thisType = ItemType.Consumption;

    public ConsumptionItem()
    {
        itemType= thisType;
    }
}
