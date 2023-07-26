using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class CraftingUIController : MonoBehaviour
{
    [SerializeField] Transform slotParent;
    [SerializeField] Sprite[] craftTypeImage;
    [SerializeField] GameObject inventoryUi;
    [SerializeField] ItemSO itemSO;

    private ItemSlot[] slots;
    public List<Transform> slotTransforms;
    public List<Image> craftTypeImages;

    InventoryPage inventoryPage;

    public List<ItemBase> items;
    public List<ItemCombineData> itemCombines;
    string[] combinationCodes = new string[9];

    [SerializeField] ItemBase tempItem;

    void Awake()
    {
        inventoryPage = GameObject.Find("Inventory").GetComponent<InventoryPage>();

        slots = slotParent.GetComponentsInChildren<ItemSlot>();
        for (int i = 0; i < slotParent.childCount; i++)
        {
            if (slotParent.GetChild(i))
            {
                slotTransforms.Add(slotParent.GetChild(i));
                craftTypeImages.Add(slotTransforms[i].GetChild(1).GetComponent<Image>());
            }
            
        }
    }

    void Start()
    {
        for(int i = 1001; i < 2000; i++)
        {
            DataManager.instance.itemCombineData.TryGetValue(i, out ItemCombineData itemData);

            if (itemData != null)
                itemCombines.Add(itemData);
            else
                break;
        }
       
        gameObject.SetActive(false);

        for (int i = 0; i < slots.Length; i++)
        {
            slotTransforms[i].gameObject.SetActive(false);
            slots[i].item = null;
            craftTypeImages[i].GetComponent<Image>().sprite = craftTypeImage[0];
        }
    }

    /// <summary>
    /// CraftBag 새로고침(?)
    /// </summary>
    public void FreshCraftingBag()
    {
        for (int i = 0; i < items.Count; i++)
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
        inventoryPage.RemoveItem(_item);
    }

    /// <summary>
    /// Exit 버튼 눌렀을 때 인벤토리로 아이템 반환
    /// </summary>
    public void ReturnItem()
    {
        gameObject.SetActive(false);
        inventoryUi.SetActive(false);

        for (int i = 0; i < items.Count; i++)
        {
            inventoryPage.AddItem(items[i]);
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
        for (int i = 0; i < itemCombines.Count; i++)
        {
            flag = 0;

            combinationCodes[0] = itemCombines[i].Material_1;
            combinationCodes[1] = itemCombines[i].Material_2;
            combinationCodes[2] = itemCombines[i].Material_3;
            combinationCodes[3] = itemCombines[i].Material_4;
            combinationCodes[4] = itemCombines[i].Material_5;
            combinationCodes[5] = itemCombines[i].Material_6;
            combinationCodes[6] = itemCombines[i].Material_7;
            combinationCodes[7] = itemCombines[i].Material_8;
            combinationCodes[8] = itemCombines[i].Result;

            for (int j = 0; j < items.Count; j++)
            {
                if (flag == 1) break;

                for (int k = 0; k < 8; k++)
                {
                    if (combinationCodes[k] == "1" || combinationCodes[k] == "-1") continue;
                    if (combinationCodes[k] != items[j].itemCode)
                    {
                        flag = 1;
                        break;
                    }
                    else
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
                ItemBase item = CombineResultItem(combinationCodes[8]);
                AddCombineItem(item);
                break;
            }
        }
    }

    /// <summary>
    /// 조합 결과 아이템 CraftBag에 표시
    /// </summary>
    public ItemBase CombineResultItem(string resultItemCode)
    {
        ItemBase resultItem;

        for (int i = 0; i < itemSO.items.Length; i++)
        {
            if (itemSO.items[i].itemCode == resultItemCode)
            {
                resultItem = itemSO.items[i];
                DataManager.instance.itemData.TryGetValue(resultItemCode, out ItemData itemData);
                resultItem.data = itemData;
                
                return resultItem;
            }
        }

        Debug.Log("아직 추가되지 않은 아이템");
        return tempItem;
    }

    public void AddCombineItem(ItemBase _item)
    {
        slotTransforms[items.Count + 1].gameObject.SetActive(true);
        slots[items.Count + 1].item = _item;
        craftTypeImages[items.Count + 1].GetComponent<Image>().sprite = craftTypeImage[1];
        slots[items.Count + 1].eSlotType = ESlotType.ResultSlot;
    }

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
        
    }

    public void ResultToInventory()
    {
        inventoryPage.AddItem(slots[items.Count + 1].item);
        slotTransforms[items.Count + 1].gameObject.SetActive(false);
        slots[items.Count + 1].item = null;
        items.Clear();
        FreshCraftingBag();
    }
}
