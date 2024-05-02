using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial06 : QuestBase
{
    private readonly string thisCode = "tutorial06";
    private readonly QuestType thisType = QuestType.Tutorial;
    private readonly int thisIndex = 3;
    private readonly int thisNextIndex = 4;

    public Tutorial06()
    {
        questCode = thisCode;
        type = thisType;
        questIndex = thisIndex;
        nextQuestIndex = thisNextIndex;
    }
    
    public override bool CheckMeetCondition()
    {
        return App.Manager.UI.isUIStatus(UIState.Normal);
    }

    public override string SetQuestText()
    {
        return "생존 프로세스: 도망";
    }
}
