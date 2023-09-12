using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class SlotBase : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image image;
    [SerializeField] public ESlotType eSlotType;

    public ItemBase _item;

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

    public abstract void OnPointerClick(PointerEventData eventData);
}
