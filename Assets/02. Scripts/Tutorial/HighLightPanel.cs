using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLightPanel : UIBase
{
    [SerializeField] HighLight[] hightLights;
    [SerializeField] GameObject highLightImg;
    public Dictionary<string, HighLight> dic_highLights = new Dictionary<string, HighLight>();

    #region Override
    public override void Init()
    {
        foreach (var h in hightLights)
        {
            dic_highLights.Add(h.objectID, h);
        }
    }

    public override void ReInit() { }
    #endregion

    public void ShowHighLight(string _objectID, string _waitUntilStatusName)
    {
        if(dic_highLights.TryGetValue(_objectID, out HighLight h))
        {
            highLightImg.GetComponent<RectTransform>().sizeDelta = h.area.sizeDelta;

            StartCoroutine(WaitForPositionUpdate(h));

            StartCoroutine(HideHighLightWhenAction(h, _waitUntilStatusName));
        }
        else
        {
            Debug.LogError($"invalid highlight object name : {_objectID}");
        }
    }

    private IEnumerator WaitForPositionUpdate(HighLight h)
    {
        yield return new WaitForEndOfFrame();

        Vector2 canvasPosition = h.area.position;

        highLightImg.GetComponent<RectTransform>().position = canvasPosition;

        highLightImg.SetActive(true);
        h.Show();
    }

    private IEnumerator HideHighLightWhenAction(HighLight _h, string _waitUntilStatusName)
    {
        UIState state = App.Manager.UI.StringToState(_waitUntilStatusName);
        yield return new WaitUntil(() => App.Manager.UI.isUIStatus(state));

        _h.Hide();
        highLightImg.SetActive(false);
    }

    public void ShowBtnHighLight(string _objectID)
    {
        if (dic_highLights.TryGetValue(_objectID, out HighLight h))
        {
            highLightImg.GetComponent<RectTransform>().sizeDelta = h.area.sizeDelta;

            StartCoroutine(WaitForPositionUpdate(h));

            if (_objectID == "ClickCraftItems")
                StartCoroutine(HideClickCraftItems(h));
            else if (_objectID == "ClickResultItem")
                StartCoroutine(HideClickResultItem(h));
        }
        else
        {
            Debug.LogError($"invalid highlight object name : {_objectID}");
        }
    }

    private IEnumerator HideClickCraftItems(HighLight _h)
    {
        yield return new WaitUntil(() => App.Manager.UI.GetPanel<CraftPanel>().Craft.IsCombinedResult);

        _h.Hide();
        highLightImg.SetActive(false);
        ShowBtnHighLight("ClickResultItem");
    }

    private IEnumerator HideClickResultItem(HighLight _h)
    {
        yield return new WaitUntil(() => App.Manager.UI.GetPanel<InventoryPanel>().CheckInventoryItem("ITEM_BATTERY"));

        _h.Hide();
        highLightImg.SetActive(false);
    }
}
