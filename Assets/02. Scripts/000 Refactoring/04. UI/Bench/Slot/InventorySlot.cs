using UnityEngine;
using UnityEngine.EventSystems;
using System;
using TMPro;

public class InventorySlot : SlotBase
{
    public static Action<Sprite> CraftItemClick;

    public int category;

    private TextMeshProUGUI countTMP;

    private void Awake()
    {
        countTMP = GetComponentInChildren<TextMeshProUGUI>();
    }

    public override void SetItem(ItemBase _item)
    {
        base.SetItem(_item);

        countTMP.text = Item.itemCount.ToString();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (benchPanel.BenchMode == BenchType.Craft)
        {
            if (benchPanel.Craft.CanAddItem == false) return;

            HideItemInfo();

            benchPanel.Craft.MoveInventoryToCraft(Item);

            string sfxName = "SFX_Craft_" + Item.Code;

            if (App.Manager.Sound.CheckSFXExist(sfxName) == true) 
            {
                App.Manager.Sound.PlaySFX(sfxName);
            }
            else
            {
                App.Manager.Sound.PlaySFX("SFX_Craft_Item");
            }
              
            CraftItemClick?.Invoke(Item.sprite);
        }
        else if (benchPanel.BenchMode == BenchType.Equip)
        {
            if (Item.itemType == ItemType.Equipment)
            {
                if (benchPanel.Equip.MoveInventoryToEquip(Item) == true)
                {
                    HideItemInfo();
                }
            }   
        }
    }
}