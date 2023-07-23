using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUIController : MonoBehaviour
{
    [SerializeField] Transform slotParent;
    [SerializeField] Sprite[] craftTypeImage;
    [SerializeField] GameObject inventoryUi;

    private ItemSlot[] slots;
    private List<Transform> slotTransforms;

    InventoryPage inventoryPage;

    public List<ItemBase> items;
    private List<Image> craftTypeImages;

    void Awake()
    {
        inventoryPage = GameObject.Find("Inventory").GetComponent<InventoryPage>();

        slots = slotParent.GetComponentsInChildren<ItemSlot>();
        for (int i = 0; i < slotParent.childCount; i++) 
        {
            slotTransforms.Add(slotParent.GetChild(i));
        }
        for(int i = 0; i < slotTransforms.Count; i++)
        {
            craftTypeImages.Add(slotTransforms[i].GetChild(1).GetComponent<Image>());
        }
    }

    void Start()
    {
        DataManager.instance.itemCombineData.TryGetValue(1001, out ItemCombineData itemData);

        gameObject.SetActive(false);

        for(int i = 0; i < slots.Length; i++)
        {
            slotTransforms[i].gameObject.SetActive(false);
            slots[i].item = null;
            craftTypeImages[i].GetComponent<Image>().sprite = craftTypeImage[0];
        }
    }

    public void FreshCraftingBag()
    {
        for (int i = 0; i < items.Count; i++)
        {
            slotTransforms[i].gameObject.SetActive(false);
            slots[i].item = null;
            craftTypeImages[i].GetComponent<Image>().sprite = craftTypeImage[0];
        }

        int j = 0;

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
            craftTypeImages[i].GetComponent<Image>().sprite = craftTypeImage[0];
        }
    }
}
