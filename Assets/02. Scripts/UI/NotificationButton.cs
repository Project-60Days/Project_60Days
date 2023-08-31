using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NotificationButton : ButtonBase
{
    public bool isOpen;
    Button button;
    RectTransform rectTransform;
    float originPos;

    private void Start()
    {
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();
        originPos = rectTransform.anchoredPosition.x;
    }

    public void ClickButton()
    {
        if (!isOpen)
            StartCoroutine(OpenPanel());
        else
            StartCoroutine(ClosePanel());
    }
    IEnumerator OpenPanel()
    {
        button.interactable = false;
        yield return rectTransform.DOAnchorPosX(0, 0.5f);
        isOpen = true;
        button.interactable = true;
    }

    IEnumerator ClosePanel()
    {
        button.interactable = false;
        yield return rectTransform.DOAnchorPosX(originPos, 0.5f);
        isOpen = false;
        button.interactable = true;
    }
}
