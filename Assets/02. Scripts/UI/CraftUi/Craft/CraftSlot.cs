using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CraftSlot : SlotBase
{
    public static Action CraftItemClick;

    public CraftSlot()
    {
        eSlotType = ESlotType.CraftingSlot;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        switch (eSlotType)
        {
            case ESlotType.CraftingSlot:
                UIManager.instance.GetCraftingUiController().MoveCraftToInventory(_item);
                break;
            case ESlotType.ResultSlot:
                UIManager.instance.GetCraftingUiController().MoveResultToInventory(_item);
                break;
        }

        CraftItemClick?.Invoke();
    }
}
