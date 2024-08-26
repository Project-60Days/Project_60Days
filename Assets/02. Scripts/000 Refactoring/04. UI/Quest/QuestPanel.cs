using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class QuestPanel : UIBase
{
    [SerializeField] Transform questParent;
    [SerializeField]  List<Quest> quests;

    private List<Quest> CurrentQuest 
        => questParent.GetComponentsInChildren<Quest>().ToList();

    private Quest GetQuest(string _code)
        => quests.Find(x => x.questCode == _code);
    private Quest GetCurrentQuest(string _code)
        => CurrentQuest.Find(x => x.questCode == _code);

    #region Override
    public override void Init() { }

    public override void ReInit() { }
    #endregion

    public void StartQuest(string _code)
    {
        GameObject obj = Instantiate(GetQuest(_code).gameObject, questParent);

        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => SortPosition())
            .Append(obj.transform.DOLocalMoveX(304f, 0.3f).SetEase(Ease.Linear));
    }

    /// <summary>
    /// Sort the position of quests.
    /// Make the main quest is at the top.
    /// </summary>
    private void SortPosition()
    {
        var list = CurrentQuest.OrderBy(x => x.type).Select(x => x.gameObject).ToList();

        var height = list[0].GetComponent<RectTransform>().rect.height;

        for (int i = 0; i < list.Count; i++) 
        {
            float yPos = -i * height;
            list[i].transform.DOLocalMoveY(yPos, 0f);
        }
    }

    public void EndQuest(string _currCode, string _nextCode = null)
    {
        Quest quest = GetCurrentQuest(_currCode);
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
}
