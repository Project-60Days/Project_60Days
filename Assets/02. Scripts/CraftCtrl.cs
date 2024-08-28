using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class CraftCtrl : ModeCtrl, IListener
{
    public override BenchType GetModeType() => BenchType.Craft;

    [SerializeField] CraftSlot[] craftSlots;
    [SerializeField] CraftSlot resultSlot;

    private Dictionary<string, ItemCombineData> itemCombineDic;
    private List<ItemBase> craftItems = new(3);

    private InventoryPanel inventory;

    public bool IsCombinedResult => resultSlot.gameObject.activeSelf;

    private void Awake()
    {
        App.Manager.Event.AddListener(EventCode.TutorialEnd, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.TutorialEnd:
                var itemToRemove = itemCombineDic.FirstOrDefault(x => x.Value.Result == "ITEM_BATTERY");
                itemCombineDic.Remove(itemToRemove.Key);
                break;
        }
    }

    public override void Init()
    {
        base.Init();

        foreach (var combineData in App.Data.Game.itemCombineData.Values)
        {
            var sb = new StringBuilder();
            sb.Append(combineData.Material_1);
            sb.Append(combineData.Material_2);

            if (combineData.Material_3 != "-1")
            {
                sb.Append(combineData.Material_3);
            }

            string key = sb.ToString();

            itemCombineDic[key] = combineData;
        }

        inventory = App.Manager.UI.GetPanel<InventoryPanel>();
    }

    public override void Exit()
    {
        base.Exit();

        foreach (var item in craftItems)
        {
            inventory.AddItem(item);
        }

        ResetSlots();
        craftItems.Clear();
    }

    public override void ResetSlots()
    {
        foreach (var slot in craftSlots)
        {
            slot.ResetItem();
        }

        resultSlot.ResetItem();
    }

    public void UpdateSlots()
    {
        ResetSlots();

        for (int i = 0; i < craftItems.Count; i++)
        {
            craftSlots[i].SetItem(craftItems[i]);
        }
  
        CompareToCombineData();
    }

    public void CompareToCombineData()
    {
        if (craftItems.Count < 2) return;

        var sortedItems = craftItems.OrderBy(item => item.Code).ToList();
        var combinedKey = string.Concat(sortedItems.Select(x => x.data.Code));

        if (itemCombineDic.TryGetValue(combinedKey, out var combineData) && itemData[combineData.Result].isBlueprintOpen)
        {
            resultSlot.SetItem(itemData[combineData.Result]);
        }
    }

    /// <summary>
    /// CraftBag에 아이템 추가
    /// </summary>
    /// <param name="_item"></param>
    public void MoveInventoryToCraft(ItemBase _item)
    {
        inventory.RemoveItem(_item);
        craftItems.Add(_item);
        UpdateSlots();
    }

    public void MoveCraftToInventory(ItemBase _item)
    {
        inventory.AddItem(_item);
        craftItems.Remove(_item);
        UpdateSlots();
    }

    public void MoveResultToInventory(ItemBase _item)
    {
        inventory.AddItem(_item);
        craftItems.Clear();
        ResetSlots();
    }
}
