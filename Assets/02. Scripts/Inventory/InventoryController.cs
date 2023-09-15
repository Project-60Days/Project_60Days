using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEditor.Progress;

public class InventoryController : ControllerBase
{
    [SerializeField] Transform slotParent;
    [SerializeField] Sprite[] itemTypeImage;
    [SerializeField] ItemSO itemSO;

    ItemSlot[] slots;
    Temp[] slotImages;
    TextMeshProUGUI[] itemCounts;

    public List<ItemBase> items;

    public override EControllerType GetControllerType()
    {
        return EControllerType.INVENTORY;
    }

    void OnValidate()
    {
        slots = slotParent.GetComponentsInChildren<ItemSlot>();
        slotImages = slotParent.GetComponentsInChildren<Temp>();
        itemCounts = slotParent.GetComponentsInChildren<TextMeshProUGUI>();
    }

    void Awake()
    {
        foreach(var item in itemSO.items)
        {
            item.itemCount = 0;
        }

        AddItemByItemCode("ITEM_TIER_2_SIGNALLER");
        AddItemByItemCode("ITEM_TIER_2_PLASMA");
        AddItemByItemCode("ITEM_TIER_1_PLASTIC");
        AddItemByItemCode("ITEM_TIER_1_STEEL");
        UpdateSlot();
    }

    /// <summary>
    /// slot에 변경사항 적용 시 호출됨. 인벤토리 내의 슬롯에 아이템 추가
    /// </summary>
    public void UpdateSlot()
    {
        InitSlots();

        for (int i = 0; i < items.Count; i++)
        {
            slots[i].item = items[i];
            itemCounts[i].gameObject.SetActive(true);
            itemCounts[i].text = items[i].itemCount.ToString();

            if (items[i].eItemType == EItemType.Consumption)
                slotImages[i].GetComponent<Image>().sprite = itemTypeImage[0];
            else if (items[i].eItemType == EItemType.Equipment)
                slotImages[i].GetComponent<Image>().sprite = itemTypeImage[1];
            else if (items[i].eItemType == EItemType.Material)
                slotImages[i].GetComponent<Image>().sprite = itemTypeImage[2];
        }
    }

    /// <summary>
    /// slot 초기화
    /// </summary>
    private void InitSlots()
    {
        for (int i = 0; i < slotParent.childCount; i++)
        {
            slots[i].item = null;
            slotImages[i].GetComponent<Image>().sprite = itemTypeImage[0];
            itemCounts[i].text = "0";
            itemCounts[i].gameObject.SetActive(false);
        }
    }





    /// <summary>
    /// 인벤토리에 ItemBase를 이용하여 아이템 추가
    /// </summary>
    /// <param name="_item"></param>
    public void AddItem(ItemBase _item)
    {
        foreach (var item in items)
        {
            if (item == _item)
            {
                item.itemCount++;
                UpdateSlot();
                return;
            }
        }
        _item.itemCount++;
        items.Add(_item);
        UpdateSlot();
    }

    /// <summary>
    /// 인벤토리에 itemCode를 이용하여 아이템 추가
    /// </summary>
    /// <param name="itemCode"></param>
    public void AddItemByItemCode(string itemCode)
    {
        for (int i = 0; i < itemSO.items.Length; i++)
        {
            if (itemSO.items[i].itemCode == itemCode)
            {
                AddItem(itemSO.items[i]);
            }
        }
    }





    /// <summary>
    /// 인벤토리에서 아이템 삭제
    /// </summary>
    /// <param name="_item"></param>
    public void RemoveItem(ItemBase _item)
    {
        _item.itemCount--;
        for(int i = 0; i < items.Count; i++)
        {
            if (items[i] == _item)
            {
                items.RemoveAt(i);
                break;
            }
        }
        UpdateSlot();
    }





    /// <summary>
    /// 인벤토리 내에 특정 아이템 존재하는지 체크
    /// </summary>
    /// <param name="itemCode"></param>
    /// <returns></returns>
    public bool CheckInventoryItem(string itemCode)
    {
        for(int i = 0; i < items.Count; i++)
        {
            if (items[i].itemCode == itemCode)
                return true;
        }

        return false;
    }
}
