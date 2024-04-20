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

    public void UpdateCraft()
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





    /// <summary>
    /// 조합표와 비교
    /// </summary>
    public void CompareToCombineData()
    {
        int flag; // 0: 일치, 1: 불일치

        foreach (ItemCombineData combineData in itemCombineData)
        {
            flag = 0;

            string[] combinationCodes = GetCombinationCodes(combineData);

            for (int i = 0; i < craftItems.Count; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    if (combinationCodes[k] == craftItems[i].data.Code)
                    {
                        combinationCodes[k] = "-1";
                        break;
                    }
                    if (k == 2) flag = 1;
                }
            }

            for (int k = 0; k < 3; k++)
            {
                if (combinationCodes[k] != "-1")
                {
                    flag = 1;
                    break;
                }
            }

            if (flag == 0)
            {
                ItemBase item = GetResultItemByItemCode(combinationCodes[3]);
                if (item.isBlueprintOpen == false)
                    return;
                AddCombineItem(item);
                break;
            }
        }
    }





    /// <summary>
    /// 조합 결과 아이템 ItemBase에서 검색 후 리턴
    /// </summary>
    public ItemBase GetResultItemByItemCode(string _resultItemCode)
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
    {
        int cnt = 0;

        for (int i = 0; i < craftItems.Count; i++)
        {
            if (craftItems[i].data.Code == _itemCode)
                cnt++;
        }

        return (cnt == _count);
    }

    public bool isMoreThanThree()
    {
        if (craftItems.Count >= 3) return true;
        else return false;
    }
}
