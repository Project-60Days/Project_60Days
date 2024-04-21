using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class ItemBase : ScriptableObject
{
    [FormerlySerializedAs("itemCode")] public string English;
    public ItemData data;
    public Sprite itemImage;
    public int itemCount = 0;
    public EItemType eItemType;
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
                eItemType = EItemType.Material;
                break;
            case 1:
                eItemType = EItemType.Equipment;
                break;
            case 2:
                eItemType = EItemType.Special;
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