using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    public string itemName;
    public Sprite itemImage;
    public int maxAmount;
    public int minAmount;
    public string itemTip;
    protected ItemType itemType;
}
