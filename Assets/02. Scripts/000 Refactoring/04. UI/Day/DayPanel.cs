using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DayPanel : UIBase
{
    [SerializeField] TextMeshProUGUI dayTMP;

    #region Override
    public override UIState GetUIState() => UIState.NewDay;

    public override bool IsAddUIStack() => true;

    public override void Init()
    {
        gameObject.SetActive(false);
    }

    public override void ReInit()
    {
        OpenPanel();
    }

    public override void OpenPanel()
    {
        base.OpenPanel();

        PlayDayAnim();
    }

    public override void ClosePanel()
    {
        base.ClosePanel();

        dayTMP.text = string.Empty;
        dayTMP.alpha = 0f;
    }
    #endregion

    private void PlayDayAnim()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() => dayTMP.text = GetText())
            .Append(dayTMP.DOFade(1f, 0.3f).SetEase(Ease.Linear))
            .AppendInterval(1.4f)
            .Append(dayTMP.DOFade(0f, 0.3f).SetEase(Ease.Linear))
            .OnComplete(() =>
            {
                App.Manager.Event.PostEvent(EventCode.NextDayEnd, this);

                ClosePanel();
            });
    }

    private string GetText() => App.Manager.Game.TodayState switch
    {
        NewDayState.Normal => "<color=white>Day " + "{vertexp}" + App.Manager.Game.DayCount.ToString() + "{/vertexp}</color>",
        NewDayState.Hit => "<color=red><shake a=0.1>" + "Day " + "{vertexp}" + App.Manager.Game.DayCount.ToString() + "{/vertexp}</shake></color>",
        NewDayState.Die => "<color=red><shake a=0.1>GAME OVER</shake></color>",
        _ => null,
    };
}