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
    [SerializeField] Sprite[] buttonSprite;

    Button[] buttons;
    Image inventoryImage;

    BlueprintSlot[] blueprintSlots;

    public ECraftModeType eCraftModeType { get; private set; }

    void Awake()
    {
        inventoryImage = GameObject.Find("InventoryBackground_Img").GetComponent<Image>();
        buttons = GameObject.Find("ModeBtn_Back").GetComponentsInChildren<Button>();
        blueprintSlots = GetComponentsInChildren<BlueprintSlot>(includeInactive: true);
        SetCraftActive();
    }
    
    void SetButtonImage(int _activeButton)
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            Image buttonImage = buttons[i].GetComponent<Image>();
            if (i == _activeButton)
                buttonImage.sprite = buttonSprite[1];
            else
                buttonImage.sprite = buttonSprite[0];
        }
    }

    void CraftActiveMode()
    {
        craftBag.SetActive(true);
        SetButtonImage(0);

        eCraftModeType = ECraftModeType.Craft;

        UIManager.instance.GetCraftingRawImageController().DestroyObject();

        inventoryImage.sprite = inventorySprite[0];
    }

    void CraftInActiveMode()
    {
        craftBag.SetActive(false);
        UIManager.instance.GetCraftingUiController().ExitCraftBag();
    }

    void EquipActiveMode()
    {
        equipBag.SetActive(true);
        SetButtonImage(1);

        eCraftModeType = ECraftModeType.Equip;

        UIManager.instance.GetCraftingRawImageController().DestroyObject();

        inventoryImage.sprite = inventorySprite[1];
    }

    void EquipInActiveMode()
    {
        equipBag.SetActive(false);
    }

    void BlueprintActiveMode()
    {
        blueprintBag.SetActive(true);
        blueprintBack.SetActive(true);
        inventoryBack.SetActive(false);
        SetButtonImage(2);

        eCraftModeType = ECraftModeType.Blueprint;

        UIManager.instance.GetCraftingRawImageController().DestroyObject();

        inventoryImage.sprite = inventorySprite[2];

        UpdateBlueprint();
    }

    void BlueprintInActiveMode()
    {
        inventoryBack.SetActive(true);
        blueprintBag.SetActive(false);
        blueprintBack.SetActive(false);

        UIManager.instance.GetCraftingUiController().ExitBlueprintBag();
    }

    public void UpdateBlueprint()
    {
        foreach (var slot in blueprintSlots)
        {
            slot.CheckShowCondition();
        }
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
