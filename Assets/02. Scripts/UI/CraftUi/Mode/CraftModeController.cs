using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftModeController : MonoBehaviour
{
    [SerializeField] GameObject craftBag;
    [SerializeField] GameObject equipBag;
    [SerializeField] GameObject blueprintBag;
    [SerializeField] GameObject blueprintBack;
    [SerializeField] GameObject inventoryBack;

    [SerializeField] Sprite[] inventorySprite;
    [SerializeField] TextMeshProUGUI modeText;

    public Image inventoryImage;

    public ECraftModeType eCraftModeType { get; private set; }

    void Awake()
    {
        inventoryImage = GetComponent<Image>();
        SetCraftActive();
    }
    

    void CraftActiveMode()
    {
        craftBag.SetActive(true);
        eCraftModeType = ECraftModeType.Craft;
        UIManager.instance.GetCraftingRawImageController().DestroyObject();
        inventoryImage.sprite = inventorySprite[0];
        modeText.text = "제작 모드";
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
        inventoryImage.sprite = inventorySprite[1];
        modeText.text = "장착 모드";
    }

    void EquipInActiveMode()
    {
        equipBag.SetActive(false);
        UIManager.instance.GetCraftingUiController().ExitEquipBag();
    }

    void BlueprintActiveMode()
    {
        blueprintBag.SetActive(true);
        blueprintBack.SetActive(true);
        inventoryBack.SetActive(false);
        eCraftModeType = ECraftModeType.Blueprint;
        UIManager.instance.GetCraftingRawImageController().DestroyObject();
        inventoryImage.sprite = inventorySprite[2];
        modeText.text = "설계도 모드";
    }

    void BlueprintInActiveMode()
    {
        inventoryBack.SetActive(true);
        blueprintBag.SetActive(false);
        blueprintBack.SetActive(false);
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
