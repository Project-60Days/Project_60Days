using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialQuest07 : QuestBase
{
    private readonly string thisCode = "tutorialQuest07";
    private readonly EQuestType thisType = EQuestType.Main;

    public TutorialQuest07()
    {
        questCode = thisCode;
        eQuestType = thisType;
    }
    
    public override bool CheckMeetCondition()
    {
        return UIManager.instance.isUIStatus("UI_NOTE");
    }

    public override string SetQuestText()
    {
        return "생존 프로세스: 수집";
    }
}
