using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestBase : MonoBehaviour
{
    public string questCode { get; protected set; }
    public EQuestType eQuestType { get; protected set; }
    public bool isMeetOnce = false;

    public abstract string SetQuestText();

    public abstract bool CheckMeetCondition();

}
