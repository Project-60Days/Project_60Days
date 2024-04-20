using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial02 : QuestBase
{
    private readonly string thisCode = "tutorial02";
    private readonly EQuestType thisType = EQuestType.Tutorial;
    private readonly int thisIndex = 0;
    private readonly int thisNextIndex = 1;

    public Tutorial02()
    {
        questCode = thisCode;
        eQuestType = thisType;
        questIndex = thisIndex;
        nextQuestIndex = thisNextIndex;
    }
    
    public override bool CheckMeetCondition()
    {
        return App.Manager.UI.GetPanel<InventoryPanel>().CheckInventoryItem("ITEM_BATTERY");
    }

    public override string SetQuestText()
    {
        return "���� ���μ���: ���͸� ����";
    }
}
