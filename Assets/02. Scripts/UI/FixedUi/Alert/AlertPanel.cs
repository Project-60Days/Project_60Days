using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AlertPanel : UIBase
{

    [SerializeField] GameObject noteAlert;
    [SerializeField] GameObject cautionAlert;

    #region Override
    public override void Init()
    {
        SetButtonEvent();

        noteAlert.SetActive(false);
        cautionAlert.SetActive(false);
    }

    public override void ReInit()
    {
        noteAlert.SetActive(false);
        cautionAlert.SetActive(false);
    }
    #endregion

    void SetButtonEvent()
    {
        noteAlert.GetComponent<Button>().onClick.AddListener(() => App.Manager.UI.GetPanel<NotePanel>().OpenPanel());
        cautionAlert.GetComponent<Button>().onClick.AddListener(() => App.Manager.Game.ctrl.GoToMap());
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
        App.Manager.Game.ctrl.GoToLab();
        yield return new WaitUntil(() => App.Manager.UI.isUIStatus(UIState.Normal));
        App.Manager.UI.GetPanel<NotePanel>().OpenPanel();
    }

    public void CautionAlert()
    {
        if (App.Manager.UI.isUIStatus(UIState.Normal) == false) return;
        App.Manager.Game.ctrl.GoToMap();
    }
}
