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

    void Awake()
    {
        SetCraftActive();
    }
    

    void CraftActiveMode()
    {
        CraftBack.SetActive(true);
        eCraftModeType = ECraftModeType.Craft;
        UIManager.instance.GetCraftingRawImageController().DestroyObject();
    }

    void CraftInActiveMode()
    {
        CraftBack.SetActive(false);
        UIManager.instance.GetCraftingUiController().ExitCraftBag();
    }

    void EquipActiveMode()
    {
        EquipBack.SetActive(true);
        eCraftModeType = ECraftModeType.Equip;
        UIManager.instance.GetCraftingRawImageController().canRotate = false;
        UIManager.instance.GetCraftingRawImageController().ChangerTarget(player);
    }

    void EquipInActiveMode()
    {
        EquipBack.SetActive(false);
        UIManager.instance.GetCraftingRawImageController().canRotate = true;
        UIManager.instance.GetCraftingUiController().ExitEquipBag();
    }

    void BlueprintActiveMode()
    {
        BlueprintBack.SetActive(true);
        eCraftModeType = ECraftModeType.Blueprint;
        UIManager.instance.GetCraftingRawImageController().DestroyObject();
    }

    void BlueprintInActiveMode()
    {
        BlueprintBack.SetActive(false);
    }




    #region Buttons
    public ECraftModeType GetECraftModeType()
    {
        return eCraftModeType;
    }

    public void SetCraftActive()
    {
        Debug.Log("Craft");
        CraftActiveMode();
        EquipInActiveMode();
        BlueprintInActiveMode();
    }

    public void SetEquipActive()
    {
        Debug.Log("Equip");
        CraftInActiveMode();
        EquipActiveMode();
        BlueprintInActiveMode();
    }

    public void SetBlueprintActive()
    {
        Debug.Log("Blueprint");
        CraftInActiveMode();
        EquipInActiveMode();
        BlueprintActiveMode();
    }
    #endregion

}
