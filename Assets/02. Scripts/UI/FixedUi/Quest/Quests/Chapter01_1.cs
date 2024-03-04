using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter01_1 : QuestBase
{
    private readonly string thisCode = "chapter01_AccessProductionStructure";
    private readonly EQuestType thisType = EQuestType.Main;
    private readonly int thisIndex = 0;
    private readonly int thisNextIndex = 1;

    public Chapter01_1()
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
        AfterQuest();
    }

    public override bool CheckMeetCondition()
    {
        return App.instance.GetMapManager().SensingProductionStructure();
    }

    public override string SetQuestText()
    {
        return "생산 공장 찾기";
    }
}
