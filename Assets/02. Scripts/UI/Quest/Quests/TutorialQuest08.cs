using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialQuest08 : QuestBase
{
    private readonly string thisCode = "tutorialQuest08";
    private readonly EQuestType thisType = EQuestType.Main;

    public TutorialQuest08()
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
        return "데이터 복원 시스템 가동";
    }
}
