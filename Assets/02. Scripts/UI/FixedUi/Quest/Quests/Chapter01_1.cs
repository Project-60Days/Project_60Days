using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter01_1 : QuestBase
{
    private readonly string thisCode = "chapter01_AccessProductionStructure";
    private readonly QuestType thisType = QuestType.Main;
    private readonly int thisIndex = 0;
    private readonly int thisNextIndex = 1;

    public Chapter01_1()
    {
        questCode = thisCode;
        type = thisType;
        questIndex = thisIndex;
        nextQuestIndex = thisNextIndex;
    }

    public override IEnumerator CheckQuestComplete()
    {
        yield return new WaitUntil(() => CheckMeetCondition());
        yield return new WaitUntil(() => App.Manager.UI.isUIStatus(UIState.Normal));
        AfterQuest();
    }

    public override bool CheckMeetCondition()
    {
        return App.Manager.Map.mapCtrl.SensingProductionStructure();
    }

    public override string SetQuestText()
    {
        return "생산 공장 찾기";
    }
}
