using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

public class WorkBenchInteraction : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent onClickEvent;
    [SerializeField] Transform cube;
    [SerializeField] Transform cubeElse;

    [SerializeField] GameObject border;

    float cubeInitPositionY;
    float cubeElseInitPositionY;

    void Start()
    {
        cubeInitPositionY = cube.position.y;
        cubeElseInitPositionY = cubeElse.position.y;

        SetOutline(false);
    }

    /// <summary>
    /// 작업대 클릭 시 이벤트 발생 함수
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        SetOutline(false);
        onClickEvent?.Invoke();
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

    public void StartAnim()
    {
        cube.DOMoveY(cubeInitPositionY + 1f, 5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        cubeElse.DOMoveY(cubeElseInitPositionY + 2f, 5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }
}
