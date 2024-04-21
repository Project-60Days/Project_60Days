using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InteractObj : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public UnityEvent onClickEvent;

    [SerializeField] GameObject border;

    void Start()
    {
        SetOutline(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (App.Manager.UI.isUIStatus(UIState.Normal) == true)
        {
            SetOutline(false);
            onClickEvent?.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (App.Manager.UI.isUIStatus(UIState.Normal) == true)
        {
            SetOutline(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (App.Manager.UI.isUIStatus(UIState.Normal) == true)
        {
            SetOutline(false);
        }
    }

    void SetOutline(bool _isEnabled)
    {
        border.SetActive(_isEnabled);
    }
}
