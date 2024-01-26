using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class BlueprintSlot : SlotBase
{
    public ItemBase bluePrintItem;
    [SerializeField] ItemBase unknownItem;
    public bool isAlreadyShowItem = false;
    [SerializeField] ItemBase[] parentItems;
    [SerializeField] Sprite lockSlot;
    [SerializeField] Sprite openSlot;

    public static Action<Sprite> CraftItemClick;

    public BlueprintSlot()
    {
        eSlotType = ESlotType.BlueprintSlot;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (item == unknownItem)
            return;

        UIManager.instance.GetCraftingUiController().ShowItemBlueprint(item);
        CraftItemClick?.Invoke(item.sprite);
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
    }

    void CheckIconShowCondition()
    {
        foreach (var item in parentItems)
        {
            if (item.isMadeOnce == false)
            {
                transform.parent.GetComponent<Image>().sprite = lockSlot;
                return;
            }    
        }

        SetIconShow();
    }


    public void SetIconShow()
    {
        item = bluePrintItem; //image.sprite = bluePrintItem.itemImage;
        transform.parent.GetComponent<Image>().sprite = openSlot;
        bluePrintItem.isBlueprintOpen = true;
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
