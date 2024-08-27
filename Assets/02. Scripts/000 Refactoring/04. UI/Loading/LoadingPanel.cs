using UnityEngine;

public class LoadingPanel : UIBase, IListener
{
    private void Awake()
    {
        App.Manager.Event.AddListener(EventCode.GameStart, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.GameStart:
                ClosePanel();
                break;
        }
    }

    #region Override
    public override void Init()
    {
        OpenPanel();
    }

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
