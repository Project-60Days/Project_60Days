using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;
using static UnityEditor.Progress;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image image;
    [SerializeField] public ESlotType eSlotType;

    public static Action<GameObject> CraftItemClick;

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
            case ESlotType.InventorySlot:
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
                CraftItemClick?.Invoke(itemSave.prefab);
                break;
            case ESlotType.CraftingSlot:
                UIManager.instance.GetInventoryController().AddItem(_item);
                UIManager.instance.GetCraftingUiController().MoveCraftToInventory(_item);
                break;
            case ESlotType.ResultSlot:
                UIManager.instance.GetInventoryController().AddItem(_item);
                UIManager.instance.GetCraftingUiController().MoveResultToInventory();
                break;
        }
    }
}