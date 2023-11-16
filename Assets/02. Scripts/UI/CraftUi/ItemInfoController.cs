using UnityEngine;
using TMPro;

public class ItemInfoController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemDescribe;
    [SerializeField] TextMeshProUGUI itemEquip;
    [SerializeField] TextMeshProUGUI itemEffect;
    [SerializeField] GameObject contour;

    RectTransform infoTransform;

    void Awake()
    {
        infoTransform = gameObject.GetComponent<RectTransform>();
        infoTransform = gameObject.GetComponent<RectTransform>();
        HideInfo();
    }

    public void ShowInfo(ItemBase _item, Vector3 _mouseCoordinate)
    {
        if (gameObject.activeSelf) HideInfo();
        gameObject.SetActive(true);

        float width = infoTransform.rect.width;
        float height = infoTransform.rect.height;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float newX = _mouseCoordinate.x;
        float newY = _mouseCoordinate.y;

        if (_mouseCoordinate.x + width > screenWidth - 10)
            newX -= width;
        if (_mouseCoordinate.y - height < 10)
            newY += height;
 
        infoTransform.position = new Vector3(newX, newY, infoTransform.position.z);
        SetInfoText(_item);
    }

    void SetInfoText(ItemBase _item)
    {
        if (!string.IsNullOrEmpty(_item.data.Korean)) 
        {
            itemName.gameObject.SetActive(true);
            itemName.text = _item.data.Korean;

        } else itemName.gameObject.SetActive(false);
        if (!string.IsNullOrEmpty(_item.data.Description))
        {
            itemDescribe.gameObject.SetActive(true);
            itemDescribe.text = _item.data.Description;

        } else itemDescribe.gameObject.SetActive(false);
        if (!string.IsNullOrEmpty(_item.data.EquipArea))
        {
            itemEquip.gameObject.SetActive(true);
            itemEquip.text = _item.data.EquipArea;

        } else itemEquip.gameObject.SetActive(false);
        if (!string.IsNullOrEmpty(_item.data.EffectDescription))
        {
            itemEffect.gameObject.SetActive(true);
            itemEffect.text = _item.data.EffectDescription;
        } else itemEffect.gameObject.SetActive(false);

        if (string.IsNullOrEmpty(_item.data.EquipArea) && string.IsNullOrEmpty(_item.data.EffectDescription)) contour.SetActive(false);
        else contour.SetActive(true);
    }

    public void HideInfo()
    {
        gameObject.SetActive(false);
    }
}
