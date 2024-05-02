using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter01_4 : QuestBase
{
    private readonly string thisCode = "chapter01_AccessSignal";
    private readonly QuestType thisType = QuestType.Main;
    private readonly int thisIndex = 3;
    private readonly int thisNextIndex = 4;

    public Chapter01_4()
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
        return App.Manager.Map.mapCtrl.SensingSignalTower();
    }

    public override string SetQuestText()
    {
        return "송신 탑 찾기";
    }
}
