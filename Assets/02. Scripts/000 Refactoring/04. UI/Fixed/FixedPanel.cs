using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FixedPanel : UIBase
{
    [SerializeField] Button noteBtn;
    [SerializeField] Button cautionBtn;

    [SerializeField] IconMap mapIcon;

    #region Override
    public override void Init()
    {
        SetButtonEvent();

        noteBtn.gameObject.SetActive(false);
        cautionBtn.gameObject.SetActive(false);
    }

    public override void ReInit() 
    {
        mapIcon.ResetIcon();
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
        App.Manager.Game.GoToShelter();

        yield return new WaitUntil(() => App.Manager.UI.CurrState == UIState.Normal);

        App.Manager.UI.GetPanel<NotePanel>().OpenPanel();
    }

    private void ClickCautionEvent()
    {
        if (App.Manager.UI.CurrState != UIState.Normal) return;

        App.Manager.Game.GoToMap();
    }

    public void SetAlert(string _type, bool _isActive)
    {
        SetAlert(GetType(_type), _isActive);
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

    private AlertType GetType(string _type) => _type switch
    {
        "note" => AlertType.Note,
        "caution" => AlertType.Caution,
        _ => AlertType.Note,
    };
}
