using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryPanel : UIBase, IListener
{
    private Dictionary<string, ItemBase> itemBaseDic;
    private List<ItemBase> items = new();

    private List<InventorySlot>[] slots = new List<InventorySlot>[6];
    private int[] counts;

    private void Awake()
    {
        App.Manager.Event.AddListener(EventCode.TutorialStart, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.TutorialStart:
                AddItemByItemCode("ITEM_PLASMA", "ITEM_CARBON", "ITEM_STEEL"); 
                break;
        }
    }

    #region Override
    public override void Init()
    {
        itemBaseDic = App.Data.Game.ITEM.ToDictionary(item => item.Code);

        counts = new int[6];

        for (int i = 0; i < 6; i++)
        {
            slots[i] = new();
        }

        foreach (Transform child in transform)
        {
            var slot = child.GetComponent<InventorySlot>();
            slots[slot.category].Add(slot);
        }

        ResetAllSlots();
    }
    #endregion

    private void ResetAllSlots()
    {
        foreach (var slotList in slots)
        {
            foreach (var slot in slotList)
            {
                slot.ResetItem();
            }
        }

        for (int i = 0; i < counts.Length; i++)
        {
            counts[i] = 0;
        }
    }

    private void ResetSlots(int category)
    {
        foreach (var slot in slots[category])
        {
            slot.ResetItem();
        }

        counts[category] = 0;
    }

    private void UpdateSlots(int category)
    {
        ResetSlots(category);

        foreach (var item in items.Where(item => item.Data.Category == category))
        {
            if (counts[category] < slots[category].Count)
            {
                var currentSlot = slots[category][counts[category]];
                currentSlot.SetItem(item);
                counts[category]++;
            }
            else
            {
                Debug.LogWarning($"슬롯이 부족합니다: 카테고리 {category}");
            }
        }

        App.Manager.Event.PostEvent(EventCode.ItemUpdate, this);
    }

    #region Add Item
    public void AddItem(ItemBase _item)
    {
        if (_item.Data.Count == 0)
        {
            items.Add(_item);
        }

        _item.Data.Count++;

        UpdateSlots(_item.Data.Category);
    }

    public void AddItemByItemCode(params string[] _itemCode)
    {
        foreach (var code in _itemCode) 
        {
            if (itemBaseDic.TryGetValue(code, out var item))
            {
                AddItem(item);
            }
        }
    }
    #endregion

    #region Remove Item
    public void RemoveItem(ItemBase _item)
    {
        _item.Data.Count--;

        if (_item.Data.Count == 0)
        {
            items.Remove(_item);
        }

        UpdateSlots(_item.Data.Category);
    }

    public void RemoveItemByCode(string _itemCode)
    {
        if (itemBaseDic.TryGetValue(_itemCode, out var item))
        {
            RemoveItem(item);
        }
    }
    
    public void RemoveRandomItem()
    {
        if (items.Count == 0) return;

        ItemBase itemToRemove;
        do
        {
            itemToRemove = items[Random.Range(0, items.Count)];
        } while (itemToRemove.Code == "ITEM_NETWORKCHIP");

        App.Manager.UI.GetPanel<PagePanel>().SetCurrResource(itemToRemove);
        App.Manager.UI.GetPanel<PagePanel>().SetResultPage("LOOSE_RESOURCE", false);

        RemoveItem(itemToRemove);
    }
    #endregion

    public bool CheckItemExist(string _itemCode)
        => items.Exists(x => x.Code == _itemCode);

    #region Cheat Key
    void Update()
    {
        InputKey();
    }

    private void InputKey()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var item in itemBaseDic.Values)
            {
                if (++counts[item.Data.Category] > slots[item.Data.Category].Count) return;
                AddItem(item);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            AddItemByItemCode("ITEM_EXPLORER");
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            AddItemByItemCode("ITEM_DISRUPTOR");
        }
    }
    #endregion
}
