using UnityEngine;

[CreateAssetMenu]
public class EquipmentItem : ItemBase
{
    private readonly ItemType thisType = ItemType.Equipment;

    public EquipmentItem()
    {
        itemType = thisType;
    }
}
