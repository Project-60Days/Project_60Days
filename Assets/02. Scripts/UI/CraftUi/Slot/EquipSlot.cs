using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipSlot : SlotBase
{
    [HideInInspector] public ItemType eItemType = ItemType.Equipment;
    [HideInInspector] public bool isLocked = false;
    [SerializeField] Image slotImage;

    Color normalColor = new Color(1f, 1f, 1f, 1f);
    Color lockColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    private void Start()
    {
        type = SlotType.EquipSlot;
    }

    public override void SetItem(ItemBase _item)
    {
        base.SetItem(_item);

        item.Equip();
        isLocked = !item.canRemoveEquipment;
        ChangeSlotColor();
    }

    public override void ResetItem()
    {
        base.ResetItem();

        item.UnEquip();
        isLocked = false;
        ChangeSlotColor();
    }

    public void ChangeSlotColor()
    {
        if (isLocked == true) slotImage.color = lockColor;
        else slotImage.color = normalColor;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (isLocked == true) return;

        App.Manager.UI.GetPanel<BenchPanel>().Equip.MoveEquipToInventory(item);

        HideItemInfo();
    }
}
