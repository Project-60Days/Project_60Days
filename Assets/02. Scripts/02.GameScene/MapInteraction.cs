using Cinemachine;
using DG.Tweening;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MapInteraction : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent onClickEvent;

    [SerializeField] GameObject border;

    void Start()
    {
        SetOutline(false);
    }

    /// <summary>
    /// 지도 클릭 시 이벤트 발생 함수
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (App.Manager.UI.isUIStatus(UIState.Normal) == false) return;
        SetOutline(false);
        App.Manager.UI.GetNextDayController().GoToMap();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (App.Manager.UI.isUIStatus(UIState.Normal) == true)
            SetOutline(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (App.Manager.UI.isUIStatus(UIState.Normal) == true)
            SetOutline(false);
    }

    void SetOutline(bool _isEnabled)
    {
        border.SetActive(_isEnabled);
    }
}