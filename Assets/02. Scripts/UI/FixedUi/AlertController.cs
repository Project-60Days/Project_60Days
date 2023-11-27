using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AlertController : MonoBehaviour
{

    [SerializeField] GameObject noteAlert;
    [SerializeField] GameObject cautionAlert;

    private void Awake()
    {
        InitAlert();
    }
    public void InitAlert()
    {
        noteAlert.SetActive(false);
        cautionAlert.SetActive(false);
    }

    public void SetAlert(string _alertType, bool _isActive) 
    {
        if (_alertType == "note")
            noteAlert.SetActive(_isActive);
        else if( _alertType =="caution")
            cautionAlert.SetActive(_isActive);
    }

    public void ClickNoteAlert()
    {
        if (UIManager.instance.isUIStatus("UI_NORMAL") == false) return;
        UIManager.instance.GetNoteController().OpenNote();
    }

    public void CautionAlert()
    {
        if (UIManager.instance.isUIStatus("UI_NORMAL") == false) return;
        UIManager.instance.GetNextDayController().GoToMap();
    }
}
