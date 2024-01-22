using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlueprintSlot : SlotBase
{
    public ItemBase bluePrintItem;
    [SerializeField] ItemBase unknownItem;
    public bool isAlreadyShowItem = false;
    [SerializeField] ItemBase[] parentItems;

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
        if (item == unknownItem)
            return;

        base.ShowItemInfo();
    }

    public void CheckShowCondition()
    {
        CheckItemShowCondition();
        CheckIconShowCondition();

        Debug.Log(bluePrintItem + " " + item);
    }

    void CheckIconShowCondition()
    {
        if (parentItems.Length == 0)
        {
            SetIconShow();
            return;
        }

        foreach (var item in parentItems)
        {
            if (item.isMadeOnce == false)
                return;
        }

        SetIconShow();
    }


    public void SetIconShow()
    {
        image.sprite = bluePrintItem.itemImage;
    }
    void CheckItemShowCondition()
    {
        if (isAlreadyShowItem == true)
            return;

        if (bluePrintItem.isMadeOnce == true)
        {
            item = bluePrintItem;
            isAlreadyShowItem = true;
        }
        else
            item = unknownItem;
    }

}
