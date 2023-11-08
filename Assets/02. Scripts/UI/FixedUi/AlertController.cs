using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AlertController : MonoBehaviour
{

    [SerializeField] GameObject selectionAlarm;
    [SerializeField] GameObject cautionAlarm;

    public void SetAlert(string _alertType, bool _isActive) 
    {
        if (_alertType == "selection")
            selectionAlarm.SetActive(_isActive);
        else if( _alertType =="caution")
            cautionAlarm.SetActive(_isActive);
    }

    public void InitAlert()
    {
        selectionAlarm.SetActive(false);
        cautionAlarm.SetActive(false);
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
