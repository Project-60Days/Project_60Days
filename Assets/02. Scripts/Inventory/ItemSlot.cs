using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image image;

    CraftingUIController craft;
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
        craft = GameObject.Find("CraftingUi").GetComponent<CraftingUIController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_item != null)
        {
            craft.CraftItem(_item);
        } 
    }
}