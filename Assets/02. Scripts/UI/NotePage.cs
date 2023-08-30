using UnityEngine;

public abstract class NotePage : MonoBehaviour
{
    public abstract ENotePageType GetENotePageType();
    public abstract void PlayPageAction();
    public abstract void SetPageEnabled(bool isNeedToday);
    public abstract bool GetPageEnabled();
    public abstract void StopDialogue();
}
