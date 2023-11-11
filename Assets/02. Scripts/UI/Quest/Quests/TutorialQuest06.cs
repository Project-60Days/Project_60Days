using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialQuest06 : QuestBase
{
    private readonly string thisCode = "tutorialQuest06";
    private readonly EQuestType thisType = EQuestType.Main;

    public TutorialQuest06()
    {
        questCode = thisCode;
        eQuestType = thisType;
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
