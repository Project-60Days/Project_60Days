using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EquipCtrl : ModeCtrl
{
    [SerializeField] EquipSlot[] equipSlots;

    public override void InitSlots()
    {
        foreach (var slot in equipSlots)
            slot.item = null;
    }

    public override void Exit() { }
    bool AddEquip(ItemBase _item)
    {
        for (int i = 0; i < equipSlots.Length; i++)
        {
            if (equipSlots[i].equipType != _item.data.EquipType)
                continue;

            if (equipSlots[i].isLocked == true) return false;

            if (equipSlots[i].item != null)
            {
                equipSlots[i].item.UnEquip();
                App.Manager.UI.GetPanel<InventoryPanel>().AddItem(equipSlots[i].item);
            }

            App.Manager.UI.GetPanel<InventoryPanel>().RemoveItem(_item);
            equipSlots[i].item = _item;
            _item.Equip();
            if (_item.canRemoveEquipment == false)
            {
                equipSlots[i].isLocked = true;
                StartCoroutine(WaitUntilItemUsed(_item));
            }

            equipSlots[i].ChangeSlotColor();

            return true;
        }

        return false;
    }

    void RemoveEquip(ItemBase _item)
    {
        for (int i = 0; i < equipSlots.Length; i++)
        {
            if (equipSlots[i].equipType != _item.data.EquipType)
                continue;

            if (equipSlots[i].item != null)
            {
                equipSlots[i].item.UnEquip();
                App.Manager.UI.GetPanel<InventoryPanel>().AddItem(equipSlots[i].item);
            }

            equipSlots[i].item = null;

            break;
        }
    }

    IEnumerator WaitUntilItemUsed(ItemBase _item)
    {
        yield return new WaitUntil(() => _item.CheckMeetCondition());

    }

    public void EquipItemDayEvent()
    {
        for (int i = 0; i < equipSlots.Length; i++)
        {
            if (equipSlots[i].item != null)
            {
                equipSlots[i].item.DayEvent();
                if (equipSlots[i].item.CheckMeetCondition() == true)
                {
                    equipSlots[i].item = null;
                    equipSlots[i].isLocked = false;
                    equipSlots[i].ChangeSlotColor();
                }
            }

        }
    }

    public bool MoveInventoryToEquip(ItemBase _item)
    {
        return AddEquip(_item);
    }

    public void MoveEquipToInventory(ItemBase _item)
    {
        RemoveEquip(_item);
    }
}
