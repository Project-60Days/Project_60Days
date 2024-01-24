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
        if (UIManager.instance.GetCraftModeController().eCraftModeType == ECraftModeType.Craft)
        {
            if (UIManager.instance.GetCraftingUiController().isMoreThanThree() == true) return;

            string sfxName = "SFX_Crafting_" + item.data.Code;
            if (App.instance.GetSoundManager().CheckSFXExist(sfxName) == true)
                App.instance.GetSoundManager().PlaySFX(sfxName);
            else
                App.instance.GetSoundManager().PlaySFX("SFX_Crafting_Item");

            CraftItemClick?.Invoke(item.sprite);

            UIManager.instance.GetCraftingUiController().MoveInventoryToCraft(item);

            HideItemInfo();
        }
        else if (UIManager.instance.GetCraftModeController().eCraftModeType == ECraftModeType.Equip)
        {
            if (item.eItemType == EItemType.Equipment)
            {
                UIManager.instance.GetCraftingUiController().MoveInventoryToEquip(item);

                HideItemInfo();
            }   
        }
    }
}