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
}
