using System.Collections;
using UnityEngine;

public abstract class QuestBase : MonoBehaviour
{
    public string questCode { get; protected set; }
    public QuestType type { get; protected set; }

    public int questIndex { get; protected set; }
    protected int nextQuestIndex;

    public abstract string SetQuestText();

    public abstract bool CheckMeetCondition();

    public virtual IEnumerator CheckQuestComplete()
    {
        yield return new WaitUntil(() => CheckMeetCondition());
        AfterQuest();
    }

    public virtual void AfterQuest()
    {
        App.Manager.UI.GetPanel<QuestPanel>().SetNextQuestIndex(type, nextQuestIndex);
        App.Manager.UI.GetPanel<QuestPanel>().StartNextQuest(this);
    }
}
