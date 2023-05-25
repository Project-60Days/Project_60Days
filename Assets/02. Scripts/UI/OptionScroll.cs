using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionScroll : MonoBehaviour
{
    public RectTransform textRect;
    public RectTransform buttonBackRect;
    public ScrollRect scrollRect;

    void Start()
    {

        float textHeight = textRect.sizeDelta.y;

        // 두 번째 텍스트의 Y 좌표를 조정합니다.
        float buttonBackHeight = textHeight;
        buttonBackRect.anchoredPosition = new Vector2(buttonBackRect.anchoredPosition.x, buttonBackHeight);

        Canvas.ForceUpdateCanvases();
        scrollRect.normalizedPosition = Vector2.zero;
    }
}
