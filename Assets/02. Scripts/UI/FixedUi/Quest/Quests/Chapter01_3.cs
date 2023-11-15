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

    public override bool CheckMeetCondition()
    {
        return true;
    }

    public override string SetQuestText()
    {
        return "송신기 접근 어쩌구 퀘스트";
    }
}

