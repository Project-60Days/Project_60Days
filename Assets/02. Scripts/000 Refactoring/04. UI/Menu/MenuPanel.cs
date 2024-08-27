using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuPanel : UIBase
{ 
    [SerializeField] Transform[] objects;
    private MenuBase[] buttons;
    private float[] objectStartPositionY;

    [SerializeField] Button backBtn;
    [SerializeField] Button quitBtn;

    #region Override
    public override void Init()
    {
        buttons = GetComponentsInChildren<MenuBase>();

        objectStartPositionY = new float[4];

        for (int i = 0; i < objects.Length; i++)
        {
            objectStartPositionY[i] = objects[i].localPosition.y;
        }

        SetButtonEvent();

        gameObject.SetActive(false);
    }

    public override void ReInit() { }

    public override UIState GetUIState() => UIState.Menu;

    public override bool IsAddUIStack() => true;

    public override void OpenPanel()
    {
        base.OpenPanel();

        foreach (var button in buttons)
        {
            button.Init();
        }
    }

    public override void ClosePanel()
    {
        foreach (var button in buttons)
        {
            if (button.IsClicked == true)
                button.CloseEvent();
        }

        base.ClosePanel();
    }
    #endregion

    private void SetButtonEvent()
    {
        backBtn.onClick.AddListener(() => ClosePanel());
        quitBtn.onClick.AddListener(() => Application.Quit());
    }

    public void ResetLocation()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].gameObject.name == "Setting_Btn") continue;
            buttons[i].ResetPosition();
        }

        for (int i = 0; i < objects.Length; i++) 
        {
            objects[i].DOLocalMoveY(objectStartPositionY[i], 0f);
        }
    }
}
