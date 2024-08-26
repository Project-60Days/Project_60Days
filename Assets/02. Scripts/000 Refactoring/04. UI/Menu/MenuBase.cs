using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class MenuBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public bool IsClicked { get; protected set; } = false;

    protected float startPositionY;

    private TextMeshProUGUI buttonText;
    private Image buttonImage;

    private void Awake()
    {
        Set();
        Init();
        startPositionY = transform.localPosition.y;
    }

    protected virtual void Set()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        buttonImage = GetComponent<Image>();
    }

    public virtual void Init()
    {
        SetButtonState(false);
    }

    public void ResetPosition()
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
        if (IsClicked) return;

        SetButtonState(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsClicked) return;

        SetButtonState(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsClicked) return;

        App.Manager.Sound.PlaySFX("SFX_Button_1");
        ClickEvent();
    }

    protected virtual void ClickEvent()
    {
        SetButtonState(false);
    }

    public virtual void CloseEvent() { }
}
