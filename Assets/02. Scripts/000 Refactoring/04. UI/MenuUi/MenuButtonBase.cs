using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class MenuButtonBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public bool isClicked = false;

    protected TextMeshProUGUI buttonText;
    protected Image buttonImage;

    public float startPositionY { get; private set; }

    void Awake()
    {
        Set();
        Init();
        startPositionY = transform.localPosition.y;
    }

    public virtual void Set()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        buttonImage = GetComponent<Image>();
    }

    public virtual void Init()
    {
        SetButtonState(false);
    }

    public virtual void ResetPosition()
    {
        transform.DOLocalMoveY(startPositionY, 0f);
    }




    protected void SetButtonState(bool _isActive)
    {
        buttonText.color = _isActive ? Color.black : Color.white;
        buttonImage.enabled = _isActive;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isClicked) return;

        SetButtonState(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isClicked) return;

        SetButtonState(false);
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (isClicked) return;

        App.Manager.Sound.PlaySFX("SFX_Button_1");
        ClickEvent();
    }

    public virtual void ClickEvent()
    {
        SetButtonState(false);
    }

    public virtual void CloseEvent()
    {

    }
}
