using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CraftCtrl : ModeCtrl
{
    List<ItemBase> craftItems = new List<ItemBase>();

    ItemCombineData batteryCombine;

    [SerializeField] GameObject slotPrefab;

    public bool IsCombinedResult => slotParent.childCount >= 3;

    public override void Init()
    {
        base.Init();

        itemCombineData = App.Data.Game.itemCombineData.Values.ToList();

        batteryCombine = itemCombineData.Find(x => x.Result == "ITEM_BATTERY");
        itemCombineData.Remove(batteryCombine);
    }

    public override void InitSlots()
    {
        for (int i = 0; i < slotParent.childCount; i++)
            Destroy(slotParent.GetChild(i).gameObject);
    }

    public override void Exit()
    {
        for (int i = 0; i < craftItems.Count; i++)
            App.Manager.UI.GetPanel<InventoryPanel>().AddItem(craftItems[i]);

        InitSlots();
        craftItems.Clear();
    }

    public void UpdateCraft() //TODO
    {
        InitSlots();

        bool isFirst = true;
        for (int i = 0; i < craftItems.Count; i++)
        {
            GameObject obj = Instantiate(slotPrefab, slotParent);
            obj.GetComponentInChildren<CraftSlot>().item = craftItems[i];
            if (isFirst == true)
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

        ItemBase resultItem = null;
        var sortedItem = craftItems.OrderBy(item => item.data.Code).ToList();

        var match_1 = itemCombineData.Where(x => x.Material_1 == sortedItem[0].data.Code).ToList();
        if (match_1.Count == 0) return;
        var match_2 = match_1.Where(x => x.Material_2 == sortedItem[1].data.Code).ToList();
        if (match_2.Count == 0) return;

        if (craftItems.Count == 2)
        {
            var combine = match_2.FirstOrDefault(x => x.Material_3 == "-1");
            if (combine != null)
                resultItem = GetItemByCode(combine.Result);
        }
        else if (craftItems.Count == 3)
        {
            var combine = match_2.FirstOrDefault(x => x.Material_3 == sortedItem[2].data.Code);
            if (combine != null)
                resultItem = GetItemByCode(combine.Result);
        }

        if (resultItem != null && resultItem.isBlueprintOpen)
            AddCombineItem(resultItem);
    }

    /// <summary>
    /// 조합 결과 아이템 ItemBase에서 검색 후 리턴
    /// </summary>
    public ItemBase GetItemByCode(string _resultItemCode)
        => itemData.Find(x => x.data.Code == _resultItemCode);

    /// <summary>
    /// 조합 결과 아이템 CraftBag에 표시
    /// </summary>
    /// <param name="_item"></param>
    public void AddCombineItem(ItemBase _item)
    {
        GameObject obj = Instantiate(slotPrefab, slotParent);
        obj.GetComponentInChildren<CraftSlot>().item = _item;
        obj.GetComponentInChildren<CraftSlot>().eSlotType = ESlotType.ResultSlot;
        obj.GetComponentInChildren<TextMeshProUGUI>().text = "=";
    }

    public void AddBatteryCombine()
    {
        itemCombineData.Add(batteryCombine);
    }

    public void RemoveBatteryCombine()
    {
        itemCombineData.Remove(batteryCombine);
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





    /// <summary>
    /// 해당 아이템이 특정 갯수만큼 있는지 체크하는 함수
    /// </summary>
    /// <param name="itemCode"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public bool CheckCraftingItem(string _itemCode, int _count = 1) 
        => _count == craftItems.FindAll(x => x.data.Code == _itemCode).Count;
}
