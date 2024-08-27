using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class QuestPanel : UIBase, IListener
{
    [SerializeField] Transform questParent;
    [SerializeField] List<QuestBase> quests;

    private List<QuestBase> CurrentQuest 
        => questParent.GetComponentsInChildren<QuestBase>().ToList();

    private void Awake()
    {
        App.Manager.Event.AddListener(EventCode.TutorialEnd, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.TutorialEnd:
                StartQuest("MAIN_01");
                break;
        }
    }
    #region Override
    public override void Init() { }
    #endregion

    public void StartQuest(string _code)
    {
        GameObject obj = Instantiate(GetQuest(_code).gameObject, questParent);

        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => SortPosition())
            .Append(obj.transform.DOLocalMoveX(304f, 0.3f).SetEase(Ease.Linear));
    }

    private QuestBase GetQuest(string _code)
        => quests.Find(x => x.questCode == _code);

    /// <summary>
    /// Sort the position of quests.
    /// Make the main quest is at the top.
    /// </summary>
    private void SortPosition()
    {
        var list = CurrentQuest.OrderBy(x => x.type).ToList();

        var height = list[0].GetComponent<RectTransform>().rect.height;

        for (int i = 0; i < list.Count; i++) 
        {
            float yPos = -i * height;
            list[i].transform.DOLocalMoveY(yPos, 0f);
        }
    }

    public void EndQuest(string _currCode, string _nextCode = null)
    {
        QuestBase quest = GetCurrentQuest(_currCode);
        GameObject obj = quest.gameObject;

        obj.GetComponent<CanvasGroup>().DOFade(0.0f, 0.3f).SetLoops(5, LoopType.Yoyo).OnComplete(() =>
        {
            Destroy(obj);

            if (string.IsNullOrEmpty(_nextCode))
            {
                SortPosition();
            }
            else
            {
                StartQuest(_nextCode);
            }
        });
    }

    private QuestBase GetCurrentQuest(string _code)
        => CurrentQuest.Find(x => x.questCode == _code);
}
