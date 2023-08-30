using UnityEngine;

public abstract class NotePage : MonoBehaviour
{
    public abstract ENotePageType GetENotePageType();
    public abstract int GetPriority();
    public abstract void playPageAction();
}
