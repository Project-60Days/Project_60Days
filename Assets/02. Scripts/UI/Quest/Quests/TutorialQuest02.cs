using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialQuest02 : QuestBase
{
    private readonly string thisCode = "tutorialQuest02";
    private readonly EQuestType thisType = EQuestType.Main;

    public TutorialQuest02()
    {
        questCode = thisCode;
        eQuestType = thisType;
    }
    
    public override bool CheckMeetCondition()
    {
        return UIManager.instance.isUIStatus("UI_CRAFTING");
    }

    public override string SetQuestText()
    {
        return "생존 프로세스: 배터리 제작";
    }
}
