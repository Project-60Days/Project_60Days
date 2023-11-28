using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class ItemSlot : SlotBase
{
    public static Action<GameObject> CraftItemClick;

    public int category;

    public ItemSlot()
    {
        eSlotType = ESlotType.InventorySlot;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {        
        if (UIManager.instance.GetCraftModeController().eCraftModeType == ECraftModeType.Craft)
        {
            if (UIManager.instance.GetCraftingUiController().isMoreThanThree() == true) return;

            HideItemInfo();

            UIManager.instance.GetCraftingUiController().MoveInventoryToCraft(item);

            string sfxName = "SFX_Crafting_" + item.data.Code;
            if (App.instance.GetSoundManager().CheckSFXExist(sfxName) == true)
                App.instance.GetSoundManager().PlaySFX(sfxName);
            else
                App.instance.GetSoundManager().PlaySFX("SFX_Crafting_Item");

            //CraftItemClick?.Invoke(item.prefab);
        }
        else if (UIManager.instance.GetCraftModeController().eCraftModeType == ECraftModeType.Equip)
        {
            if (item.eItemType == EItemType.Equipment || item.eItemType == EItemType.Both)
            {
                HideItemInfo();

                UIManager.instance.GetCraftingUiController().MoveInventoryToEquip(item);
            }   
        }
    }
}