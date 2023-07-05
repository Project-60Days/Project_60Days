using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePage : MonoBehaviour
{
    [SerializeField] GameObject materialInventory;
    [SerializeField] GameObject equipmentInventory;
    [SerializeField] GameObject consumptionInventory;

    void Start()
    {
        materialInventory.SetActive(true);
        equipmentInventory.SetActive(false);
        consumptionInventory.SetActive(false);
    }

    public void MaterialButtonClick()
    {
        materialInventory.SetActive(true);
        equipmentInventory.SetActive(false);
        consumptionInventory.SetActive(false);
    }

    public void EquipmentButtonClick()
    {
        materialInventory.SetActive(false);
        equipmentInventory.SetActive(true);
        consumptionInventory.SetActive(false);
    }

    public void ConsumptionButtonClick()
    {
        materialInventory.SetActive(false);
        equipmentInventory.SetActive(false);
        consumptionInventory.SetActive(true);
    }
}
