using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image image;
    //[SerializeField]
    //ESlotType
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
                image.color = new Color(1, 1, 1, 1);
            }
            else
            {
                image.sprite = null;
                image.color = new Color(1, 1, 1, 0);
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
        if (craft.activeSelf)
        {
            if (_item != null)
            {
                craft.GetComponent<CraftingUIController>().CraftItem(_item);
                CraftItemClick?.Invoke(_item);
            }
        }
        else
        {
            Debug.Log("´­¸²");
            Debug.Log(itemDiscription);
            Debug.Log(_item.data.Description.ToString());
            itemDiscription.text = _item.data.Description.ToString();
        }
        
    }
}