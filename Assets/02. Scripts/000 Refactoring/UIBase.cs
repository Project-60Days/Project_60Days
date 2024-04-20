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
    /// Init Panel (call Once on Awake, first time)
    /// </summary>
    public abstract void Init();

    /// <summary>
    /// is Panel need Refresh when it opened?
    /// </summary>
    /// <returns></returns>
    public abstract bool IsRefresh();

    /// <summary>
    /// Refresh Panel (need override)
    /// called every opening
    /// </summary>
    public abstract void Refresh();

    /// <summary>
    /// Open Panel (virtual, choose override)
    /// </summary>
    public virtual void OpenPanel()
    {
        if (IsAddUIStack() && !gameObject.activeSelf)
            App.Manager.UI.AddUIStack(GetUIState());

        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// Close Panel (virtual, choose override)
    /// </summary>
    public virtual void ClosePanel()
    {
        this.gameObject.SetActive(false);

        App.Manager.UI.PopUIStack();
    }
}
