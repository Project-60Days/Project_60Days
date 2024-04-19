using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial07 : QuestBase
{
    private readonly string thisCode = "tutorial07";
    private readonly EQuestType thisType = EQuestType.Tutorial;
    private readonly int thisIndex = 4;
    private readonly int thisNextIndex = -1;

    public Tutorial07()
    {
        questCode = thisCode;
        eQuestType = thisType;
        questIndex = thisIndex;
        nextQuestIndex = thisNextIndex;
    }
    
    public override bool CheckMeetCondition()
    {
        return App.Manager.UI.isUIStatus(UIState.Note);
    }

    public override string SetQuestText()
    {
        return "���� ���μ���: ����";
    }
}
