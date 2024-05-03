using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MenuButtonBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public bool isClicked = false;

    protected TextMeshProUGUI buttonText;
    protected Image buttonImage;

    Color normalTextColor = Color.white;
    Color highlightTextColor = Color.black;

    public float initialY { get; private set; }

    void Awake()
    {
        Set();
        Init();
        initialY = transform.localPosition.y;
    }

    public virtual void Set()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        buttonImage = GetComponent<Image>();
    }

    public virtual void Init()
    {
        SetButtonNormal();
    }




    protected void SetButtonNormal()
    {
        buttonText.color = normalTextColor;
        buttonImage.enabled = false;
    }

    protected void SetButtonHighlighted()
    {
        buttonText.color = highlightTextColor;
        buttonImage.enabled = true;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isClicked == false) 
            SetButtonHighlighted();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isClicked == false)
            SetButtonNormal();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (isClicked == false)
        {
            App.Manager.Sound.PlaySFX("SFX_Button_1");
            ClickEvent();
        }
    }

    public virtual void ClickEvent()
    {
        SetButtonNormal();
    }

    public virtual void CloseEvent()
    {

    }
}
