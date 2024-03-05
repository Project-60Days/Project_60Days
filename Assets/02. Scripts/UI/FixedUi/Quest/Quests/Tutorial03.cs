using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial03 : QuestBase
{
    private readonly string thisCode = "tutorial03";
    private readonly EQuestType thisType = EQuestType.Tutorial;
    private readonly int thisIndex = 1;
    private readonly int thisNextIndex = 2;

    public Tutorial03()
    {
        questCode = thisCode;
        eQuestType = thisType;
        questIndex = thisIndex;
        nextQuestIndex = thisNextIndex;
    }

    public override IEnumerator CheckQuestComplete()
    {
        yield return new WaitUntil(() => CheckMeetCondition());
        yield return new WaitUntil(() => UIManager.instance.isUIStatus("UI_NORMAL"));
    }

    public override bool CheckMeetCondition()
    {
        return UIManager.instance.isUIStatus("UI_NOTE");
    }

    public override string SetQuestText()
    {
        return "생존 프로세스: 결단";
    }
}
