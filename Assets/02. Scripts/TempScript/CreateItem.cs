using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateItem : MonoBehaviour
{
    [SerializeField] ItemBase sliver;
    [SerializeField] ItemBase sword;
    [SerializeField] ItemBase pork;

    InventoryPage page;

    void Awake()
    {
        page = GameObject.Find("Inventory").GetComponent<InventoryPage>();
    }
    public void MaterialClick()
    {
        page.AddItem(sliver);
    }
    public void EquipmentClick()
    {
        page.AddItem(sword);
    }
    public void ConsumptionClick()
    {
        page.AddItem(pork);
    }
}
