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

        countTMP.text = Item.Data.Count.ToString();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (bench.BenchMode == BenchType.Craft)
        {
            if (bench.Craft.CanAddItem == false) return;

            CraftItemClick?.Invoke(Item.IllustSprite);

            DescriptionOff();

            bench.Craft.MoveInventoryToCraft(Item);

            string sfxName = "SFX_Craft_" + Item.Code;

            if (App.Manager.Sound.CheckSFXExist(sfxName) == true) 
            {
                App.Manager.Sound.PlaySFX(sfxName);
            }
            else
            {
                App.Manager.Sound.PlaySFX("SFX_Craft_Item");
            }
        }
        else if (bench.BenchMode == BenchType.Equip)
        {
            if (bench.Equip.MoveInventoryToEquip(Item) == true)
            {
                DescriptionOff();
            }
        }
    }
}