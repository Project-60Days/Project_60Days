using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image image;
    [SerializeField] public ESlotType eSlotType = ESlotType.EquipSlot;
    [SerializeField] public EItemType eItemType;

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
    }
}
