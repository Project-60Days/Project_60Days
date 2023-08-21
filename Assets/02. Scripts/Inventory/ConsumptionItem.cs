using UnityEngine;

[CreateAssetMenu]
public class ConsumptionItem : ItemBase
{
    private readonly ItemType thisType = ItemType.Consumption;

    public ConsumptionItem()
    {
        itemType= thisType;
    }
}
