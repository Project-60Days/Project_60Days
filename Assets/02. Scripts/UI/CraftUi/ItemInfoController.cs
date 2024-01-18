using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfoController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemDescribe;
    [SerializeField] TextMeshProUGUI itemEquip;
    [SerializeField] TextMeshProUGUI itemEffect;

    [SerializeField] GameObject contour;

    [SerializeField] Transform blueprintSlotParent;
    [SerializeField] GameObject blueprintSlotPrefab;

    [SerializeField] ItemSO itemSO;

    RectTransform infoTransform;

    public bool isNew = true;

    public bool isOpen = false;

    void Awake()
    {
        infoTransform = gameObject.GetComponent<RectTransform>();

        HideInfo();
    }

    public void HideInfo()
    {
        isNew = true;
        InitObjects();
        InitBlueprintSlots();
    }

    void InitObjects()
    {
        itemName.gameObject.SetActive(false);
        itemDescribe.gameObject.SetActive(false);
        itemEquip.gameObject.SetActive(false);
        itemEffect.gameObject.SetActive(false);

        contour.SetActive(false);

        blueprintSlotParent.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }

    void InitBlueprintSlots()
    {
        for (int i = 0; i < blueprintSlotParent.childCount; i++)
            Destroy(blueprintSlotParent.GetChild(i).gameObject);
    }

    public void ShowInfo(ItemBase _item, Vector3 _mouseCoordinate)
    {
        if (isOpen == false) return;

        if (isNew == true)
        {
            HideInfo();
            SetObejcts(_item);
            SetBlueprint(_item);
            isNew = false;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(infoTransform);

        float width = infoTransform.rect.width;
        float height = infoTransform.rect.height;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float newX = _mouseCoordinate.x;
        float newY = _mouseCoordinate.y;

        if (newX + width > screenWidth * 0.95)
            newX -= width * (screenWidth / 1920);
        if (newY - height < screenHeight * 0.1)
            newY += height * (screenHeight / 1080);

        infoTransform.position = new Vector3(newX, newY, infoTransform.position.z);

        gameObject.SetActive(true);
    }

    void SetObejcts(ItemBase _item)
    {
        if (!string.IsNullOrEmpty(_item.data.Korean))
        {
            itemName.gameObject.SetActive(true);
            itemName.text = _item.data.Korean;
        }
       
        if (!string.IsNullOrEmpty(_item.data.Description))
        {
            itemDescribe.gameObject.SetActive(true);
            itemDescribe.text = _item.data.Description;
        }
       
        if (_item.data.EquipArea != "-1")
        {
            itemEquip.gameObject.SetActive(true);
            contour.SetActive(true);
            itemEquip.text = _item.data.EquipArea;
        }

        if (!string.IsNullOrEmpty(_item.data.EffectDescription))
        {
            itemEffect.gameObject.SetActive(true);
            contour.SetActive(true);

            itemEffect.text = string.Format(_item.data.EffectDescription, _item.data.value1, _item.data.value2, _item.data.value3);
        }
    }

    void SetBlueprint(ItemBase _item)
    {
        string[] blueprintCodes = UIManager.instance.GetCraftingUiController().GetItemCombineCodes(_item);
        if (blueprintCodes == null) return;

        for (int i = 0; i < blueprintCodes.Length - 1; i++) 
        {
            if (blueprintCodes[i] == "-1") break;
            AddItemByItemCode(blueprintCodes[i]);
        }
    }

    void AddItemByItemCode(string _itemCode)
    {
        for (int i = 0; i < itemSO.items.Length; i++)
            if (itemSO.items[i].data.Code == _itemCode)
            {
                AddBlueprintItem(itemSO.items[i]);
                return;
            }

        Debug.Log("아직 추가되지 않은 아이템: " + _itemCode);
    }

    void AddBlueprintItem(ItemBase _item)
    {
        GameObject obj = Instantiate(blueprintSlotPrefab, blueprintSlotParent);
        obj.GetComponentInChildren<BlueprintSlot>().item = _item;
        obj.GetComponentInChildren<BlueprintSlot>().enabled = false;
        obj.GetComponentInChildren<TextMeshProUGUI>().text = _item.data.Korean;

        blueprintSlotParent.gameObject.SetActive(true);
    }

}
