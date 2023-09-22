using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class ItemSlot : SlotBase
{
    public static Action<GameObject> CraftItemClick;

    public ItemSlot()
    {
        eSlotType = ESlotType.InventorySlot;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        var item = _item;

        if (UIManager.instance.GetCraftModeController().GetECraftModeType() == ECraftModeType.Craft)
        {
            UIManager.instance.GetCraftingUiController().MoveInventoryToCraft(_item);
            CraftItemClick?.Invoke(item.prefab);
        }
        else if (UIManager.instance.GetCraftModeController().GetECraftModeType() == ECraftModeType.Equip)
        {
            if (_item.eItemType != EItemType.Equipment) return;
            UIManager.instance.GetCraftingUiController().MoveInventoryToEquip(_item);
        }

        if (_item.itemCount == 1)
            UIManager.instance.GetInventoryController().RemoveItem(_item);
        else
        {
            _item.itemCount--;
            UIManager.instance.GetInventoryController().UpdateSlot();
        }
    }
}