using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Alarm : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] EAlarmType alarmType;

    [Header("TempForTest")] //테스트용 임시 변수들
    [SerializeField] GameObject ResultAlarm;
    [SerializeField] GameObject CautionAlarm;

    public void OnPointerClick(PointerEventData eventData)
    {
        //if (this.alarmType == EAlarmType.New)
        //EnableAlarm(ENotePageType.Select);
        if (this.alarmType == EAlarmType.Result)
            UIManager.instance.GetNoteController().OpenNote();
        //EnableAlarm(ENotePageType.Result);
        else if (this.alarmType == EAlarmType.Caution)
            UIManager.instance.GetNextDayController().FadeOutUiObjects();
    }

    void SetAlarm(EAlarmType type) //일단 임시로 만든.. 데이터 연결해서 해당 알림 띄울건지 말지 결정 후 이 함수로 알림 호출
    {
        if (this.alarmType == type)
            gameObject.SetActive(true);
    }

    //테스트용 임시 함수들
    #region Temp
    public void AddResult()
    {
        ResultAlarm.SetActive(true);
    }
    public void RemoveResult()
    {
        ResultAlarm.SetActive(false);
    }
    public void AddCaution()
    {
        CautionAlarm.SetActive(true);
    }
    public void RemoveCaution()
    {
        CautionAlarm.SetActive(false);
    }
    #endregion
}
