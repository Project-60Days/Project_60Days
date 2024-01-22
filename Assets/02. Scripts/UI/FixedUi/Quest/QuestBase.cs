using System.Collections;
using UnityEngine;

public abstract class QuestBase : MonoBehaviour
{
    public string questCode { get; protected set; }
    public EQuestType eQuestType { get; protected set; }

    public int questIndex { get; protected set; }
    protected int nextQuestIndex;

    public abstract string SetQuestText();

    public abstract bool CheckMeetCondition();

    public virtual void AfterQuest()
    {

    }

    public IEnumerator CheckQuestComplete()
    {
        yield return new WaitUntil(() => CheckMeetCondition());
        UIManager.instance.GetQuestController().SetNextQuestIndex(eQuestType, nextQuestIndex);
        UIManager.instance.GetQuestController().StartNextQuest(this);
        AfterQuest();
    }
}
