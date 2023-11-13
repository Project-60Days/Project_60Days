using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial06 : QuestBase
{
    private readonly string thisCode = "tutorial06";
    private readonly EQuestType thisType = EQuestType.Tutorial;
    private readonly int thisIndex = 3;
    private readonly int thisNextIndex = 4;

    public Tutorial06()
    {
        questCode = thisCode;
        eQuestType = thisType;
        questIndex = thisIndex;
        nextQuestIndex = thisNextIndex;
    }
    
    public override bool CheckMeetCondition()
    {
        return UIManager.instance.isUIStatus("UI_NORMAL");
    }

    public override string SetQuestText()
    {
        return "생존 프로세스: 도망";
    }
}
