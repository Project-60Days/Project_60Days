using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlueprintSlot : SlotBase
{
    public BlueprintSlot()
    {
        eSlotType = ESlotType.BlueprintSlot;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.GetCraftingUiController().ShowItemBlueprint(_item);
    }
}
