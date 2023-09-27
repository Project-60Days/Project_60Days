using UnityEngine;

[CreateAssetMenu]
public class SpecialItem : ItemBase
{
    private readonly EItemType thisType = EItemType.Special;

    public SpecialItem()
    {
        eItemType = thisType;
    }
}
