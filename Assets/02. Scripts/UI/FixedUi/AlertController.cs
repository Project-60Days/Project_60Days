using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AlertController : MonoBehaviour
{

    [SerializeField] GameObject SelectionAlarm;
    [SerializeField] GameObject CautionAlarm;

    public void SetAlert(string _alertType, bool _isActive) 
    {
        if (_alertType == "selection")
            SelectionAlarm.SetActive(_isActive);
        else if( _alertType =="caution")
            CautionAlarm.SetActive(_isActive);
    }
}
