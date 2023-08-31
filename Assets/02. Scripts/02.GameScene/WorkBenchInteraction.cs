using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class WorkBenchInteraction : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent onClickEvent;

    /// <summary>
    /// 작업대 클릭 시 이벤트 발생 함수
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.AddCurrUIName(StringUtility.UI_CRAFTING);
        onClickEvent?.Invoke();
    }
}
