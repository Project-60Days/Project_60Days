using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter01_2 : QuestBase
{
    private readonly string thisCode = "chapter01_ConnectProductionStructure";
    private readonly EQuestType thisType = EQuestType.Main;
    private readonly int thisIndex = 1;
    private readonly int thisNextIndex = 2;

    public Chapter01_2()
    {
        questCode = thisCode;
        eQuestType = thisType;
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
        return App.Manager.UI.GetPageController().isClickYesBtnInProductionStructure;
    }

    public override string SetQuestText()
    {
        return "노트를 열어 생산 공장 조사하기";
    }
}
