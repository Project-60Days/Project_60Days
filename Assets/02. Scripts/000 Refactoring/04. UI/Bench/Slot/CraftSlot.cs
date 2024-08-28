using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CraftSlot : SlotBase
{
    public static Action CraftItemClick;

    [SerializeField] SlotType slotType;

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        switch (slotType)
        {
            case SlotType.CraftingSlot:
                bench.Craft.MoveCraftToInventory(Item);
                break;
            case SlotType.ResultSlot:
                bench.Craft.MoveResultToInventory(Item);

                App.Manager.Sound.PlaySFX("SFX_Craft_Result");

                Item.isMadeOnce = true;
                break;
        }

        CraftItemClick?.Invoke();
    }
}
