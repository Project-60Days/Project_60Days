using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class InteractBase : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public UnityEvent onClickEvent;

    [SerializeField] GameObject border;

    private void Start()
    {
        onClickEvent.AddListener(OnClickEvent);

        SetOutline(false);
    }

    /// <summary>
    /// Event function called when an interactive object is clicked
    /// </summary>
    protected abstract void OnClickEvent();

    private void SetOutline(bool _isEnabled)
    {
        border.SetActive(_isEnabled);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (App.Manager.UI.CurrState == UIState.Normal)
        {
            SetOutline(false);
            onClickEvent?.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (App.Manager.UI.CurrState == UIState.Normal)
        {
            SetOutline(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (App.Manager.UI.CurrState == UIState.Normal)
        {
            SetOutline(false);
        }
    }
}
