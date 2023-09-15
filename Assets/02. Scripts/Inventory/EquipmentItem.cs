using UnityEngine;

[CreateAssetMenu]
public class EquipmentItem : ItemBase
{
    private readonly EItemType thisType = EItemType.Equipment;

    public EquipmentItem()
    {
        eItemType = thisType;
    }
}
