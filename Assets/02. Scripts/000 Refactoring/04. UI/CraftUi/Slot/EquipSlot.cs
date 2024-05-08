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

    public EquipSlot()
    {
        type = SlotType.EquipSlot;
    }
 
    public void ChangeSlotColor()
    {
        if (isLocked == true) slotImage.color = lockColor;
        else slotImage.color = normalColor;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (isLocked == true) return;

        App.Manager.UI.GetPanel<CraftPanel>().Equip.MoveEquipToInventory(item);

        HideItemInfo();
    }
}
