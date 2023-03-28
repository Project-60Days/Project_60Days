using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OpenedButton_Anim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Text buttonText;
    private Image buttonImage;

    private Color normalTextColor = Color.cyan;
    private Color highlightTextColor = Color.white;

    private void Start()
    {
        // 하위 오브젝트에서 Text와 Image 컴포넌트를 가져옵니다.
        buttonText = GetComponentInChildren<Text>();
        buttonImage = GetComponentInChildren<Image>();

        // 초기 상태에서는 Text의 색은 cyan이고 Image는 안보입니다.
        buttonText.color = normalTextColor;
        buttonImage.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Highlighted 상태일 때 Text의 색을 흰색으로 바꾸고 Image를 보이게 합니다.
        buttonText.color = highlightTextColor;
        buttonImage.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Normal한 상태로 돌아가면 Text의 색은 cyan으로 바꾸고 Image를 안보이게 합니다.
        buttonText.color = normalTextColor;
        buttonImage.enabled = false;
    }
}