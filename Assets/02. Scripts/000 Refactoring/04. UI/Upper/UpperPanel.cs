using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UpperPanel : UIBase, IListener
{
    [Serializable]
    public struct ItemText
    {
        public string Code;
        public TextMeshProUGUI Text;
    }

    [Header("ItemCountText")]
    [SerializeField] TextMeshProUGUI durabilityText;
    [SerializeField] List<ItemText> itemTextList;

    private readonly Color cyan = new(56f / 255f, 221f / 255f, 205f / 255f);
    private readonly Color red = Color.red;

    private Dictionary<string, ItemBase> itemBaseDic;
    private Dictionary<string, TextMeshProUGUI> itemTextDic;

    private void Awake()
    {
        App.Manager.Event.AddListener(EventCode.Hit, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.Hit:
                PlayDurabilityAnim(false);
                break;
        }
    }

    #region Override
    public override void Init()
    {
        itemBaseDic = new(itemTextList.Count);
        itemTextDic = new(itemTextList.Count);

        foreach (var text in itemTextList)
        {
            itemTextDic[text.Code] = text.Text;
        }

        itemTextList.Clear();

        foreach (var item in App.Manager.Game.itemData)
        {
            if (itemTextDic.ContainsKey(item.data.Code))
            {
                itemBaseDic[item.data.Code] = item;
            }
        }

        UpdateAllItemCount();
        UpdateDurability();
    }
    #endregion

    public void UpdateAllItemCount()
    {
        foreach (var itemBase in itemBaseDic)
        {
            var itemCode = itemBase.Key;
            var item = itemBase.Value;

            itemTextDic[itemCode].text = item.itemCount.ToString("D3");
        }
    }

    private void UpdateItemCount(string itemCode)
    {
        if (itemBaseDic.ContainsKey(itemCode))
        {
            itemTextDic[itemCode].text = itemBaseDic[itemCode].itemCount.ToString("D3");
        }
    }

    private void UpdateDurability()
    {
        durabilityText.text = App.Manager.Game.durability.ToString("D3");
    }

    public void UpdateAfterFight()
    {
        UpdateItemCount("ITEM_BULLET");
        UpdateDurability();
    }

    public void PlayDurabilityAnim(bool increase = true)
    {
        int endNumber = App.Manager.Game.durability;
        int currentNumber = int.Parse(durabilityText.text);

        float duration = increase ? 1f : 2f;

        if (!increase)
        {
            durabilityText.color = red;
        }

        DOTween.To(() => currentNumber, x => currentNumber = x, endNumber, duration)
            .OnUpdate(() => durabilityText.text = currentNumber.ToString())
            .OnComplete(() => durabilityText.color = cyan);
    }
}
