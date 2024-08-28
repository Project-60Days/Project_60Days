using UnityEngine;


public class EquipCtrl : ModeCtrl, IListener
{
    public override BenchType GetModeType() => BenchType.Equip;

    [SerializeField] EquipSlot[] equipSlots;

    private InventoryPanel inventory;

    private void Awake()
    {
        App.Manager.Event.AddListener(EventCode.NextDayMiddle, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.NextDayMiddle:
                CheckItemUsed();
                break;
        }
    }

    public override void Init()
    {
        inventory = App.Manager.UI.GetPanel<InventoryPanel>();
    }

    public override void ResetSlots()
    {
        foreach (var slot in equipSlots)
        {
            slot.ResetItem();
        }
    }

    private bool AddEquip(ItemBase item)
    {
        if (item.data.EquipType >= equipSlots.Length)
        {
            Debug.LogWarning($"EquipType {item.data.EquipType} is out of range for equipSlots.");
            return false;
        }

        var slot = equipSlots[item.data.EquipType];

        if (slot.isLocked) return false;

        if (slot.item != null)
        {
            inventory.AddItem(slot.item);
            slot.ResetItem();
        }

        inventory.RemoveItem(item);
        slot.SetItem(item);

        return true;
    }

    private void RemoveEquip(ItemBase item)
    {
        var slot = equipSlots[item.data.EquipType];

        if (slot.item == null) return;
        
        inventory.AddItem(slot.item);
        slot.ResetItem();
    }

    private void CheckItemUsed()
    {
        foreach (var slot in equipSlots)
        {
            if (slot.item == null) continue;

            slot.item.DayEvent();

            if (slot.item.CheckMeetCondition())
            {
                slot.ResetItem();
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