using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial04 : QuestBase
{
    private readonly string thisCode = "tutorial04";
    private readonly EQuestType thisType = EQuestType.Tutorial;
    private readonly int thisIndex = 2;
    private readonly int thisNextIndex = 3;

    public Tutorial04()
    {
        questCode = thisCode;
        eQuestType = thisType;
        questIndex = thisIndex;
        nextQuestIndex = thisNextIndex;
    }
    
    public override bool CheckMeetCondition()
    {
        return UIManager.instance.isUIStatus("UI_MAP");
    }

    public override string SetQuestText()
    {
        return "생존 프로세스: 글리처";
    }
}
