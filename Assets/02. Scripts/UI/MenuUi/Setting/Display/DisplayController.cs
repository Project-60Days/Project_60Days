using UnityEngine;

public class DisplayController : MonoBehaviour
{
    [SerializeField] GameObject resolutionList;

    public ScreenModeButton[] screenModeBtns;
    ResolutionButton[] resolutionBtns;

    void Start()
    {
        screenModeBtns = GetComponentsInChildren<ScreenModeButton>();
        resolutionBtns = GetComponentsInChildren<ResolutionButton>();
        SetFullScreen();
        SetResolution(resolutionBtns[2]);
    }

    public void SetFullScreen()
    {
        screenModeBtns[0].SetButtonClicked();
        screenModeBtns[1].SetButtonNormal();

        Screen.fullScreen = true;
    }

    public void SetWindowedMode()
    {
        screenModeBtns[0].SetButtonNormal();
        screenModeBtns[1].SetButtonClicked();

        Screen.fullScreen = false;
    }

    public void OpenResolutionList()
    {
        resolutionList.SetActive(true);
    }

    void CloseResolutionList()
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
        CloseResolutionList();
    }
}
