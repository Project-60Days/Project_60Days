using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter01_3 : QuestBase
{
    private readonly string thisCode = "chapter01_GetNetworkChip";
    private readonly EQuestType thisType = EQuestType.Main;
    private readonly int thisIndex = 2;
    private readonly int thisNextIndex = 3;

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
        yield return new WaitUntil(() => App.Manager.UI.isUIStatus(UIState.Normal));
        AfterQuest();
    }

    public override bool CheckMeetCondition()
    {
        return App.Manager.UI.GetInventoryController().CheckInventoryItem("ITEM_NETWORKCHIP");
    }

    public override string SetQuestText()
    {
        return "넷 카드 회수하기";
    }
}