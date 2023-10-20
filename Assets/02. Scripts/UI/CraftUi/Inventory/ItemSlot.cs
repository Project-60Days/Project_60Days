using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class ItemSlot : SlotBase
{
    public static Action<GameObject> CraftItemClick;

    public int category;

    public ItemSlot()
    {
        eSlotType = ESlotType.InventorySlot;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        var item = _item;
        
        if (UIManager.instance.GetCraftModeController().eCraftModeType == ECraftModeType.Craft)
        {
            UIManager.instance.GetCraftingUiController().MoveInventoryToCraft(item);
            CraftItemClick?.Invoke(item.prefab);
        }
        else if (UIManager.instance.GetCraftModeController().eCraftModeType == ECraftModeType.Equip)
        {
            if (item.eItemType != EItemType.Equipment) return;
            UIManager.instance.GetCraftingUiController().MoveInventoryToEquip(item);
        }
    }
}