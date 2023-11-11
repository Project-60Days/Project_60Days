using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter01_2 : QuestBase
{
    private readonly string thisCode = "chapter01_AccessSignal";
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

    public override bool CheckMeetCondition()
    {
        return UIManager.instance.GetInventoryController().CheckInventoryItem("ITEM_NETWORKCHIP");
    }

    public override string SetQuestText()
    {
        return "송신기 접근 어쩌구 퀘스트";
    }
}
