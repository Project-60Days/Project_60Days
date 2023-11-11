using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialQuest03 : QuestBase
{
    private readonly string thisCode = "tutorialQuest03";
    private readonly EQuestType thisType = EQuestType.Main;

    public TutorialQuest03()
    {
        questCode = thisCode;
        eQuestType = thisType;
    }

    public override bool CheckMeetCondition()
    {
        return UIManager.instance.GetInventoryController().CheckInventoryItem("ITEM_TUTORIAL_BATTERY");
    }

    public override string SetQuestText()
    {
        return "생존 프로세스: 결단";
    }
}
