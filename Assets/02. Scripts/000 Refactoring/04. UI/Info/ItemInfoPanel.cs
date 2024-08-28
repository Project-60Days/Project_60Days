using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ItemInfoPanel : UIBase
{
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemDescribe;
    [SerializeField] TextMeshProUGUI itemEquip;
    [SerializeField] TextMeshProUGUI itemEffect;

    [SerializeField] InfoSlot[] infoSlots;
    [SerializeField] GameObject contour;

    private RectTransform rect;
    private CanvasGroup canvasGroup;

    private Dictionary<string, ItemBase> itemBaseDic;
    private Dictionary<string, ItemCombineData> itemCombineDic;

    private bool isDescriptionOn;

    #region Override
    public override void Init()
    {
        rect = gameObject.GetComponent<RectTransform>();
        canvasGroup = gameObject.GetComponent<CanvasGroup>();

        itemBaseDic = App.Data.Game.ITEM.ToDictionary(item => item.data.Code);
        itemCombineDic = App.Data.Game.itemCombineData.Values.ToDictionary(x => x.Result);

        ClosePanel();
    }

    public override void ClosePanel()
    {
        canvasGroup.DOKill();

        isDescriptionOn = false;

        ResetUI();
    }
    #endregion

    private void Update()
    {
        if (isDescriptionOn)
        {
            UpdateDescriptionTransform();
        }
    }

    private void UpdateDescriptionTransform()
    {
        Vector2 position = Input.mousePosition;

        if (position.x + rect.rect.width > App.Data.Setting.Screen.resolutionWidth)
        {
            position.x -= rect.rect.width;
        }

        if (position.y - rect.rect.height < 0)
        {
            position.y += rect.rect.height;
        }

        rect.position = position;
    }

    public void SetInfo(ItemBase _item)
    {
        canvasGroup.DOKill();

        canvasGroup.DOFade(0f, 0.1f).OnComplete(() =>
        {
            isDescriptionOn = true;

            ResetUI();

            UpdateUI(_item);
            UpdateBlueprint(_item);

            canvasGroup.DOFade(1f, 0.1f);
        });
    }

    private void ResetUI()
    {
        canvasGroup.alpha = 0f;

        itemName.gameObject.SetActive(false);
        itemDescribe.gameObject.SetActive(false);
        itemEquip.gameObject.SetActive(false);
        itemEffect.gameObject.SetActive(false);

        contour.SetActive(false);

        foreach (var slot in infoSlots)
        {
            slot.ResetItem();
        }
    }

    private void UpdateUI(ItemBase _item)
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

    private void UpdateBlueprint(ItemBase _item)
    {
        if (itemCombineDic.TryGetValue(_item.Code, out var combineData) == false) return;

        AddItemByItemCode(combineData.Material_1, 0);
        AddItemByItemCode(combineData.Material_2, 1);
        AddItemByItemCode(combineData.Material_3, 2);
    }

    private void AddItemByItemCode(string _itemCode, int _index)
    {
        if (_itemCode == "-1") return;

        if (itemBaseDic.TryGetValue(_itemCode, out var item))
        {
            infoSlots[_index].SetItem(item);
        }
        else
        {
            Debug.Log("아직 추가되지 않은 아이템: " + _itemCode);
        }
    }
}
