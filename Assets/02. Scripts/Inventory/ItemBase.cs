using UnityEngine;

public class ItemBase : ScriptableObject
{
    public string itemCode;
    public ItemData data;
    public Sprite itemImage;
    public int itemCount = 0;
    public ItemType itemType;
    public GameObject prefab;
}
