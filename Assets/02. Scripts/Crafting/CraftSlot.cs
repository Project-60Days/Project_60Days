using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CraftSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image image;
    [SerializeField] public ESlotType eSlotType;

    public static Action CraftItemClick;

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
        switch (eSlotType)
        {
            case ESlotType.CraftingSlot:
                UIManager.instance.GetInventoryController().AddItem(_item);
                UIManager.instance.GetCraftingUiController().MoveCraftToInventory(_item);
                break;
            case ESlotType.ResultSlot:
                UIManager.instance.GetInventoryController().AddItem(_item);
                UIManager.instance.GetCraftingUiController().MoveResultToInventory();
                break;
        }

        CraftItemClick?.Invoke();
    }
}
