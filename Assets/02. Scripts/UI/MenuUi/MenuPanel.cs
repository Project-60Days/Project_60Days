using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuPanel : UIBase
{
    [SerializeField] Transform text;
    [SerializeField] Transform saveDetails;
    [SerializeField] Transform loadDetails;
    [SerializeField] Transform settingDetails;
    MenuButtonBase[] buttons;

    [SerializeField] Button backBtn;
    [SerializeField] Button saveBtn;
    [SerializeField] Button loadBtn;
    [SerializeField] Button optionBtn;
    [SerializeField] Button quitBtn;

    float textInitialY;
    float saveInitialY;
    float loadInitialY;
    float settingInitialY;

    #region Override
    public override void Init()
    {
        buttons = GetComponentsInChildren<MenuButtonBase>();

        textInitialY = text.transform.localPosition.y;
        saveInitialY = saveDetails.transform.localPosition.y;
        loadInitialY = loadDetails.transform.localPosition.y;
        settingInitialY = settingDetails.transform.localPosition.y;

        SetButtonEvent();

        gameObject.SetActive(false);
    }

    public override void ReInit() { }

    public override UIState GetUIState() => UIState.Note;

    public override bool IsAddUIStack() => true;

    public override void OpenPanel()
    {
        base.OpenPanel();

        foreach (var button in buttons)
            button.Init();
    }

    public override void ClosePanel()
    {
        foreach (var button in buttons)
        {
            if (button.isClicked == true)
                button.CloseEvent();
        }

        base.ClosePanel();
    }
    #endregion

    void SetButtonEvent()
    {
        backBtn.onClick.AddListener(() => ClosePanel());
        // optionBtn.onClick.AddListener(() =>);
        quitBtn.onClick.AddListener(() => Application.Quit());
    }

    public void InitSettingButtonsLocation()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].gameObject.name == "Setting_Btn") continue;
            buttons[i].gameObject.GetComponent<Transform>().DOLocalMoveY(buttons[i].initialY, 0f);
        }

        text.localPosition = new Vector3(text.localPosition.x, textInitialY, text.localPosition.z);
        saveDetails.localPosition = new Vector3(saveDetails.localPosition.x, saveInitialY, saveDetails.localPosition.z);
        loadDetails.localPosition = new Vector3(loadDetails.localPosition.x, loadInitialY, loadDetails.localPosition.z);
        settingDetails.localPosition = new Vector3(settingDetails.localPosition.x, settingInitialY, settingDetails.localPosition.z);
    }
}
