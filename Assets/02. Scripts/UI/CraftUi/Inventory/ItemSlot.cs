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

        if (App.Manager.UI.GetPanel<CraftPanel>().ModeType == CraftMode.Craft)
        {
            if (App.Manager.UI.GetPanel<CraftPanel>().Craft.IsCombinedResult) return;

            string sfxName = "SFX_Crafting_" + item.data.Code;
            if (App.Manager.Sound.CheckSFXExist(sfxName) == true)
                App.Manager.Sound.PlaySFX(sfxName);
            else
                App.Manager.Sound.PlaySFX("SFX_Crafting_Item");

            CraftItemClick?.Invoke(item.sprite);

            App.Manager.UI.GetPanel<CraftPanel>().Craft.MoveInventoryToCraft(item);

            HideItemInfo();
        }
        else if (App.Manager.UI.GetPanel<CraftPanel>().ModeType == CraftMode.Equip)
        {
            if (item.eItemType == EItemType.Equipment)
            {
                if (App.Manager.UI.GetPanel<CraftPanel>().Equip.MoveInventoryToEquip(item) == true) 
                    HideItemInfo();
            }   
        }
    }
}