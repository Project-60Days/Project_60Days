using UnityEngine;

[CreateAssetMenu]
public class ItemBase : ScriptableObject
{
    public string Code;
    public ItemData data;
    public Sprite itemImage;
    public int itemCount = 0;
    public ItemType itemType;
    public Sprite sprite;
    public string sfxName;
    public bool isMadeOnce = false;
    public bool isBlueprintOpen = false;
    public bool canRemoveEquipment = false;

    public void Init()
    {
        itemCount = 0;

        if (data.Code == "ITEM_PLATE" || data.Code == "ITEM_WIRE" || data.Code == "ITEM_GEAR" || data.Code == "ITEM_BATTERY")
        {
            isMadeOnce = true;
            isBlueprintOpen = true;
        }
        else
        {
            isMadeOnce = false;
            isBlueprintOpen = false;
        }

        switch (data.Type)
        {
            case 0:
                itemType = ItemType.Material;
                break;
            case 1:
                itemType = ItemType.Equipment;
                break;
            case 2:
                itemType = ItemType.Special;
                break;
        }
    }

    public virtual void Equip()
    {

    }
    public virtual void UnEquip()
    {

    }

    public virtual bool CheckMeetCondition()
    {
        return false;
    }

    public virtual void DayEvent()
    {

    }
}