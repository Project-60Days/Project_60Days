using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLightPanel : UIBase
{
    [SerializeField] HighLight[] hightLights;
    [SerializeField] RectTransform highLightImg;

    private Dictionary<string, HighLight> dic_highLights = new Dictionary<string, HighLight>();

    #region Override
    public override void Init()
    {
        foreach (var hightLight in hightLights)
        {
            dic_highLights.Add(hightLight.objectID, hightLight);
        }
    }

    public override void ReInit() { }
    #endregion

    public void ShowHighLight(string _objectID, string _state)
    {
        if (dic_highLights.TryGetValue(_objectID, out HighLight hightLight)) 
        {
            highLightImg.sizeDelta = hightLight.area.sizeDelta;

            SetPosition(hightLight);

            StartCoroutine(WaitUntilState(_state));
        }
    }

    private void SetPosition(HighLight _highLight)
    {
        Vector2 canvasPosition = _highLight.area.position;

        highLightImg.position = canvasPosition;

        highLightImg.gameObject.SetActive(true);
    }

    private IEnumerator WaitUntilState(string _state)
    {
        UIState state = App.Manager.UI.StringToState(_state);
        yield return new WaitUntil(() => App.Manager.UI.CurrState == state);

        highLightImg.gameObject.SetActive(false);
    }

    #region Craft In Tutorial
    public void ShowCraftHighLight(string _objectID)
    {
        if (dic_highLights.TryGetValue(_objectID, out HighLight highLight))
        {
            highLightImg.sizeDelta = highLight.area.sizeDelta;

            SetPosition(highLight);

            switch (_objectID)
            {
                case "CraftItems":
                    StartCoroutine(WaitCraftItems());
                    break;

                case "ResultItem":
                    StartCoroutine(WaitResultItem());
                    break;
            }
        }
    }

    private IEnumerator WaitCraftItems()
    {
        yield return new WaitUntil(() => App.Manager.UI.GetPanel<CraftPanel>().Craft.IsCombinedResult);

        highLightImg.gameObject.SetActive(false);
        ShowCraftHighLight("ResultItem");
    }

    private IEnumerator WaitResultItem()
    {
        yield return new WaitUntil(() => App.Manager.UI.GetPanel<InventoryPanel>().CheckInventoryItem("ITEM_BATTERY"));

        highLightImg.gameObject.SetActive(false);
    }
    #endregion
}
