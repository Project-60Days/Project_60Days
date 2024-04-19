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
                UIManager.instance.GetCraftingUiController().MoveCraftToInventory(item);
                break;
            case ESlotType.ResultSlot:
                UIManager.instance.GetCraftingUiController().MoveResultToInventory(item);
                App.Manager.Sound.PlaySFX("SFX_Crafting_Result");
                if (item.isMadeOnce == false)
                {
                    item.isMadeOnce = true;
                    UIManager.instance.GetCraftModeController().UpdateBlueprint();
                }
                    
                break;
        }

        CraftItemClick?.Invoke();

        HideItemInfo();
    }
}
