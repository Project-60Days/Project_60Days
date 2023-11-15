using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUiController : ControllerBase
{
    [Header ("Craft Mode")]
    [SerializeField] Transform craftSlotParent;
    [SerializeField] ItemSO itemSO;
    [SerializeField] GameObject craftSlotPrefab;

    List<ItemBase> craftItems = new List<ItemBase>();
    List<ItemCombineData> itemCombines = new List<ItemCombineData>();

    [Header("Equip Mode")]
    [SerializeField] Transform equipSlotParent;
    [SerializeField] EquipSlot[] equipSlots;

    [Header("Blueprint Mode")]
    [SerializeField] Transform blueprintSlotParent;
    [SerializeField] GameObject blueprintSlotPrefab;

    /// <summary>
    /// 아직 ItemSO에 추가되지 않은 아이템 조합 시에 생성될 임시 아이템
    /// </summary>
    [SerializeField] ItemBase tempItem;

    public override EControllerType GetControllerType()
    {
        return EControllerType.CRAFT;
    }





    void Start()
    {
        int i = 1001;
        while(true)
        {
            App.instance.GetDataManager().itemCombineData.TryGetValue(i, out ItemCombineData itemData); //ItemCombineData내의 모든 값 itemComines 리스트에 추가

            if (itemData == null) break;

            itemCombines.Add(itemData);
            i++;
        }

        InitCraftSlots();
        InitEquipSlots();
        InitBlueprintSlots();
    }





    public void InitCraftSlots()
    {
        for (int i = 0; i < craftSlotParent.childCount; i++)
            Destroy(craftSlotParent.GetChild(i).gameObject);
    }

    void InitEquipSlots()
    {
        foreach (var slot in equipSlots)
            slot.item = null;
    }

    void InitBlueprintSlots()
    {
        for (int i = 0; i < blueprintSlotParent.childCount; i++)
            Destroy(blueprintSlotParent.GetChild(i).gameObject);
    }





    #region Craft
    /// <summary>
    /// CraftBag 새로고침(?)
    /// </summary>
    public void UpdateCraft()
    {
        InitCraftSlots();

        bool isFirst = true;
        for(int i = 0; i < craftItems.Count; i++)
        {
            GameObject obj = Instantiate(craftSlotPrefab, craftSlotParent);
            obj.GetComponentInChildren<CraftSlot>().item = craftItems[i];
            if (isFirst == true)
            {
                obj.transform.GetChild(1).gameObject.SetActive(false);
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

        foreach(ItemCombineData combineData in itemCombines)
        {
            flag = 0;

            string[] combinationCodes = GetCombinationCodes(combineData);
         
            for (int i = 0; i < craftItems.Count; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    if (combinationCodes[k] == craftItems[i].English)
                    {
                        combinationCodes[k] = "-1";
                        break;  
                    }
                    if (k == 7) flag = 1;
                }
            }

            for (int k = 0; k < 8; k++)
            {
                if (combinationCodes[k] != "-1")
                {
                    flag = 1;
                    break;
                }
            }

            if (flag == 0)
            {
                ItemBase item = GetResultItemByItemCode(combinationCodes[8]);
                AddCombineItem(item);
                break;
            }
        }
    }

    string[] GetCombinationCodes(ItemCombineData _combineData)
    {
        string[] codes = new string[9];

        codes[0] = _combineData.Material_1;
        codes[1] = _combineData.Material_2;
        codes[2] = _combineData.Material_3;
        codes[3] = _combineData.Material_4;
        codes[4] = _combineData.Material_5;
        codes[5] = _combineData.Material_6;
        codes[6] = _combineData.Material_7;
        codes[7] = _combineData.Material_8;
        codes[8] = _combineData.Result;

        return codes;
    }





    /// <summary>
    /// 조합 결과 아이템 ItemBase에서 검색 후 리턴
    /// </summary>
    public ItemBase GetResultItemByItemCode(string _resultItemCode)
    {
        foreach(ItemBase item in itemSO.items)
        {
            if (item.English == _resultItemCode)
                return item;
        }

        Debug.Log("아직 추가되지 않은 아이템: " + _resultItemCode);
        return tempItem;
    }

    /// <summary>
    /// 조합 결과 아이템 CraftBag에 표시
    /// </summary>
    /// <param name="_item"></param>
    public void AddCombineItem(ItemBase _item)
    {
        GameObject obj = Instantiate(craftSlotPrefab, craftSlotParent);
        obj.GetComponentInChildren<CraftSlot>().item = _item;
        obj.GetComponentInChildren<CraftSlot>().eSlotType = ESlotType.ResultSlot;
    }





    /// <summary>
    /// CraftBag에 아이템 추가
    /// </summary>
    /// <param name="_item"></param>
    public void MoveInventoryToCraft(ItemBase _item)
    {
        UIManager.instance.GetInventoryController().RemoveItem(_item);
        craftItems.Add(_item);
        UpdateCraft();
    }

    public void MoveCraftToInventory(ItemBase _item)
    {
        UIManager.instance.GetInventoryController().AddItem(_item);
        craftItems.Remove(_item);
        UpdateCraft();
    }

    public void MoveResultToInventory(ItemBase _item)
    {
        UIManager.instance.GetInventoryController().AddItem(_item);
        craftItems.Clear();
        InitCraftSlots();
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
            if (craftItems[i].English == _itemCode)
                cnt++;
        }

            return (cnt == _count);
    }
    #endregion





    #region Equip
    void AddEquip(ItemBase _item)
    {
        for (int i = 0; i < equipSlots.Length; i++) 
        {
            if (equipSlots[i].equipType != _item.data.EquipType)
                continue;

            if (equipSlots[i].item != null)
                UIManager.instance.GetInventoryController().AddItem(equipSlots[i].item);

            equipSlots[i].item = _item;
        }
    }

    void RemoveEquip(ItemBase _item)
    {
        for (int i = 0; i < equipSlots.Length; i++)
        {
            if (equipSlots[i].equipType != _item.data.EquipType)
                continue;

            if (equipSlots[i].item != null)
                UIManager.instance.GetInventoryController().AddItem(equipSlots[i].item);

            equipSlots[i].item = null;
        }
    }

    public void MoveInventoryToEquip(ItemBase _item)
    {
        UIManager.instance.GetInventoryController().RemoveItem(_item);
        AddEquip(_item);
    }

    public void MoveEquipToInventory(ItemBase _item)
    {
        RemoveEquip(_item);
    }
    #endregion





    #region Blueprint
    public void ShowItemBlueprint(ItemBase _item)
    {
        InitBlueprintSlots();

        string[] blueprintCodes = new string[9];

        foreach (ItemCombineData combineData in itemCombines)
        {
            if (combineData.Result == _item.English)
            {
                blueprintCodes = GetCombinationCodes(combineData);
                Debug.Log(_item.English);
                break;
            }
        }

        foreach (string blueprintCode in blueprintCodes)
        {
            if (blueprintCode == null || blueprintCode == "-1") break;
            AddItemByItemCode(blueprintCode);
        }
    }

    void AddItemByItemCode(string _itemCode)
    {
        for (int i = 0; i < itemSO.items.Length; i++)
            if (itemSO.items[i].English == _itemCode)
                AddBlueprintItem(itemSO.items[i]);

        Debug.Log("아직 추가되지 않은 아이템: " + _itemCode);
        AddBlueprintItem(tempItem);
    }

    void AddBlueprintItem(ItemBase _item)
    {
        GameObject obj = Instantiate(blueprintSlotPrefab, blueprintSlotParent);
        obj.GetComponentInChildren<BlueprintSlot>().item = _item;
        obj.GetComponentInChildren<BlueprintSlot>().enabled = false;
    }
    #endregion





    /// <summary>
    /// Exit 버튼 눌렀을 때 인벤토리로 아이템 반환
    /// </summary>
    public void ExitUi()
    {
        ExitCraftBag();
        ExitEquipBag();
        ExitBlueprintBag();
    }

    public void ExitCraftBag()
    {
        for (int i = 0; i < craftItems.Count; i++)
            UIManager.instance.GetInventoryController().AddItem(craftItems[i]);

        InitCraftSlots();
        craftItems.Clear();
    }

    public void ExitEquipBag()
    {
        for (int i = 0; i < equipSlots.Length; i++) 
        {
            if (equipSlots[i].item == null) continue;
            UIManager.instance.GetInventoryController().AddItem(equipSlots[i].item);
            equipSlots[i].item = null;
        }
    }

    public void ExitBlueprintBag()
    {
        InitBlueprintSlots();
    }
}
