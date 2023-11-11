using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialQuest04 : QuestBase
{
    private readonly string thisCode = "tutorialQuest04";
    private readonly EQuestType thisType = EQuestType.Main;

    public TutorialQuest04()
    {
        questCode = thisCode;
        eQuestType = thisType;
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
