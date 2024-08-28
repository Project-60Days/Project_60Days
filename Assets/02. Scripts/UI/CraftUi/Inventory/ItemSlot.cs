using UnityEngine;
using UnityEngine.EventSystems;
using System;
using TMPro;

public class ItemSlot : SlotBase
{
    public int category;
    public static Action<Sprite> CraftItemClick;
    private TextMeshProUGUI countTMP;

    private void Awake()
    {
        type = SlotType.InventorySlot;
        countTMP = GetComponentInChildren<TextMeshProUGUI>();
    }

    public override void SetItem(ItemBase _item)
    {
        base.SetItem(_item);

        countTMP.text = item.itemCount.ToString();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (item.data.Code == "ITEM_NETWORKCHIP") return;

        if (App.Manager.UI.GetPanel<BenchPanel>().BenchMode == BenchType.Craft)
        {
            if (App.Manager.UI.GetPanel<BenchPanel>().Craft.IsCombinedResult) return;

            string sfxName = "SFX_Craft_" + item.data.Code;
            if (App.Manager.Sound.CheckSFXExist(sfxName) == true)
                App.Manager.Sound.PlaySFX(sfxName);
            else
                App.Manager.Sound.PlaySFX("SFX_Craft_Item");

            CraftItemClick?.Invoke(item.sprite);

            App.Manager.UI.GetPanel<BenchPanel>().Craft.MoveInventoryToCraft(item);

            HideItemInfo();
        }
        else if (App.Manager.UI.GetPanel<BenchPanel>().BenchMode == BenchType.Equip)
        {
            if (item.itemType == ItemType.Equipment)
            {
                if (App.Manager.UI.GetPanel<BenchPanel>().Equip.MoveInventoryToEquip(item) == true) 
                    HideItemInfo();
            }   
        }
    }
}