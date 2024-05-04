using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter01_5 : QuestBase
{
    private readonly string thisCode = "chapter01_ConnectNetworkChip";
    private readonly QuestType thisType = QuestType.Main;
    private readonly int thisIndex = 4;
    private readonly int thisNextIndex = -1;

    public Chapter01_5()
    {
        questCode = thisCode;
        type = thisType;
        questIndex = thisIndex;
        nextQuestIndex = thisNextIndex;
    }

    public override IEnumerator CheckQuestComplete()
    {
        yield return new WaitUntil(() => CheckMeetCondition());
        yield return new WaitUntil(() => App.Manager.UI.CurrState == UIState.Loading);
        yield return new WaitUntil(() => App.Manager.UI.CurrState == UIState.Normal);
        AfterQuest();
    }

    public override bool CheckMeetCondition()
    {
        return App.Manager.UI.GetPanel<PagePanel>().isClickYesBtnInTower;
    }

    public override string SetQuestText()
    {
        return "넷 카드 연결하기";
    }

    public override void AfterQuest()
    {
        App.Manager.UI.GetPanel<QuestPanel>().SetNextQuestIndex(type, nextQuestIndex);
        App.Manager.UI.GetPanel<QuestPanel>().StartNextQuest(this);
        StartCoroutine(App.Manager.UI.GetPanel<EndingPanel>().EndGamePopUp());
    }
}

