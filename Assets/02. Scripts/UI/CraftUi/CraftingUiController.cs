using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    [HideInInspector] public Transform blueprintSlotParent;
    [SerializeField] GameObject blueprintSlotPrefab;

    [Header("Hologram")]
    [SerializeField] GameObject hologramBack;

    ItemCombineData batteryCombine;

    /// <summary>
    /// 아직 ItemSO에 추가되지 않은 아이템 조합 시에 생성될 임시 아이템
    /// </summary>
    [SerializeField] ItemBase tempItem;

    public override EControllerType GetControllerType()
    {
        return EControllerType.CRAFT;
    }





    void Awake()
    {
        int i = 1001;
        while(true)
        {
            App.instance.GetDataManager().itemCombineData.TryGetValue(i, out ItemCombineData itemData); //ItemCombineData내의 모든 값 itemComines 리스트에 추가

            if (itemData == null) break;

            if (itemData.Result == "ITEM_BATTERY")
            {
                batteryCombine = itemData;
                i++;
                continue;
            }

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





    public void TurnOnHologram()
    {
        hologramBack.SetActive(true);
    } 

    public void TurnOffHologram()
    {
        hologramBack.SetActive(false);
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

        foreach(ItemCombineData combineData in itemCombines)
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
                AddCombineItem(item);
                break;
            }
        }
    }

    string[] GetCombinationCodes(ItemCombineData _combineData)
    {
        string[] codes = new string[4];

        codes[0] = _combineData.Material_1;
        codes[1] = _combineData.Material_2;
        codes[2] = _combineData.Material_3;
        codes[3] = _combineData.Result;

        return codes;
    }





    /// <summary>
    /// 조합 결과 아이템 ItemBase에서 검색 후 리턴
    /// </summary>
    public ItemBase GetResultItemByItemCode(string _resultItemCode)
    {
        foreach(ItemBase item in itemSO.items)
        {
            if (item.data.Code == _resultItemCode)
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
        obj.GetComponentInChildren<TextMeshProUGUI>().text = "=";
    }

    public void AddBatteryCombine()
    {
        itemCombines.Add(batteryCombine);
    }

    public void RemoveBatteryCombine()
    {
        itemCombines.Remove(batteryCombine);
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

        string[] blueprintCodes = GetItemCombineCodes(_item);
        if (blueprintCodes == null) return;

        for (int i = 0; i < blueprintCodes.Length - 1; i++)
        {
            if (blueprintCodes[i] == "-1") break;
            AddItemByItemCode(blueprintCodes[i]);
        }
    }

    public string[] GetItemCombineCodes(ItemBase _item)
    {
        foreach (ItemCombineData combineData in itemCombines)
        {
            if (combineData.Result == _item.data.Code)
                return GetCombinationCodes(combineData);
        }
            
        return null;
    }

    void AddItemByItemCode(string _itemCode)
    {
        for (int i = 0; i < itemSO.items.Length; i++)
            if (itemSO.items[i].data.Code == _itemCode)
            {
                AddBlueprintItem(itemSO.items[i]);
                return;
            }
                
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




    public void EnterUi()
    {
        UIManager.instance.GetCraftModeController().SetCraftActive();
        UIManager.instance.GetItemInfoController().HideInfo();
    }

    /// <summary>
    /// Exit 버튼 눌렀을 때 인벤토리로 아이템 반환
    /// </summary>
    public void ExitUi()
    {
        ExitCraftBag();
        ExitBlueprintBag();
    }

    public void ExitCraftBag()
    {
        for (int i = 0; i < craftItems.Count; i++)
            UIManager.instance.GetInventoryController().AddItem(craftItems[i]);

        InitCraftSlots();
        craftItems.Clear();
    }

    public void ExitBlueprintBag()
    {
        InitBlueprintSlots();
    }
}
