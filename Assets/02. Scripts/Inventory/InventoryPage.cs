using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPage : MonoBehaviour
{
    [SerializeField] Transform slotParent;
    [SerializeField] Transform tipParent;
    [SerializeField] private ItemSlot[] slots;
    [SerializeField] private Text[] itemTips;
    [SerializeField] private Text[] itemCounts;

    public List<ItemBase> items;
    int slotCount = 0;

    private void OnValidate()
    {
        slots = slotParent.GetComponentsInChildren<ItemSlot>();
        itemCounts = slotParent.GetComponentsInChildren<Text>();
        itemTips = tipParent.GetComponentsInChildren<Text>();
    }
    void Awake()
    {
        FreshSlot();
    }

    public void FreshSlot()
    {
        int flag;
  
        for (int i = 0; i < items.Count;  i++)
        {
            flag = 0;

            if (i < slots.Length && slots[i].item != null)
                slots[i].item.itemCount = 0;

            for (int j = 0; j < slotCount; j++)
            {
                if (slots[j].item == items[i])
                {
                    items[i].itemCount++;
                    itemCounts[j].text = items[i].itemCount.ToString();
                    flag = 1;
                    break;
                }
            }

            if (flag == 1)
                continue;

            slots[slotCount].item = items[i];
            slots[slotCount].item.itemCount = 1;
            itemCounts[slotCount].gameObject.SetActive(true);
            itemCounts[slotCount].text = items[i].itemCount.ToString();
            //string itemDiscription = items[i].itemName + "\n" + items[i].itemTip;
            //itemTips[slotCount].text = itemDiscription;
            if (slots[slotCount].item != null)
                slotCount++;
        }
        for (int i = slotCount; i < slots.Length; i++)
        {
            slots[i].item = null;
            itemCounts[i].gameObject.SetActive(false);
        }
    }

    public void AddItem(ItemBase _item)
    {
        if (slotCount < slots.Length)
        {
            items.Add(_item);
            FreshSlot();
        }
        else
        {
            Debug.Log("½½·ÔÀÌ °¡µæ Â÷ ÀÖ½À´Ï´Ù.");
        }
    }
}
