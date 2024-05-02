using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class SlotBase : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image image;
    public ESlotType eSlotType;

    ItemBase _item;

    bool isMouseEnter = false;

    void Update()
    {
        if (isMouseEnter == true)
            ShowItemInfo();
    }

    public virtual void ShowItemInfo()
    {
        Vector3 mousePos = Input.mousePosition;
        App.Manager.UI.GetPanel<ItemInfoPanel>().ShowInfo(_item, mousePos);
    }

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null && isMouseEnter == false)
        {
            isMouseEnter = true;
            App.Manager.UI.GetPanel<ItemInfoPanel>().isNew = true;
        }
            
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isMouseEnter == true) 
        {
            App.Manager.UI.GetPanel<ItemInfoPanel>().HideInfo();
            isMouseEnter = false;
        }
    }

    protected void HideItemInfo()
    {
        App.Manager.UI.GetPanel<ItemInfoPanel>().HideInfo();
        if (item == null)
            isMouseEnter = false;
    }
}
