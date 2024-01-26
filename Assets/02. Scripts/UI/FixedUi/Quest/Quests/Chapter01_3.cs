using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter01_3 : QuestBase
{
    private readonly string thisCode = "chapter01_ConnectNetworkChip";
    private readonly EQuestType thisType = EQuestType.Main;
    private readonly int thisIndex = 2;
    private readonly int thisNextIndex = -1;

    public Chapter01_3()
    {
        questCode = thisCode;
        eQuestType = thisType;
        questIndex = thisIndex;
        nextQuestIndex = thisNextIndex;
    }

    public override IEnumerator CheckQuestComplete()
    {
        yield return new WaitUntil(() => CheckMeetCondition());
        yield return new WaitUntil(() => UIManager.instance.isUIStatus("UI_LOADING"));
        AfterQuest();
    }

    public override bool CheckMeetCondition()
    {
        return UIManager.instance.GetPageController().isClickYesBtnInTower;
    }

    public override string SetQuestText()
    {
        return "넷 카드 연결하기";
    }

    public override void AfterQuest()
    {
        UIManager.instance.GetQuestController().SetNextQuestIndex(eQuestType, nextQuestIndex);
        UIManager.instance.GetQuestController().StartNextQuest(this);
        StartCoroutine(UIManager.instance.GetPopUpController().EndGamePopUp());
    }
}

