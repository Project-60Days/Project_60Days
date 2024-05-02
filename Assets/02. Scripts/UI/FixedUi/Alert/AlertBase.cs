using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AlertBase : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    bool isMouseEnter = false;
    [SerializeField] string alertCode;

    void Update()
    {
        if (isMouseEnter == true)
            ShowItemInfo();
    }

    public virtual void ShowItemInfo()
    {
        Vector3 mousePos = Input.mousePosition;
        App.Manager.UI.GetPanel<InfoPanel>().ShowAlertInfo(alertCode, mousePos);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isMouseEnter = false;
        App.Manager.UI.GetPanel<InfoPanel>().HideInfo();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isMouseEnter == false && App.Manager.UI.isUIStatus(UIState.Normal))
        {
            isMouseEnter = true;
            App.Manager.UI.GetPanel<InfoPanel>().isNew = true;
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isMouseEnter == true)
        {
            isMouseEnter = false;
            App.Manager.UI.GetPanel<InfoPanel>().HideInfo();
        }
    }
}
