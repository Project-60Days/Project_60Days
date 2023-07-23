using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingUIController : MonoBehaviour
{
    [SerializeField] GameObject slot;
    [SerializeField] Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        DataManager.instance.itemCombineData.TryGetValue(1001, out ItemCombineData itemData);
        Debug.Log(itemData);

        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CraftItem(ItemBase _item)
    {
        Instantiate(slot, parent);
        //slot.item = _item;
    }
}
