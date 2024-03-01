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
        if (UIManager.instance.isUIStatus("UI_NORMAL") == false) return;
        SetOutline(false);
        UIManager.instance.GetNextDayController().GoToMap();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (UIManager.instance.isUIStatus("UI_NORMAL") == true)
            SetOutline(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (UIManager.instance.isUIStatus("UI_NORMAL") == true)
            SetOutline(false);
    }

    void SetOutline(bool _isEnabled)
    {
        border.SetActive(_isEnabled);
    }
}