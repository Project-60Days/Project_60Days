using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusPanel : UIBase
{
    [SerializeField] FocusBase[] focus;
    [SerializeField] RectTransform focusImg;

    private Dictionary<string, FocusBase> dic_focus = new Dictionary<string, FocusBase>();

    #region Override
    public override void Init()
    {
        foreach (var hightLight in focus)
        {
            dic_focus.Add(hightLight.objectID, hightLight);
        }
    }

    public override void ReInit() { }
    #endregion

    public void ShowFocus(string _objectID)
    {
        if (dic_focus.TryGetValue(_objectID, out FocusBase focus)) 
        {
            focusImg.sizeDelta = focus.area.sizeDelta;

            SetPosition(focus);

            StartCoroutine(WaitUntilState(focus));
        }
    }

    private void SetPosition(FocusBase _focus)
    {
        Vector2 canvasPosition = _focus.area.position;
        focusImg.position = canvasPosition;

        focusImg.gameObject.SetActive(true);
    }

    private IEnumerator WaitUntilState(FocusBase _focus)
    {
        yield return new WaitUntil(() => _focus.CheckCondition());

        focusImg.gameObject.SetActive(false);

        _focus.OnFinish();
    }
}
