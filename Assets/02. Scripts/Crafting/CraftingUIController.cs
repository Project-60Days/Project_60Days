using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUiController : ControllerBase
{
    [SerializeField] Transform slotParent;
    [SerializeField] Sprite[] craftTypeImage;
    [SerializeField] ItemSO itemSO;

    ItemSlot[] slots;

    List<Transform> slotTransforms;
    List<Image> craftTypeImages;
    List<ItemBase> items;
    List<ItemCombineData> itemCombines;

    string[] combinationCodes = new string[9];

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
        slots = slotParent.GetComponentsInChildren<ItemSlot>();
        slotTransforms = new List<Transform>();
        craftTypeImages = new List<Image>();

        foreach (Transform child in slotParent)
        {
            slotTransforms.Add(child);
            craftTypeImages.Add(child.GetChild(1).GetComponent<Image>());
        }
    }

    void Start()
    {
        itemCombines = new List<ItemCombineData>();

        for (int i = 1001; i < 2000; i++)
        {
            App.instance.GetDataManager().itemCombineData.TryGetValue(i, out ItemCombineData itemData); //ItemCombineData내의 모든 값 itemComines 리스트에 추가

            if (itemData != null)
                itemCombines.Add(itemData);
            else
                break;
        }

        for (int i = 0; i < slots.Length; i++) //CraftBag 내의 slot 모두 초기화
        {
            slotTransforms[i].gameObject.SetActive(false);
            slots[i].item = null;
            slots[i].eSlotType = ESlotType.CraftingSlot;
            craftTypeImages[i].GetComponent<Image>().sprite = craftTypeImage[0];
        }
    }

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

    /// <summary>
    /// CraftBag 새로고침(?)
    /// </summary>
    public void FreshCraftingBag()
    {
        for (int i = 0; i < items.Count + 1; i++)
        {
            slotTransforms[i].gameObject.SetActive(false);
            slots[i].item = null;
            slots[i].eSlotType = ESlotType.CraftingSlot;
            craftTypeImages[i].GetComponent<Image>().sprite = craftTypeImage[0];
        }

        int j = 0;

        craftTypeImages[j].gameObject.SetActive(false);

        for (; j < items.Count; j++)
        {
            slotTransforms[j].gameObject.SetActive(true);
            slots[j].item = items[j];
        }

        for (; j < slots.Length; j++)
        {
            slotTransforms[j].gameObject.SetActive(false);
            slots[j].item = null;
        }
    }

    /// <summary>
    /// CraftBag에 아이템 추가
    /// </summary>
    /// <param name="_item"></param>
    public void CraftItem(ItemBase _item)
    {
        items.Add(_item);
        FreshCraftingBag();
        CombineItem();
        
    }

    /// <summary>
    /// Exit 버튼 눌렀을 때 인벤토리로 아이템 반환
    /// </summary>
    public void ReturnItem()
    {
        for (int i = 0; i < items.Count; i++)
        {
            UIManager.instance.GetInventoryController().AddItem(items[i]);
            slotTransforms[i].gameObject.SetActive(false);
            slots[i].item = null;
        }

        items.Clear();
    }

    /// <summary>
    /// 조합표 비교
    /// </summary>
    public void CombineItem()
    {
        int flag; // 0: 일치, 1: 불일치

        foreach(ItemCombineData combineData in itemCombines)
        {
            flag = 0;

            combinationCodes[0] = combineData.Material_1;
            combinationCodes[1] = combineData.Material_2;
            combinationCodes[2] = combineData.Material_3;
            combinationCodes[3] = combineData.Material_4;
            combinationCodes[4] = combineData.Material_5;
            combinationCodes[5] = combineData.Material_6;
            combinationCodes[6] = combineData.Material_7;
            combinationCodes[7] = combineData.Material_8;
            combinationCodes[8] = combineData.Result;

            for (int i = 0; i < items.Count; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    if (combinationCodes[k] == "1" || combinationCodes[k] == "-1") continue;
                    if (combinationCodes[k] == items[i].itemCode)
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
                //if (combinationCodes[8] == "ITEM_TIER_2_SIGNALLER" || combinationCodes[8] == "ITEM_TIER_2_RISISTOR") continue;
                Debug.Log(combinationCodes[8]);
                ItemBase item = CombineResultItem(combinationCodes[8]);
                AddCombineItem(item);
                break;
            }
        }
    }

    /// <summary>
    /// 조합 결과 아이템 ItemBase에서 검색 후 리턴
    /// </summary>
    public ItemBase CombineResultItem(string resultItemCode)
    {
        ItemBase resultItem;

        for (int i = 0; i < itemSO.items.Length; i++)
        {
            if (itemSO.items[i].itemCode == resultItemCode)
            {
                resultItem = itemSO.items[i];
                App.instance.GetDataManager().itemData.TryGetValue(resultItemCode, out ItemData itemData);
                resultItem.data = itemData;
                
                return resultItem;
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
        int slotIndex = items.Count;
        slotTransforms[slotIndex].gameObject.SetActive(true);
        slots[slotIndex].item = _item;
        craftTypeImages[slotIndex].GetComponent<Image>().sprite = craftTypeImage[1];
        slots[slotIndex].eSlotType = ESlotType.ResultSlot;
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

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemCode == itemCode)
                _cnt++;
        }

        if (_cnt == count)
            return true;
        else
            return false;
    }

    public void CraftToInventory(ItemSlot itemSlot)
    {
        items.Remove(itemSlot.item);
        UIManager.instance.GetInventoryController().AddItem(slots[items.Count].item);
        FreshCraftingBag();
        CombineItem();
    }

    public void ResultToInventory()
    {
        UIManager.instance.GetInventoryController().AddItem(slots[items.Count].item);
        slotTransforms[items.Count].gameObject.SetActive(false);
        slots[items.Count].item = null;
        items.Clear();
        FreshCraftingBag();
        CombineItem();
    }
}
