using System;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    public virtual Type GetPanelType() => GetType();

    public virtual UIState GetUIState() => UIState.Normal;

    /// <summary>
    /// is Panel need to add ui stack?
    /// </summary>
    /// <returns></returns>
    public virtual bool IsAddUIStack() => false;

    /// <summary>
    /// is Panel can close with ESC?
    /// </summary>
    /// <returns></returns>
    public virtual bool IsCloseWithESC() => false;

    /// <summary>
    /// Initialize Panel 
    /// called once on Awake, first time
    /// </summary>
    public abstract void Init();

    /// <summary>
    /// Re Initialize Panel
    /// called every new day
    /// </summary>
    public abstract void ReInit();

    /// <summary>
    /// Open Panel (virtual, choose override)
    /// </summary>
    public virtual void OpenPanel()
    {
        if (IsAddUIStack() && !gameObject.activeSelf)
            App.Manager.UI.AddUIStack(GetUIState());

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Close Panel (virtual, choose override)
    /// </summary>
    public virtual void ClosePanel()
    {
        gameObject.SetActive(false);

        if (IsAddUIStack()) 
            App.Manager.UI.PopUIStack(GetUIState());
    }
}
