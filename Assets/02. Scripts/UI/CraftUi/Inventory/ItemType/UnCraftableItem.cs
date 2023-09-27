using UnityEngine;

[CreateAssetMenu]
public class UnCraftableItem : ItemBase
{
    private readonly EItemType thisType = EItemType.UnCraftable;

    public UnCraftableItem()
    {
        eItemType = thisType;
    }
}
