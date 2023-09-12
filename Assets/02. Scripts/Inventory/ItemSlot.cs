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
        if (UIManager.instance.GetCraftModeController().GetECraftModeType() == ECraftModeType.Craft)
        {
            CraftItemClick?.Invoke(_item.prefab);
            UIManager.instance.GetCraftingUiController().MoveInventoryToCraft(_item);
            if (_item.itemCount == 1)
            {
                UIManager.instance.GetInventoryController().RemoveItem(_item);
            }
            else
            {
                _item.itemCount--;
                UIManager.instance.GetInventoryController().UpdateSlot();
            }
        }
        else if (UIManager.instance.GetCraftModeController().GetECraftModeType() == ECraftModeType.Equip)
        {
            if (_item.eItemType != EItemType.Equipment) return;
            UIManager.instance.GetCraftingUiController().MoveInventoryToEquip(_item);
        }
    }
}