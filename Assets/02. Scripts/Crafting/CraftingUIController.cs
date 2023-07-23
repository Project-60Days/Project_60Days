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
    private ItemSlot[] slots;
    public List<Transform> slotTransforms;

    InventoryPage inventoryPage;

    public List<ItemBase> items;
    public List<Image> craftTypeImages;

    int craftBagCount = 0;

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

        this.gameObject.SetActive(false);

        for(int i = 0; i < slots.Length; i++)
        {
            slotTransforms[i].gameObject.SetActive(false);
            slots[i].item = null;
            craftTypeImages[i].GetComponent<Image>().sprite = craftTypeImage[0];
        }
    }

    public void FreshCraftingBag()
    {
        for (int i = 0; i < craftBagCount; i++)
        {
            slotTransforms[i].gameObject.SetActive(false);
            slots[i].item = null;
            craftTypeImages[i].GetComponent<Image>().sprite = craftTypeImage[0];
        }

        craftBagCount = 0;

        for (int i = 0; i < items.Count; i++)
        {
            slotTransforms[i].gameObject.SetActive(true);
            slots[i].item = items[i];
            craftBagCount++;
        }

        for (int i = craftBagCount; i < slots.Length; i++)
        {
            slotTransforms[i].gameObject.SetActive(false);
            slots[i].item = null;
        }
    }

    public void CraftItem(ItemBase _item)
    {
        items.Add(_item);
        FreshCraftingBag();
        inventoryPage.RemoveItem(_item);
    }
}
