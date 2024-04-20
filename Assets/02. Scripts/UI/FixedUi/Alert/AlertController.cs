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
        if (App.Manager.UI.isUIStatus(UIState.Normal) == true)
            App.Manager.UI.GetPanel<NotePanel>().OpenPanel();
        else if (App.Manager.UI.isUIStatus(UIState.Map) == true)
            StartCoroutine(OpenNoteInMap());
        else
            return;
    }

    IEnumerator OpenNoteInMap()
    {
        App.Manager.UI.GetNextDayController().GoToLab();
        yield return new WaitUntil(() => App.Manager.UI.isUIStatus(UIState.Normal));
        App.Manager.UI.GetPanel<NotePanel>().OpenPanel();
    }

    public void CautionAlert()
    {
        if (App.Manager.UI.isUIStatus(UIState.Normal) == false) return;
        App.Manager.UI.GetNextDayController().GoToMap();
    }
}
