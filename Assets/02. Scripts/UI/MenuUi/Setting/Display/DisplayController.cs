using UnityEngine;

public class DisplayController : MonoBehaviour
{
    [SerializeField] GameObject resolutionList;

    ScreenModeButton[] screenModeBtns;
    ResolutionButton[] resolutionBtns;

    void Awake()
    {
        screenModeBtns = GetComponentsInChildren<ScreenModeButton>();
        resolutionBtns = GetComponentsInChildren<ResolutionButton>();

        SetWindowedMode();
        SetResolution(resolutionBtns[2]);
    }

    public void SetFullScreen()
    {
        screenModeBtns[0].SetButtonNormal();
        screenModeBtns[1].SetButtonClicked();

        Screen.fullScreen = true;
    }

    public void SetWindowedMode()
    {
        screenModeBtns[0].SetButtonClicked();
        screenModeBtns[1].SetButtonNormal(); //이 버튼만 선택 시 배경이 검정색이 되어서 ..

        Screen.fullScreen = false;
    }

    public void OpenResolutionList()
    {
        resolutionList.SetActive(true);
    }

    public void CloseResolutionList()
    {
        resolutionList.SetActive(false);
    }

    public void SetResolution(ResolutionButton _resolutionButton)
    {
        foreach (var btn in resolutionBtns)
            btn.SetButtonNormal();
        _resolutionButton.SetButtonClicked();
        int width = _resolutionButton.width;
        int height = _resolutionButton.height;
        Screen.SetResolution(width, height, Screen.fullScreen);
        App.Manager.UI.GetPanel<MenuPanel>().InitSettingButtonsLocation();
        CloseResolutionList();
    }
}
