using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image image;
    [SerializeField] public ESlotType eSlotType;

    TextMeshProUGUI itemDiscription;
    GameObject craft;

    public static Action<ItemBase> CraftItemClick;

    private ItemBase _item;

    public ItemBase item
    {
        get { return _item; }
        set
        {
            _item = value;
            if (_item != null)
            {
                image.sprite = item.itemImage;
                image.color = Color.white;
            }
            else
            {
                image.sprite = null;
                image.color = Color.clear;
            }
        }
    }

    void Awake()
    {
        craft = GameObject.Find("CraftingUi");
        itemDiscription = GameObject.FindWithTag("ItemDiscription").GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eSlotType)
        {
            case ESlotType.InventorySlot:
                if (craft.activeSelf)
                {
                    if (_item != null)
                    {
                        var itemSave = _item;
                        App.instance.GetCraftController().CraftItem(_item);
                        CraftItemClick?.Invoke(itemSave);
                    }
                }
                else
                    itemDiscription.text = _item.data.Description.ToString();
                break;
            case ESlotType.CraftingSlot:
                //App.instance.GetCraftController().CraftToInventory(this); break;
                return;
            case ESlotType.ResultSlot:
                App.instance.GetCraftController().ResultToInventory(); break;
        }
    }
}