using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftModeController : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject CraftBack;
    [SerializeField] GameObject EquipBack;
    [SerializeField] GameObject BlueprintBack;

    ECraftModeType eCraftModeType;

    void Start()
    {
        SetCraftActive();
    }
    

    void CraftActiveMode()
    {
        eCraftModeType = ECraftModeType.Craft;
        UIManager.instance.GetCraftingUiController().enabled = true;
        UIManager.instance.GetCraftingRawImageController().DestroyObject();
        CraftBack.SetActive(true);
    }

    void CraftInActiveMode()
    {
        UIManager.instance.GetCraftingUiController().enabled = false;
        UIManager.instance.GetCraftingUiController().ExitUi();
        CraftBack.SetActive(false);
    }

    void EquipActiveMode()
    {
        eCraftModeType = ECraftModeType.Equip;
        UIManager.instance.GetCraftingRawImageController().canRotate = false;
        UIManager.instance.GetCraftingRawImageController().ChangerTarget(player);
        EquipBack.SetActive(true);
    }

    void EquipInActiveMode()
    {
        UIManager.instance.GetCraftingRawImageController().canRotate = false;
        EquipBack.SetActive(false);
    }

    void BlueprintActiveMode()
    {
        eCraftModeType = ECraftModeType.Blueprint;
        UIManager.instance.GetCraftingRawImageController().DestroyObject();
        BlueprintBack.SetActive(true);
    }

    void BlueprintInActiveMode()
    {
        BlueprintBack.SetActive(false);
    }

    public ECraftModeType GetECraftModeType()
    {
        return eCraftModeType;
    }

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
}
