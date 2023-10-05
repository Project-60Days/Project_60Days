using UnityEngine;

[CreateAssetMenu]
public class BothItem : ItemBase
{
    private readonly EItemType thisType = EItemType.Both;

    public BothItem()
    {
        eItemType = thisType;
    }
}
