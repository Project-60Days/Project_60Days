using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class MenuButtonBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    TextMeshProUGUI buttonText;
    Image buttonImage;
    Transform transform;

    Color normalTextColor = Color.white;
    Color highlightTextColor = Color.black;

    void Start()
    {
        Init();
        SetButtonNormal();
    }

    void Init()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        buttonImage = GetComponent<Image>();
        transform = GetComponent<Transform>();
    }





    void SetButtonNormal()
    {
        buttonText.color = normalTextColor;
        buttonImage.enabled = false;
    }

    void SetButtonHighlighted()
    {
        buttonText.color = highlightTextColor;
        buttonImage.enabled = true;
    }





    public void OnPointerEnter(PointerEventData eventData)
    {
        SetButtonHighlighted();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetButtonNormal();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        transform.DOLocalMoveY(231f, 1.0f);
    }
}
