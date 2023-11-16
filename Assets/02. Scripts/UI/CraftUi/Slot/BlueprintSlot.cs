using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlueprintSlot : SlotBase
{
    public ItemBase bluePrintItem;

    void Start()
    {
        item = bluePrintItem;
    }

    public BlueprintSlot()
    {
        eSlotType = ESlotType.BlueprintSlot;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.GetCraftingUiController().ShowItemBlueprint(item);
    }
}
