using UnityEngine;

public class InteractCtrl : MonoBehaviour
{
    void Start()
    {
        SetOnClickEvent();
    }

    void SetOnClickEvent()
    {
        App.Manager.Shelter.Map.onClickEvent.AddListener(InteractMap);
        App.Manager.Shelter.Note.onClickEvent.AddListener(InteractNote);
        App.Manager.Shelter.Bench.onClickEvent.AddListener(InteractBench);
    }

    protected void InteractMap()
    {
        App.Manager.UI.GetNextDayController().GoToMap();
    }

    protected void InteractNote()
    {
        App.Manager.UI.GetPanel<NotePanel>().OpenPanel();
    }

    protected void InteractBench()
    {
        App.Manager.UI.GetPanel<CraftPanel>().OpenPanel();
    }
}