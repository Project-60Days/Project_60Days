using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FixedPanel : UIBase, IListener
{
    [SerializeField] Button noteBtn;
    [SerializeField] Button cautionBtn;

    private void Awake()
    {
        App.Manager.Event.AddListener(EventCode.TutorialStart, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.TutorialStart:
                SetAlert(AlertType.Note, false);
                break;
        }
    }

    #region Override
    public override void Init()
    {
        SetButtonEvent();

        noteBtn.gameObject.SetActive(false);
        cautionBtn.gameObject.SetActive(false);
    }
    #endregion

    private void SetButtonEvent()
    {
        noteBtn.onClick.AddListener(() => ClickNoteEvent());
        cautionBtn.onClick.AddListener(() => ClickCautionEvent());
    }

    private void ClickNoteEvent()
    {
        switch (App.Manager.UI.CurrState)
        {
            case UIState.Normal:
                App.Manager.UI.GetPanel<NotePanel>().OpenPanel();
                break;

            case UIState.Map:
                StartCoroutine(OpenNoteInMap());
                break;
        }
    }

    private IEnumerator OpenNoteInMap()
    {
        App.Manager.Event.PostEvent(EventCode.GoToShelter, this);

        yield return new WaitUntil(() => App.Manager.UI.CurrState == UIState.Normal);

        App.Manager.UI.GetPanel<NotePanel>().OpenPanel();
    }

    private void ClickCautionEvent()
    {
        if (App.Manager.UI.CurrState != UIState.Normal) return;

        App.Manager.Event.PostEvent(EventCode.GoToMap, this);
    }

    public void SetAlert(AlertType _type, bool _isActive)
    {
        switch (_type)
        {
            case AlertType.Note:
                noteBtn.gameObject.SetActive(_isActive);
                break;

            case AlertType.Caution:
                cautionBtn.gameObject.SetActive(_isActive);
                break;
        }
    }

    public void SetAlert(string _type, bool _isActive)
    {
        SetAlert(GetType(_type), _isActive);
    }

    private AlertType GetType(string _type) => _type switch
    {
        "note" => AlertType.Note,
        "caution" => AlertType.Caution,
        _ => AlertType.Note,
    };
}
