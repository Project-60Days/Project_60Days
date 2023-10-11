using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftModeController : MonoBehaviour
{
    [SerializeField] GameObject craftBag;
    [SerializeField] GameObject equipBag;
    [SerializeField] GameObject blueprintBag;
    [SerializeField] GameObject blueprint;
    [SerializeField] GameObject inventoryBack;

    public ECraftModeType eCraftModeType { get; private set; }

    void Awake()
    {
        SetCraftActive();
    }
    

    void CraftActiveMode()
    {
        craftBag.SetActive(true);
        eCraftModeType = ECraftModeType.Craft;
        UIManager.instance.GetCraftingRawImageController().DestroyObject();
    }

    void CraftInActiveMode()
    {
        craftBag.SetActive(false);
        UIManager.instance.GetCraftingUiController().ExitCraftBag();
    }

    void EquipActiveMode()
    {
        equipBag.SetActive(true);
        eCraftModeType = ECraftModeType.Equip;
    }

    void EquipInActiveMode()
    {
        equipBag.SetActive(false);
        UIManager.instance.GetCraftingUiController().ExitEquipBag();
    }

    void BlueprintActiveMode()
    {
        blueprintBag.SetActive(true);
        blueprint.SetActive(true);
        inventoryBack.SetActive(false);
        eCraftModeType = ECraftModeType.Blueprint;
        UIManager.instance.GetCraftingRawImageController().DestroyObject();
    }

    void BlueprintInActiveMode()
    {
        inventoryBack.SetActive(true);
        blueprintBag.SetActive(false);
        blueprint.SetActive(false);
    }




    #region Buttons
    public void SetCraftActive()
    {
        CraftActiveMode();
        EquipInActiveMode();
        BlueprintInActiveMode();
    }

    public void SetEquipActive()
    {
        CraftInActiveMode();
        EquipActiveMode();
        BlueprintInActiveMode();
    }

    public void SetBlueprintActive()
    {
        CraftInActiveMode();
        EquipInActiveMode();
        BlueprintActiveMode();
    }
    #endregion
}
