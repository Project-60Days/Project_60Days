using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class ItemSlot : SlotBase
{
    public int category;
    public static Action<Sprite> CraftItemClick;

    public ItemSlot()
    {
        eSlotType = ESlotType.InventorySlot;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (item.data.Code == "ITEM_NETWORKCHIP") return;

        if (UIManager.instance.GetCraftModeController().eCraftModeType == ECraftModeType.Craft)
        {
            if (UIManager.instance.GetCraftingUiController().isMoreThanThree() == true) return;

            string sfxName = "SFX_Crafting_" + item.data.Code;
            if (App.Manager.Sound.CheckSFXExist(sfxName) == true)
                App.Manager.Sound.PlaySFX(sfxName);
            else
                App.Manager.Sound.PlaySFX("SFX_Crafting_Item");

            CraftItemClick?.Invoke(item.sprite);

            UIManager.instance.GetCraftingUiController().MoveInventoryToCraft(item);

            HideItemInfo();
        }
        else if (UIManager.instance.GetCraftModeController().eCraftModeType == ECraftModeType.Equip)
        {
            if (item.eItemType == EItemType.Equipment)
            {
                if (UIManager.instance.GetCraftingUiController().MoveInventoryToEquip(item) == true) 
                    HideItemInfo();
            }   
        }
    }
}