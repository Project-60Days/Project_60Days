using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AlertBase : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    bool isMouseEnter = false;
    public string alertText;

    void Update()
    {
        if (isMouseEnter == true)
            ShowItemInfo();
    }

    public virtual void ShowItemInfo()
    {
        Vector3 mousePos = Input.mousePosition;
        UIManager.instance.GetAlertInfoController().ShowInfo(alertText, mousePos);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isMouseEnter = false;
        UIManager.instance.GetAlertInfoController().HideInfo();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isMouseEnter == false && UIManager.instance.isUIStatus("UI_NORMAL"))
        {
            isMouseEnter = true;
            UIManager.instance.GetAlertInfoController().isNew = true;
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isMouseEnter == true)
        {
            UIManager.instance.GetAlertInfoController().HideInfo();
            isMouseEnter = false;
        }
    }
}
