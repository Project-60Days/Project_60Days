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
        if (item.Data.EquipType >= equipSlots.Length) return false;

        var slot = equipSlots[item.Data.EquipType];

        if (slot.IsLocked) return false;

        if (slot.Item != null)
        {
            inventory.AddItem(slot.Item);
            slot.ResetItem();
        }

        inventory.RemoveItem(item);
        slot.SetItem(item);

        return true;
    }

    private void RemoveEquip(ItemBase item)
    {
        var slot = equipSlots[item.Data.EquipType];

        if (slot.Item == null) return;
        
        inventory.AddItem(slot.Item);
        slot.ResetItem();
    }

    private void CheckItemUsed()
    {
        foreach (var slot in equipSlots)
        {
            if (slot.Item == null) continue;

            slot.Item.DayEvent();

            if (slot.Item.CheckMeetCondition())
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