using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class BlueprintSlot : SlotBase
{
    public static Action<Sprite> CraftItemClick;

    [SerializeField] ItemBase bluePrintItem;
    [SerializeField] ItemBase[] parentItems;
    [SerializeField] Sprite openSlot;

    private Image parentImg;

    private void Awake()
    {
        parentImg = transform.parent.GetComponent<Image>();

        var unknownItem = App.Data.Game.ITEM.Find(x => x.Code == "ITEM_UNKNOWN");
        SetItem(unknownItem);
    }

    protected override void DescriptionOn()
    {
        if (Item.Code.Equals("ITEM_UNKNOWN")) return;

        base.DescriptionOn();
    }

    public void CheckShowCondition()
    {
        foreach (var item in parentItems)
        {
            if (item.isMadeOnce == false) return;
        }

        SetIconShow();
    }

    private void SetIconShow()
    {
        SetItem(bluePrintItem);

        parentImg.sprite = openSlot;
        bluePrintItem.isBlueprintOpen = true;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (Item.Code.Equals("ITEM_UNKNOWN")) return;

        base.OnPointerClick(eventData);

        bench.Blueprint.UpdateSlots(Item);

        CraftItemClick?.Invoke(Item.sprite);
    }
}
