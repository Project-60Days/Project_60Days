using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CraftCtrl : ModeCtrl, IListener
{
    public override BenchType GetModeType() => BenchType.Craft;

    List<ItemBase> craftItems = new();

    [SerializeField] GameObject slotPrefab;
    [SerializeField] Transform slotParent;

    private Dictionary<string, ItemCombineData> itemCombineDic;

    public bool IsCombinedResult => slotParent.childCount > 3;

    private void Awake()
    {
        App.Manager.Event.AddListener(EventCode.TutorialEnd, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.TutorialEnd:
                itemCombineDic.Remove("ITEM_BATTERY");
                break;
        }
    }

    public override void Init()
    {
        base.Init();

        itemCombineDic = App.Data.Game.itemCombineData.Values.ToDictionary(x => x.Result);
    }

    public override void InitSlots()
    {
        for (int i = 0; i < slotParent.childCount; i++)
            Destroy(slotParent.GetChild(i).gameObject);
    }

    public override void Exit()
    {
        base.Exit();

        foreach (var item in craftItems)
        {
            App.Manager.UI.GetPanel<InventoryPanel>().AddItem(item);
        }

        InitSlots();
        craftItems.Clear();
    }

    public void UpdateCraft() //TODO
    {
        InitSlots();

        bool isFirst = true;
        foreach (var item in craftItems)
        {
            GameObject obj = Instantiate(slotPrefab, slotParent);
            var craftSlot = obj.GetComponentInChildren<CraftSlot>();
            craftSlot.item = item;

            if (isFirst)
            {
                obj.transform.GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);
                isFirst = false;
            }
        }
        CompareToCombineData();
    }

    public void CompareToCombineData()
    {
        if (craftItems.Count < 2) return;

        var sortedItems = craftItems.OrderBy(item => item.Code).ToList();

        var combine = itemCombineDic.Values
            .Where(x => x.Material_1 == sortedItems[0].data.Code && x.Material_2 == sortedItems[1].data.Code)
            .FirstOrDefault(x => craftItems.Count == 2 || x.Material_3 == sortedItems.ElementAtOrDefault(2)?.data.Code);

        if (combine != null && itemData[combine.Result].isBlueprintOpen)
        {
            AddCombineItem(itemData[combine.Result]);
        }
    }

    /// <summary>
    /// 조합 결과 아이템 CraftBag에 표시
    /// </summary>
    /// <param name="_item"></param>
    public void AddCombineItem(ItemBase _item)
    {
        GameObject obj = Instantiate(slotPrefab, slotParent);
        obj.GetComponentInChildren<CraftSlot>().item = _item;
        obj.GetComponentInChildren<CraftSlot>().type = SlotType.ResultSlot;
        obj.GetComponentInChildren<TextMeshProUGUI>().text = "=";
    }

    /// <summary>
    /// CraftBag에 아이템 추가
    /// </summary>
    /// <param name="_item"></param>
    public void MoveInventoryToCraft(ItemBase _item)
    {
        App.Manager.UI.GetPanel<InventoryPanel>().RemoveItem(_item);
        craftItems.Add(_item);
        UpdateCraft();
    }

    public void MoveCraftToInventory(ItemBase _item)
    {
        App.Manager.UI.GetPanel<InventoryPanel>().AddItem(_item);
        craftItems.Remove(_item);
        UpdateCraft();
    }

    public void MoveResultToInventory(ItemBase _item)
    {
        App.Manager.UI.GetPanel<InventoryPanel>().AddItem(_item);
        craftItems.Clear();
        InitSlots();
    }
}
