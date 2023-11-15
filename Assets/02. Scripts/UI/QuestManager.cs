using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : ManagementBase
{
    int currQuestLevel;

    public override EManagerType GetManagemetType()
    {
        return EManagerType.QUEST;
    }

    public void NextQuest()
    {
        //ui°»½Å
        currQuestLevel++;
    }
    

    void SetNextQuest()
    {
        App.instance.GetQuestManager().currQuestLevel++;
        //App.instance.GetQuestManager().updateQuest();
    }

    void NullFunc()
    {

    }

    
}
