using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetNextDay : MonoBehaviour
{
    [SerializeField] Image blackPanel;
    [SerializeField] Button nextDayBtn;

    [SerializeField] NotePage[] pages;

    void Start()
    {
        nextDayBtn.onClick.AddListener(NextDayEvent);
        Init();
    }

    private void Init()
    {
        blackPanel.DOFade(0f, 0f);
        blackPanel.gameObject.SetActive(false);
        InitPageEnabled();
    }

    void NextDayEvent()
    {
        UIManager.instance.GetNoteController().CloseNote();
        blackPanel.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(blackPanel.DOFade(1f, 0.5f)).SetEase(Ease.InQuint)
            .AppendInterval(0.5f)
            .Append(blackPanel.DOFade(0f, 0.5f + 0.5f))
            .OnComplete(() => NextDayEventCallBack());
        sequence.Play();

    }

    void NextDayEventCallBack()
    {
        blackPanel.gameObject.SetActive(false);
        UIManager.instance.GetNoteController().SetNextDay();
        InitPageEnabled();
        App.instance.GetMapManager().AllowMouseEvent(true);
        MapController.instance.NextDay();
        GameManager.instance.SetPrioryty(false);
    }

    void InitPageEnabled()
    {
        foreach(NotePage page in pages)
        {
            page.SetPageEnabled(false);
            if (page.GetENotePageType()==ENotePageType.DayStart || page.GetENotePageType() == ENotePageType.SelectEvent)
            {
                page.StopDialogue();
            }
        }
    }

    public NotePage[] GetNotePageArray()
    {
        List<NotePage> todayPages = new List<NotePage>();
        
        foreach (NotePage page in pages)
        {
            if (page.GetPageEnabled())
            {
                todayPages.Add(page);
            }
        }

        return todayPages.ToArray(); ;
    }
}
