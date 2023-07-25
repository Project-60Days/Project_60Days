using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    public string itemCode;
    public ItemData data;
    public Sprite itemImage;
    public int itemCount;
    public ItemType itemType;
    public EResourceType resourceType;

    public void SetCount(int count)
    {
        itemCount = count;
    }

    public void IncreaseDecreaseCount(int count)
    {
        itemCount += count;

        if (itemCount < 0)
            itemCount = 0;
    }
}
