using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateItem : MonoBehaviour
{
    [SerializeField] ItemBase sliver;
    [SerializeField] ItemBase sword;
    [SerializeField] ItemBase pork;

    InventoryPage page1;
    InventoryPage page2;
    InventoryPage page3;

    void Awake()
    {
        page1 = GameObject.Find("MaterialInventory").GetComponent<InventoryPage>();
        page2 = GameObject.Find("EquipmentInventory").GetComponent<InventoryPage>();
        page3 = GameObject.Find("ConsumptionInventory").GetComponent<InventoryPage>();
    }
    public void MaterialClick()
    {
        page1.AddItem(sliver);
    }
    public void EquipmentClick()
    {
        page2.AddItem(sword);
    }
    public void ConsumptionClick()
    {
        page3.AddItem(pork);
    }
}
