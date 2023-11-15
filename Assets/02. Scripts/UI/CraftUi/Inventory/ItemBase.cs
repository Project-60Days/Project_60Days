using UnityEngine;
using UnityEngine.Serialization;

public class ItemBase : ScriptableObject
{
    [FormerlySerializedAs("itemCode")] public string English;
    public ItemData data;
    public Sprite itemImage;
    public int itemCount = 0;
    public EItemType eItemType;
    public GameObject prefab;
    public string sfxName;
}