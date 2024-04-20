using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryPanel : UIBase
{
    private readonly List<ItemBase> itemData = App.Data.Game.itemSO.items.ToList();

    List<ItemSlot>[] slots = new List<ItemSlot>[6];
    int[] counts = new int[6];

    List<ItemBase> items = new List<ItemBase>();

    ItemBase disturbe
        => itemData.ToList().Find(x => x.data.Code == "ITEM_DISTURBE");
    ItemBase findor
        => itemData.ToList().Find(x => x.data.Code == "ITEM_FINDOR");
    ItemBase netCard
        => itemData.ToList().Find(x => x.data.Code == "ITEM_NETWORKCHIP");

    #region Override
    public override void Init()
    {
        for (int i = 0; i < 6; i++)
            slots[i] = (new List<ItemSlot>());

        for (int i = 0; i < transform.childCount; i++)
        {
            var slot = transform.GetChild(i).GetComponent<ItemSlot>();
            int category = slot.category;
            slots[category].Add(slot);
        }

        foreach (var item in itemData)
        {
            item.Init();
        }

        InitSlots();

        gameObject.SetActive(false);
    }

    public override void ReInit() { }
    #endregion

    /// <summary>
    /// slot 초기화
    /// </summary>
    void InitSlots()
    {
        for (int i = 0; i < slots.Length; i++)
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
    /// slot에 변경사항 적용 시 호출됨. 인벤토리 내의 슬롯에 아이템 추가
    /// </summary>
    public void UpdateSlot()
    {
        InitSlots();

        App.Manager.UI.GetUpperController().UpdateItemCount();

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
        App.Manager.Map.mapUIController.ExplorerButtonInteractable(canUseFindor);

        bool canUseDisturbe = disturbe.itemCount > 0;
        App.Manager.Map.mapUIController.DistrubtorButtonInteractable(canUseDisturbe);
    }

    /// <summary>
    /// 인벤토리에 itemCode를 이용하여 아이템 추가
    /// </summary>
    /// <param name="itemCode"></param>
    public void AddItemByItemCode(string _itemCode)
    {
        var item = itemData.Find(x => x.data.Code == _itemCode);

        if (item != null)
            AddItem(item);
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
        var item = itemData.Find(x => x.data.Code == _itemCode);

        if (item != null) 
            RemoveItem(item);
    }
    
    public void RemoveRandomItem()
    {
        int random;

        if (items.Count == 0) 
            return;
        
        while (true)
        {
            random = Random.Range(0, items.Count);

            if (items[random].data.Code != "ITEM_NETWORKCHIP") break;
        }

        App.Manager.UI.GetPageController().SetCurrResource(items[random]);
        App.Manager.UI.GetPageController().SetResultPage("LOOSE_RESOURCE", false);
        
        RemoveItem(items[random]);
    }



    /// <summary>
    /// 인벤토리 내에 특정 아이템 존재하는지 체크
    /// </summary>
    /// <param name="itemCode"></param>
    /// <returns></returns>
    public bool CheckInventoryItem(string _itemCode)
        => items.Find(x => x.data.Code == _itemCode) != null ? true : false;
    




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
        => disturbe.itemCount > 0 ? true : false;


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

    void Update()
    {
        //InputKey();
    }

    private void InputKey()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var item in itemData)
            {
                if (++counts[item.data.Category] > slots[item.data.Category].Count) return;
                AddItem(item);
            }

        }
    }
    #endregion
}
