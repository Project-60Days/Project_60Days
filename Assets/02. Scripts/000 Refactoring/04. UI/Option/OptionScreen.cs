using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class OptionScreen : OptionBase
{
    [Header("Screen Mode")]
    [SerializeField] TextMeshProUGUI screenModeTMP;
    [SerializeField] Button[] screenModeBtns;
    [SerializeField] TextMeshProUGUI[] btnTexts;
    [SerializeField] Sprite[] btnSprites;

    [Header("Resolution")]
    [SerializeField] TextMeshProUGUI resolutionTMP;
    [SerializeField] TMP_Dropdown resolutionDropdown;

    private List<Resolution> resolutions;
    private int resolutionIndex;

    #region Override
    protected override void Awake()
    {
        base.Awake();

        InitResolutionList();
    }

    protected override void Start()
    {
        base.Start();

        SetResolution();
    }

    protected override void SetString()
    {
        screenModeTMP.text = App.Data.Game.GetString("STR_OPTION_SCREEN_SCREENMODE");
        btnTexts[0].text = App.Data.Game.GetString("STR_OPTION_SCREEN_FULLSCREEN");
        btnTexts[1].text = App.Data.Game.GetString("STR_OPTION_SCREEN_WINDOWED");
        resolutionTMP.text = App.Data.Game.GetString("STR_OPTION_SCREEN_RESOLUTION");
    }

    protected override void SetEvent()
    {
        screenModeBtns[0].onClick.AddListener(() => FullScreenBtnEvent());
        screenModeBtns[1].onClick.AddListener(() => WindowedBtnEvent());

        resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
    }

    protected override void SetValueFromData()
    {
        int index = Setting.Screen.isFullScreen ? 0 : 1;
        ScreenBtnEvent(index);

        resolutionIndex = FindResolutionIndex(
            Setting.Screen.resolutionWidth,
            Setting.Screen.resolutionHeight);

        ChangeResolution(resolutionIndex);

        resolutionDropdown.value = resolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public override void SaveOption()
    {
        Setting.Screen = new()
        {
            isFullScreen = Screen.fullScreen,
            resolutionWidth = resolutions[resolutionIndex].width,
            resolutionHeight = resolutions[resolutionIndex].height,
        };
    }
    #endregion

    #region Screen Mode
    private void FullScreenBtnEvent()
    {
        ScreenBtnEvent(0);
        Screen.fullScreen = true;
    }

    private void WindowedBtnEvent()
    {
        ScreenBtnEvent(1);
        Screen.fullScreen = false;
    }

    private void ScreenBtnEvent(int _index)
    {
        for (int i = 0; i < screenModeBtns.Length; i++)
        {
            if (_index == i)
            {
                screenModeBtns[i].image.sprite = btnSprites[0];
                btnTexts[i].color = Color.white;
            }
            else
            {
                screenModeBtns[i].image.sprite = btnSprites[1];
                btnTexts[i].color = Color.black;
            }
        }
    }
    #endregion

    #region Resolution
    private void InitResolutionList()
    {
        resolutions = new List<Resolution>(Screen.resolutions);
        resolutions.Reverse();

        //resolutions = resolutions.FindAll(x => x.height * 16 == x.width * 9);

        //Remove all but the highest Hz among the same resolutions.
        List<Resolution> tempResolutions = new();
        int currWidth = resolutions[0].width;
        int currHeight = resolutions[0].height;

        tempResolutions.Add(resolutions[0]);

        for (int i = 0; i < resolutions.Count; i++)
        {
            if (currWidth != resolutions[i].width || currHeight != resolutions[i].height)
            {
                tempResolutions.Add(resolutions[i]);
                currWidth = resolutions[i].width;
                currHeight = resolutions[i].height;
            }
        }

        resolutions = tempResolutions;
    }

    private void SetResolution()
    {
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Count; i++)
        {
            string option = resolutions[i].width + " X " + resolutions[i].height;

            options.Add(option);
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);

        resolutionDropdown.value = resolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    #region Change Resolution
    private void ChangeResolution(int _index)
    {
        if (resolutions.Count <= _index)
        {
            Debug.LogError("ERROR: Resolution index is out of range. Setting to default");
            return;
        }

        resolutionIndex = _index;
        ChangeResolution(Setting.DefaultScreen.resolutionWidth, Setting.DefaultScreen.resolutionHeight);
    }

    private void ChangeResolution(int _width, int _height)
    {
        Screen.SetResolution(_width, _height, Setting.Screen.isFullScreen);
        App.Manager.UI.GetPanel<MenuPanel>().ResetLocation();
    }

    private int FindResolutionIndex(int _width, int _height)
    {
        var perfectMatch = resolutions.FindIndex(res =>
            res.width == _width && res.height == _height);

        if (perfectMatch != -1)
        {
            return perfectMatch; // return perfect match on found
        }

        // find resolution which has a best match
        var bestScore = int.MaxValue;
        var bestMatch = 0;
        for (int i = 0; i < resolutions.Count; ++i)
        {
            var widthDiff = Math.Abs(resolutions[i].width - _width);
            var heightDiff = Math.Abs(resolutions[i].height - _height);
            var ratioDiff = Mathf.Abs(16.0f / 9.0f - resolutions[i].width / resolutions[i].height);
            var score = (int)(ratioDiff * 1000.0f) + widthDiff + heightDiff;

            if (score < bestScore)
            {
                bestScore = score;
                bestMatch = i;
            }
        }

        return bestMatch;
    }
    #endregion

    #endregion
}
