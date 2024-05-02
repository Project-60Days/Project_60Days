using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CraftSlot : SlotBase
{
    public static Action CraftItemClick;

    public CraftSlot()
    {
        type = SlotType.CraftingSlot;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        switch (type)
        {
            case SlotType.CraftingSlot:
                App.Manager.UI.GetPanel<CraftPanel>().Craft.MoveCraftToInventory(item);
                break;
            case SlotType.ResultSlot:
                App.Manager.UI.GetPanel<CraftPanel>().Craft.MoveResultToInventory(item);
                App.Manager.Sound.PlaySFX("SFX_Crafting_Result");
                if (item.isMadeOnce == false)
                {
                    item.isMadeOnce = true;
                }
                    
                break;
        }

        CraftItemClick?.Invoke();

        HideItemInfo();
    }
}
