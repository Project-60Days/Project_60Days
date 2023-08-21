using UnityEngine;

[CreateAssetMenu]
public class MaterialItem : ItemBase
{
    private readonly ItemType thisType = ItemType.Material;

    public MaterialItem()
    {
        itemType = thisType;
    }
}
