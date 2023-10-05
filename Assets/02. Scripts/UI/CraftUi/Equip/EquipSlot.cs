using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipSlot : SlotBase
{
    [SerializeField] public EItemType eItemType = EItemType.Equipment;

    public EquipSlot()
    {
        eSlotType = ESlotType.EquipSlot;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.GetCraftingUiController().MoveEquipToInventory(_item);
    }
}
