using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPanel : UIBase
{
    #region Override
    public override void Init()
    {
        OpenPanel();
    }

    public override void ReInit()
    {
        throw new System.NotImplementedException();
    }

    public override UIState GetUIState() => UIState.Loading;

    public override bool IsAddUIStack() => true;

    public override void OpenPanel()
    {
        base.OpenPanel();

        StartCoroutine(LoadScene());
    }

    public override void ClosePanel()
    {
        base.ClosePanel();

        App.Manager.UI.FadeOut();
    }
    #endregion

    IEnumerator LoadScene()
    {
        yield return new WaitUntil(() => App.Manager.Map.mapController != null);
        yield return new WaitUntil(() => App.Manager.Map.mapController.LoadingComplete == true);

        App.Manager.Sound.PlayBGM("BGM_InGameTheme");

        ClosePanel();
    }
}
