using UnityEngine;

public abstract class NotePage : MonoBehaviour
{
    public abstract ENotePageType GetENotePageType();
    public abstract void PlayPageAction();
    public abstract void SetNodeName(string nodeName);
    public abstract void SetPageEnabled(bool isNeedToday);
    public abstract bool GetPageEnableToday();
    public abstract void StopDialogue();
}
