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
                App.Manager.UI.GetCraftingUiController().MoveCraftToInventory(item);
                break;
            case ESlotType.ResultSlot:
                App.Manager.UI.GetCraftingUiController().MoveResultToInventory(item);
                App.Manager.Sound.PlaySFX("SFX_Crafting_Result");
                if (item.isMadeOnce == false)
                {
                    item.isMadeOnce = true;
                    App.Manager.UI.GetCraftModeController().UpdateBlueprint();
                }
                    
                break;
        }

        CraftItemClick?.Invoke();

        HideItemInfo();
    }
}
