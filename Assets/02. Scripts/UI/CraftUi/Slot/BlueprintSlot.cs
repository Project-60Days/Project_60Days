using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlueprintSlot : SlotBase
{
    public ItemBase bluePrintItem;
    public bool isReadyToShowIcon = false;
    public bool isAlreadyShow = false;
    [SerializeField] BlueprintSlot[] childSlot;

    void Start()
    {
        if (isReadyToShowIcon == true)
            item = bluePrintItem;
        else item = UIManager.instance.GetInventoryController().unknownItem;
    }

    public BlueprintSlot()
    {
        eSlotType = ESlotType.BlueprintSlot;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.GetCraftingUiController().ShowItemBlueprint(item);
    }

    public override void ShowItemInfo()
    {
        if (item.isMadeOnce == true)
            base.ShowItemInfo();
    }

    public void SetIconShow()
    {
        isReadyToShowIcon = true;
        item = bluePrintItem;
    }

    public void SetBlueprintShow()
    {
        if (item.isMadeOnce == true) 
        {
            isAlreadyShow = true;
            foreach (var child in childSlot)
                child.SetIconShow();
        }
    }
}
