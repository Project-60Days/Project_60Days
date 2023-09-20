using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class CraftingUiController : ControllerBase
{
    [Header ("Craft Mode")]
    [SerializeField] Transform craftSlotParent;
    [SerializeField] Sprite[] craftTypeImage;
    [SerializeField] ItemSO itemSO;
    [SerializeField] GameObject craftSlotPrefab;

    List<ItemBase> craftItems;
    List<ItemCombineData> itemCombines;

    string[] combinationCodes = new string[9];

    /// <summary>
    /// 아직 ItemSO에 추가되지 않은 아이템 조합 시에 생성될 임시 아이템
    /// </summary>
    [SerializeField] ItemBase tempItem;

    [Header("Equip Mode")]
    [SerializeField] Transform equipSlotParent;
    EquipSlot[] equipSlots;
    public List<ItemBase> equipItems;





    public override EControllerType GetControllerType()
    {
        return EControllerType.CRAFT;
    }





    void Start()
    {
        craftItems = new List<ItemBase>();
        equipItems = new List<ItemBase>();
        itemCombines = new List<ItemCombineData>();

        int i = 1001;
        while(true)
        {
            App.instance.GetDataManager().itemCombineData.TryGetValue(i, out ItemCombineData itemData); //ItemCombineData내의 모든 값 itemComines 리스트에 추가

            if (itemData == null) break;

            itemCombines.Add(itemData);
            i++;
        }

        equipSlots = equipSlotParent.GetComponentsInChildren<EquipSlot>();

        InitCraftSlots();
        InitEquipSlots();
    }





    #region Craft
    public void InitCraftSlots()
    {
        for(int i = 0; i < craftSlotParent.childCount; i++)
        {
            Destroy(craftSlotParent.GetChild(i).gameObject);
        }
    }





    #region temp
    /// <summary>
    /// 시연회용 임시 함수(맞나?)
    /// </summary>
    void Update()
    {
        InputKey();
    }

    /// <summary>
    /// 정다은이 생성한 함수가 아닙니다.. P키를 누르면 아이템이 추가되는건가 보네요~
    /// </summary>
    private void InputKey()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            UIManager.instance.GetInventoryController().AddItem(itemSO.items[0]);
            UIManager.instance.GetInventoryController().AddItem(itemSO.items[1]);
            UIManager.instance.GetInventoryController().AddItem(itemSO.items[4]);
        }
    }
    #endregion





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
            if (isFirst)
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

            GetCombinationCodes(combineData);

            for (int i = 0; i < craftItems.Count; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    if (combinationCodes[k] == "1" || combinationCodes[k] == "-1") continue;
                    if (combinationCodes[k] == craftItems[i].itemCode)
                    {
                        combinationCodes[k] = "1";
                        break;
                    }
                }
            }

            for (int k = 0; k < 8; k++)
            {
                if (combinationCodes[k] == "1" || combinationCodes[k] == "-1") continue;
                else
                {
                    flag = 1; break;
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

    void GetCombinationCodes(ItemCombineData combineData)
    {
        combinationCodes[0] = combineData.Material_1;
        combinationCodes[1] = combineData.Material_2;
        combinationCodes[2] = combineData.Material_3;
        combinationCodes[3] = combineData.Material_4;
        combinationCodes[4] = combineData.Material_5;
        combinationCodes[5] = combineData.Material_6;
        combinationCodes[6] = combineData.Material_7;
        combinationCodes[7] = combineData.Material_8;
        combinationCodes[8] = combineData.Result;
    }





    /// <summary>
    /// 조합 결과 아이템 ItemBase에서 검색 후 리턴
    /// </summary>
    public ItemBase GetResultItemByItemCode(string resultItemCode)
    {
        foreach(ItemBase item in itemSO.items)
        {
            if (item.itemCode == resultItemCode)
            {
                return item;
            }
        }

        Debug.Log("아직 추가되지 않은 아이템");
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
        obj.transform.GetChild(1).GetComponent<Image>().sprite = craftTypeImage[1];
    }





    /// <summary>
    /// CraftBag에 아이템 추가
    /// </summary>
    /// <param name="_item"></param>
    public void MoveInventoryToCraft(ItemBase _item)
    {
        craftItems.Add(_item);
        UpdateCraft();
    }

    public void MoveCraftToInventory(ItemBase _item)
    {
        craftItems.Remove(_item);
        UpdateCraft();
    }

    public void MoveResultToInventory()
    {
        craftItems.Clear();
        InitCraftSlots();
    }





    /// <summary>
    /// 해당 아이템이 특정 갯수만큼 있는지 체크하는 함수
    /// </summary>
    /// <param name="itemCode"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public bool CheckCraftingItem(string itemCode, int count = 1)
    {
        int _cnt = 0;

        for (int i = 0; i < craftItems.Count; i++)
        {
            if (craftItems[i].itemCode == itemCode)
                _cnt++;
        }

        if (_cnt == count)
            return true;
        else
            return false;
    }
    #endregion





    #region Equip
    void InitEquipSlots()
    {
        foreach (var slot in equipSlots) 
        {
            slot.item = null;
        }
    }

    void UpdateEquip()
    {
        if (equipItems.Count == 4)
        {
            UIManager.instance.GetInventoryController().AddItem(equipSlots[0].item);
            equipItems.RemoveAt(0);
        }

        InitEquipSlots();

        for (int i = 0; i < equipItems.Count; i++) 
        {
            equipSlots[i].item = equipItems[i];
        }
    }

    public void MoveInventoryToEquip(ItemBase _item)
    {
        equipItems.Add(_item);
        UpdateEquip();
    }

    public void MoveEquipToInventory(ItemBase _item)
    {
        equipItems.Remove(_item);
        UpdateEquip();
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
        {
            UIManager.instance.GetInventoryController().AddItem(craftItems[i]);
        }

        InitCraftSlots();
        craftItems.Clear();
    }

    public void ExitEquipBag()
    {
        foreach (var slot in equipSlots) 
        {
            if (slot.item == null) continue;
            UIManager.instance.GetInventoryController().AddItem(slot.item);
            slot.item = null;
        }
    }

    public void ExitBlueprintBag()
    {

    }
}
