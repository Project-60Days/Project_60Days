using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipSlot : SlotBase
{
    [SerializeField] Image slotImage;

    public bool IsLocked { get; private set; } = false;

    public override void SetItem(ItemBase _item)
    {
        base.SetItem(_item);

        Item.Equip();
        IsLocked = !Item.Data.CanRemoveEquipment;
        ChangeSlotColor();
    }

    public override void ResetItem()
    {
        if (Item == null) return;

        Item.UnEquip();
        IsLocked = false;
        ChangeSlotColor();

        base.ResetItem();
    }

    private void ChangeSlotColor()
    {
        slotImage.color = IsLocked ? Color.gray : Color.white;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (IsLocked == true) return;

        base.OnPointerClick(eventData);

        bench.Equip.MoveEquipToInventory(Item);
    }
}
