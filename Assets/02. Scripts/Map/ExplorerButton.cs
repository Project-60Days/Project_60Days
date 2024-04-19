using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ExplorerButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    bool isMouseEnter = false;
    [SerializeField] string text;

    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(Explorer);
    }

    void Update()
    {
        if (isMouseEnter == true)
            ShowItemInfo();
    }

    public virtual void ShowItemInfo()
    {
        Vector3 mousePos = Input.mousePosition;
        App.Manager.UI.GetInfoController().ShowItemInfo(text, mousePos);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isMouseEnter = false;
        App.Manager.UI.GetInfoController().HideInfo();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isMouseEnter == false && App.Manager.UI.isUIStatus(UIState.Map))
        {
            isMouseEnter = true;
            App.Manager.UI.GetInfoController().isNew = true;
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isMouseEnter == true)
        {
            isMouseEnter = false;
            App.Manager.UI.GetInfoController().HideInfo();
        }
    }
    

    public void Explorer()
    {
        if (App.Manager.UI.GetInventoryController().CheckFindorUsage())
        {
            Debug.Log("탐색기 있음");
            if (App.Manager.Map.CheckCanInstallDrone())
            {
                Debug.Log("탐색기 설치 가능");

                App.Manager.Map.mapController.PreparingExplorer(true);
            }
        }
        else
        {
            return;
        }
    }
}
