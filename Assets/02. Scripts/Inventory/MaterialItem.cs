using UnityEngine;

[CreateAssetMenu]
public class MaterialItem : ItemBase
{
    private readonly EItemType thisType = EItemType.Material;

    public MaterialItem()
    {
        eItemType = thisType;
    }
}
