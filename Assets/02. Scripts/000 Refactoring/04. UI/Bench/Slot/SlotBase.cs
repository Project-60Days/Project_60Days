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

    protected BenchPanel bench;
    private ItemInfoPanel itemInfo;

    private ItemBase item;

    private bool isMouseEnter = false;

    protected virtual void Start()
    {
        itemInfo = App.Manager.UI.GetPanel<ItemInfoPanel>();
        bench = App.Manager.UI.GetPanel<BenchPanel>();
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

    protected virtual void DescriptionOn()
    {
        if (item != null &&  isMouseEnter) return;

        itemInfo.SetInfo(Item);
        isMouseEnter = true;
    }

    protected void DescriptionOff()
    {
        itemInfo.ClosePanel();
        isMouseEnter = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DescriptionOn();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DescriptionOff();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        DescriptionOff();
    }
}