using UnityEngine;

public class FocusBase : MonoBehaviour
{
    public string objectID;
    public RectTransform area;
    public UIState state;

    public virtual bool CheckCondition()
        => App.Manager.UI.CurrState == state;

    public virtual void OnFinish() { }
}
