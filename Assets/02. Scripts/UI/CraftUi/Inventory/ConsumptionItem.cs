using UnityEngine;

[CreateAssetMenu]
public class ConsumptionItem : ItemBase
{
    private readonly EItemType thisType = EItemType.Consumption;

    public ConsumptionItem()
    {
        eItemType = thisType;
    }
}
