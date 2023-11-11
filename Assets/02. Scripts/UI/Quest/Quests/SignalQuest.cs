using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalQuest : QuestBase
{
    private readonly string thisCode = "signalQuest";
    private readonly EQuestType thisType = EQuestType.Main;

    public SignalQuest()
    {
        questCode = thisCode;
        eQuestType = thisType;
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
