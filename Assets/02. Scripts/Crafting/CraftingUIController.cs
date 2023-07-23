using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUIController : MonoBehaviour
{
    [SerializeField] Transform slotParent;
    //[SerializeField] Sprite[] craftTypeImage;
    [SerializeField] GameObject inventoryUi;

    private ItemSlot[] slots;
    public List<Transform> slotTransforms;

    InventoryPage inventoryPage;

    public List<ItemBase> items;
    //private List<Image> craftTypeImages;
    public List<ItemCombineData> itemCombines;

    void Awake()
    {
        inventoryPage = GameObject.Find("Inventory").GetComponent<InventoryPage>();

        slots = slotParent.GetComponentsInChildren<ItemSlot>();
        for (int i = 0; i < slotParent.childCount; i++)
        {
            if (slotParent.GetChild(i))
                slotTransforms.Add(slotParent.GetChild(i));
            //craftTypeImages.Add(slotTransforms[i].GetChild(1).GetComponent<Image>());
        }
    }

    void Start()
    {
        for(int i = 0; i < 1050; i++)
        {
            DataManager.instance.itemCombineData.TryGetValue(i + 1001, out ItemCombineData itemData);

            if (itemData != null)
            {
                itemCombines.Add(itemData);
                Debug.Log(itemData.Result);
            }
            else
                break;
        }
       
        gameObject.SetActive(false);

        for (int i = 0; i < slots.Length; i++)
        {
            slotTransforms[i].gameObject.SetActive(false);
            slots[i].item = null;
            //craftTypeImages[i].GetComponent<Image>().sprite = craftTypeImage[0];
        }
    }

    public void FreshCraftingBag()
    {
        for (int i = 0; i < items.Count; i++)
        {
            slotTransforms[i].gameObject.SetActive(false);
            slots[i].item = null;
            //craftTypeImages[i].GetComponent<Image>().sprite = craftTypeImage[0];
        }

        int j = 0;

        for (; j < items.Count; j++)
        {
            slotTransforms[j].gameObject.SetActive(true);
            slots[j].item = items[j];
            CombineItem();
        }

        for (; j < slots.Length; j++)
        {
            slotTransforms[j].gameObject.SetActive(false);
            slots[j].item = null;
        }
    }

    public void CraftItem(ItemBase _item)
    {
        items.Add(_item);
        FreshCraftingBag();
        inventoryPage.RemoveItem(_item);
    }

    public void ReturnItem()
    {
        gameObject.SetActive(false);
        inventoryUi.SetActive(false);

        for (int i = 0; i < items.Count; i++)
        {
            inventoryPage.AddItem(items[i]);
            slotTransforms[i].gameObject.SetActive(false);
            slots[i].item = null;
            //craftTypeImages[i].GetComponent<Image>().sprite = craftTypeImage[0];
        }
    }

    /// <summary>
    /// 조합 초안티비.. 진짜 지저분하고 마음에 안들지만,, 일단.. 자러가기위해 머지해놓은것입니다,, 기다려주새요,, 흑흑티비
    /// </summary>
    public void CombineItem()
    {
        int flag = 0;
        int j = 0;
        for (int i = 0; i < items.Count; i++) 
        {
            if (flag == 1)
            {
                break;
            }

            for (; i < itemCombines.Count; j++) 
            {
                flag = 0;

                if (itemCombines[j].Material_1 != "-1" && itemCombines[j].Material_1 == items[i].itemCode)
                {
                    flag = 1; break;
                }
                else if (itemCombines[j].Material_2 != "-1" && itemCombines[j].Material_2 == items[i].itemCode)
                {
                    flag = 1; break;
                }
                else if (itemCombines[j].Material_3 != "-1" && itemCombines[j].Material_3 == items[i].itemCode)
                {
                    flag = 1; break;
                }
                else if (itemCombines[j].Material_4 != "-1" && itemCombines[j].Material_4 == items[i].itemCode)
                {
                    flag = 1; break;
                }
                else if (itemCombines[j].Material_5 != "-1" && itemCombines[j].Material_5 == items[i].itemCode)
                {
                    flag = 1; break;
                }
                else if (itemCombines[j].Material_6 != "-1" && itemCombines[j].Material_6 == items[i].itemCode)
                {
                    flag = 1; break;
                }
                else if (itemCombines[j].Material_7 != "-1" && itemCombines[j].Material_7 == items[i].itemCode)
                {
                    flag = 1; break;
                }
                else if (itemCombines[j].Material_8 != "-1" && itemCombines[j].Material_8 == items[i].itemCode)
                {
                    flag = 1; break;
                }
            }
        }
        
        if (flag == 1)
        {
            Debug.Log(itemCombines[j].Result);
        }
    }
}
