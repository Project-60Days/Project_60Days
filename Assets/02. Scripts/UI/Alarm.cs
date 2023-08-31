using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Alarm : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] EAlarmType alarmType;
    [SerializeField] GameObject NewAlarm;
    [SerializeField] GameObject ResultAlarm;
    [SerializeField] GameObject CautionAlarm;

    NoteController noteController;
    void Start()
    {
        noteController = UIManager.instance.GetNoteController();

        NewAlarm.SetActive(false);
        ResultAlarm.SetActive(false);
        CautionAlarm.SetActive(false);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.alarmType == EAlarmType.New)
            SetAlarm(ENotePageType.SelectEvent);
        else if (this.alarmType == EAlarmType.Result)
            SetAlarm(ENotePageType.Result);
        else if (this.alarmType == EAlarmType.Caution)
            Debug.Log("지도가 열릴 예정인");
    }

    public void SetAlarm(ENotePageType type)
    {
        int index = noteController.GetPageNum(type);
        if (index > 0)
        {
            noteController.SetPageNum(index);
            noteController.OpenNote();
        }
    }

    public void AddNew()
    {
        NewAlarm.SetActive(true);
    }
    public void RemoveNew()
    {
        NewAlarm.SetActive(false);
    }
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
}
