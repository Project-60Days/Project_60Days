using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial04 : QuestBase
{
    private readonly string thisCode = "tutorial04";
    private readonly QuestType thisType = QuestType.Tutorial;
    private readonly int thisIndex = 2;
    private readonly int thisNextIndex = 3;

    public Tutorial04()
    {
        questCode = thisCode;
        type = thisType;
        questIndex = thisIndex;
        nextQuestIndex = thisNextIndex;
    }
    
    public override bool CheckMeetCondition()
    {
        return App.Manager.UI.isUIStatus(UIState.Map);
    }

    public override string SetQuestText()
    {
        return "생존 프로세스: 글리처";
    }
}
