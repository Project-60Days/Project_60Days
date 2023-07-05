using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MaterialItem : ItemBase
{
    ItemType thisType = ItemType.Material;

    public MaterialItem()
    {
        itemType = thisType;
    }
}
