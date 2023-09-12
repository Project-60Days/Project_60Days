using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image image;
    [SerializeField] public ESlotType eSlotType = ESlotType.InventorySlot;

    public static Action<GameObject> InventoryItemClick;

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

    public void OnPointerClick(PointerEventData eventData)
    {
        var itemSave = _item;
        UIManager.instance.GetCraftingUiController().MoveInventoryToCraft(_item);
        if (_item.itemCount == 1)
        {
            UIManager.instance.GetInventoryController().RemoveItem(_item);
        }
        else
        {
            _item.itemCount--;
            UIManager.instance.GetInventoryController().UpdateSlot();
        }
        InventoryItemClick?.Invoke(itemSave.prefab);
    }
}