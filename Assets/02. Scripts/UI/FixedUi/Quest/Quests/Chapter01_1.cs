using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter01_1 : QuestBase
{
    private readonly string thisCode = "chapter01_GetNetworkChip";
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

    public override bool CheckMeetCondition()
    {
        return UIManager.instance.GetInventoryController().CheckInventoryItem("ITEM_NETWORKCHIP");
    }

    public override string SetQuestText()
    {
        return "네트워크 칩 회수하기";
    }
}