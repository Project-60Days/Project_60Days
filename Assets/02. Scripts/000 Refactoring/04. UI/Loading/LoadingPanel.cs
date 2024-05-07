using System.Collections;
using UnityEngine;

public class LoadingPanel : UIBase
{
    #region Override
    public override void Init()
    {
        OpenPanel();
    }

    public override void ReInit() { }

    public override UIState GetUIState() => UIState.Loading;

    public override bool IsAddUIStack() => true;

    public override void OpenPanel()
    {
        App.Manager.UI.AddUIStack(GetUIState());
    }

    public override void ClosePanel()
    {
        base.ClosePanel();

        App.Manager.Sound.PlayBGM("BGM_InGame");
        App.Manager.UI.FadeOut();
    }
    #endregion
}
