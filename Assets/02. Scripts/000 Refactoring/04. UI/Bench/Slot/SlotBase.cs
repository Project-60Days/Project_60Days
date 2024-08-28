using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class SlotBase : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image image;

    public ItemBase Item
    {
        get { return item; }
        set
        {
            item = value;
            if (item != null)
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

    protected BenchPanel benchPanel;
    private ItemInfoPanel itemInfoPanel;

    private ItemBase item;

    private bool isMouseEnter = false;

    protected virtual void Start()
    {
        itemInfoPanel = App.Manager.UI.GetPanel<ItemInfoPanel>();
        benchPanel = App.Manager.UI.GetPanel<BenchPanel>();
    }

    private void Update()
    {
        if (isMouseEnter == true)
        {
            ShowItemInfo();
        }
    }

    public virtual void ShowItemInfo()
    {
        Vector3 mousePos = Input.mousePosition;
        itemInfoPanel.ShowInfo(item, mousePos);
    }

    public virtual void SetItem(ItemBase _item)
    {
        gameObject.SetActive(true);
        Item = _item;
    }

    public virtual void ResetItem()
    {
        gameObject.SetActive(false);
        Item = null;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        HideItemInfo();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null && isMouseEnter == false)
        {
            itemInfoPanel.isNew = true;
            isMouseEnter = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isMouseEnter == true) 
        {
            itemInfoPanel.HideInfo();
            isMouseEnter = false;
        }
    }

    protected void HideItemInfo()
    {
        itemInfoPanel.HideInfo();
        isMouseEnter = false;
    }
}
