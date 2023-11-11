using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AlertController : MonoBehaviour
{

    [SerializeField] GameObject noteAlert;
    [SerializeField] GameObject cautionAlert;

    public void SetAlert(string _alertType, bool _isActive) 
    {
        if (_alertType == "note")
            noteAlert.SetActive(_isActive);
        else if( _alertType =="caution")
            cautionAlert.SetActive(_isActive);
    }

    public void InitAlert()
    {
        noteAlert.SetActive(false);
        cautionAlert.SetActive(false);
    }

    public void ClickNoteAlert()
    {
        UIManager.instance.GetNoteController().OpenNote();
    }

    public void CautionAlert()
    {
        UIManager.instance.GetNextDayController().GoToMap();
    }
}
