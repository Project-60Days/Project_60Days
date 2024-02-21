using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryController : MonoBehaviour
{
    [SerializeField] ItemSO itemSO;
    Transform slotParent;

    List<List<ItemSlot>> slots = new List<List<ItemSlot>>();
    int[] counts = new int[6];
    List<ItemBase> items = new List<ItemBase>();

    ItemBase disturbe;
    ItemBase findor;
    ItemBase netCard;

    void Awake()
    {
        slotParent = gameObject.GetComponent<Transform>();

        for (int i = 0; i < 6; i++)
            slots.Add(new List<ItemSlot>());

        for (int i = 0; i < slotParent.childCount; i++)
        {
            var slot = slotParent.GetChild(i).GetComponent<ItemSlot>();
            int category = slot.category;
            slots[category].Add(slot);
        }

        foreach (var item in itemSO.items)
        {
            item.Init();

            if (item.data.Code == "ITEM_DISTURBE")
                disturbe = item;
            else if (item.data.Code == "ITEM_FINDOR")
                findor = item;
            else if (item.data.Code == "ITEM_NETWORKCHIP")
                netCard = item;
        }

        InitSlots();
    }


    /// <summary>
    /// slot에 변경사항 적용 시 호출됨. 인벤토리 내의 슬롯에 아이템 추가
    /// </summary>
    public void UpdateSlot()
    {
        InitSlots();

        UIManager.instance.GetUpperController().UpdateItemCount();

        for (int i = 0; i < items.Count; i++)
        {
            int category = items[i].data.Category;
            var currentSlot = slots[category][counts[category]];
            currentSlot.gameObject.SetActive(true);
            currentSlot.item = items[i];
            currentSlot.GetComponentInChildren<TextMeshProUGUI>().text = items[i].itemCount.ToString();
            counts[category]++;
        }

        CheckDisturbeNFindor();
    }

    /// <summary>
    /// slot 초기화
    /// </summary>
    void InitSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            for (int j = 0; j < slots[i].Count; j++)
            {
                slots[i][j].gameObject.SetActive(false);
                slots[i][j].item = null;
            }
        }

        for (int i = 0; i < counts.Length; i++)
            counts[i] = 0;
    }





    /// <summary>
    /// 인벤토리에 ItemBase를 이용하여 아이템 추가
    /// </summary>
    /// <param name="_item"></param>
    public void AddItem(ItemBase _item)
    {
        _item.itemCount++;

        if (items.Contains(_item))
        {
            UpdateSlot();
            return;
        }

        items.Add(_item);
        UpdateSlot();
    }

    void CheckDisturbeNFindor()
    {
        bool canUseFindor = findor.itemCount > 0;
        App.instance.GetMapManager().UIController.ExplorerButtonInteractable(canUseFindor);

        bool canUseDisturbe = disturbe.itemCount > 0;
        App.instance.GetMapManager().UIController.DistrubtorButtonInteractable(canUseDisturbe);
    }

    /// <summary>
    /// 인벤토리에 itemCode를 이용하여 아이템 추가
    /// </summary>
    /// <param name="itemCode"></param>
    public void AddItemByItemCode(string _itemCode)
    {
        for (int i = 0; i < itemSO.items.Length; i++)
            if (itemSO.items[i].data.Code == _itemCode)
                AddItem(itemSO.items[i]);
    }





    /// <summary>
    /// 인벤토리에서 아이템 삭제
    /// </summary>
    /// <param name="_item"></param>
    public void RemoveItem(ItemBase _item)
    {
        _item.itemCount--;

        if (_item.itemCount == 0)
            items.Remove(_item);

        UpdateSlot();
    }

    public void RemoveItemByCode(string _itemCode)
    {
        ItemBase item;
        for (int i = 0; i < itemSO.items.Length; i++)
            if (itemSO.items[i].English == _itemCode)
            {
                item = itemSO.items[i];
                RemoveItem(item);
            }
    }
    
    public void RemoveRandomItem()
    {
        int random;
        
        if(items.Count == 0)
            return;
        
        while (true)
        {
            random = Random.Range(0, items.Count);

            if(items[random].data.Code != "ITEM_NETWORKCHIP")
                break;
        }
        UIManager.instance.GetPageController().SetCurrResource(items[random]);
        UIManager.instance.GetPageController().SetResultPage("LOOSE_RESOURCE", false);
        
        RemoveItem(items[random]);
    }



    /// <summary>
    /// 인벤토리 내에 특정 아이템 존재하는지 체크
    /// </summary>
    /// <param name="itemCode"></param>
    /// <returns></returns>
    public bool CheckInventoryItem(string _itemCode)
    {
        foreach (var item in items) 
        {
            if (item.English == _itemCode)
                return true;
        }
        return false;
    }
    




    public bool CheckFindorUsage()
    {
        if (findor.itemCount <= 0)
            return false;
        else
        {
            RemoveItem(findor);
            return true;
        }
    }
    
    public bool CheckFindorExist()
    {
        if (findor.itemCount <= 0)
            return false;
        else
        {
            return true;
        }
    }

    public bool CheckDistrubtorUsage()
    {
        if (disturbe.itemCount <= 0)
            return false;
        else
        {
            RemoveItem(disturbe);
            return true;
        }
    }
    
    public bool CheckDisturbeExist()
    {
        if (disturbe.itemCount <= 0)
            return false;
        else
        {
            return true;
        }
    }

    public bool CheckNetCardUsage()
    {
        if (netCard.itemCount <= 0)
            return false;
        else
        {
            RemoveItem(netCard);
            return true;
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
    /// 아이템 전부 획득하는 궁극의 함수..이지만 이 함수 한 번 호출하면 다른 아이템 추가가 안됩니다
    /// </summary>
    private void InputKey()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach(var item in itemSO.items)
            {
                if (++counts[item.data.Category] > slots[item.data.Category].Count) return;
                AddItem(item);
            }
                
        }
    }
    #endregion
}
