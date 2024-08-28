using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EquipCtrl : ModeCtrl
{
    [SerializeField] EquipSlot[] equipSlots;

    public override BenchType GetModeType() => BenchType.Equip;

    public override void InitSlots()
    {
        foreach (var slot in equipSlots)
        {
            slot.item = null;
        }
    }

    private bool AddEquip(ItemBase item)
    {
        var slot = FindEquipSlot(item);

        if (slot == null || slot.isLocked)
            return false;

        if (slot.item != null)
        {
            slot.item.UnEquip();
            App.Manager.UI.GetPanel<InventoryPanel>().AddItem(slot.item);
        }

        App.Manager.UI.GetPanel<InventoryPanel>().RemoveItem(item);
        slot.item = item;
        item.Equip();

        if (!item.canRemoveEquipment)
        {
            slot.isLocked = true;
            StartCoroutine(WaitUntilItemUsed(item, slot));
        }

        slot.ChangeSlotColor();

        return true;
    }

    private void RemoveEquip(ItemBase item)
    {
        var slot = FindEquipSlot(item);

        if (slot == null || slot.item == null)
            return;

        slot.item.UnEquip();
        App.Manager.UI.GetPanel<InventoryPanel>().AddItem(slot.item);
        slot.item = null;
        slot.isLocked = false;

        slot.ChangeSlotColor();
    }

    private EquipSlot FindEquipSlot(ItemBase item)
    {
        return equipSlots.FirstOrDefault(slot => (int)slot.type == item.data.EquipType);
    }

    private IEnumerator WaitUntilItemUsed(ItemBase item, EquipSlot slot)
    {
        yield return new WaitUntil(() => item.CheckMeetCondition());

        slot.item = null;
        slot.isLocked = false;
        slot.ChangeSlotColor();
    }

    public void EquipItemDayEvent()
    {
        foreach (var slot in equipSlots)
        {
            if (slot.item == null) continue;

            slot.item.DayEvent();

            if (slot.item.CheckMeetCondition())
            {
                slot.item = null;
                slot.isLocked = false;
                slot.ChangeSlotColor();
            }
        }
    }

    public bool MoveInventoryToEquip(ItemBase item)
    {
        return AddEquip(item);
    }

    public void MoveEquipToInventory(ItemBase item)
    {
        RemoveEquip(item);
    }
}