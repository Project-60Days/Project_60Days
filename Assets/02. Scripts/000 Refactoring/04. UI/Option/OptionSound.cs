using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionSound : OptionBase
{
    [Serializable]
    private struct VolumeOption
    {
        public TextMeshProUGUI titleText;
        public TMP_InputField inputField;
        public Slider slider;
    }

    [SerializeField] VolumeOption Master;
    [SerializeField] VolumeOption BGM;
    [SerializeField] VolumeOption SFX;

    private SoundManager Sound;

    #region Override
    protected override void SetString()
    {
        Master.titleText.text = App.Data.Game.GetString("STR_OPTION_SOUND_MASTER");
        BGM.titleText.text = App.Data.Game.GetString("STR_OPTION_SOUND_BGM");
        SFX.titleText.text = App.Data.Game.GetString("STR_OPTION_SOUND_SFX");
    }

    protected override void SetEvent()
    {
        Master.slider.onValueChanged.AddListener((value) => OnMasterChange(value));
        BGM.slider.onValueChanged.AddListener((value) => OnBGMChange(value));
        SFX.slider.onValueChanged.AddListener((value) => OnSFXChange(value));

        Master.inputField.onValueChanged.AddListener((value) => OnMasterChange(value));
        BGM.inputField.onValueChanged.AddListener((value) => OnBGMChange(value));
        SFX.inputField.onValueChanged.AddListener((value) => OnSFXChange(value));
    }

    protected override void SetValueFromData()
    {
        Master.slider.SetValueWithoutNotify(Sound.Volume.Master);
        BGM.slider.SetValueWithoutNotify(Sound.Volume.BGM);
        SFX.slider.SetValueWithoutNotify(Sound.Volume.SFX);

        int masterVolume = (int)(Sound.Volume.Master * 100);
        int bgmVolume = (int)(Sound.Volume.BGM * 100);
        int sfxVolume = (int)(Sound.Volume.SFX * 100);

        Master.inputField.SetTextWithoutNotify(masterVolume.ToString());
        BGM.inputField.SetTextWithoutNotify(bgmVolume.ToString());
        SFX.inputField.SetTextWithoutNotify(sfxVolume.ToString());
    }

    public override void SaveOption()
    {
        Setting.Sound = new()
        {
            Master = Sound.Volume.Master,
            BGM = Sound.Volume.BGM,
            SFX = Sound.Volume.SFX,
        };
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        Sound = App.Manager.Sound;
    } 

    #region Slider
    private void OnMasterChange(float _value)
    {
        Sound.Volume.Master = _value;
        int value = (int)(_value * 100);
        Master.inputField.SetTextWithoutNotify(value.ToString());
    }
    private void OnBGMChange(float _value)
    {
        Sound.Volume.BGM = _value;
        int value = (int)(_value * 100);
        BGM.inputField.SetTextWithoutNotify(value.ToString());
    }

    private void OnSFXChange(float _value)
    {
        Sound.Volume.SFX = _value;
        int value = (int)(_value * 100);
        SFX.inputField.SetTextWithoutNotify(value.ToString());
    }
    #endregion

    #region InputField
    private void OnMasterChange(string _value)
    {
        float value = float.Parse(_value);
        Sound.Volume.Master = value;
        Master.slider.SetValueWithoutNotify(value);
    }

    private void OnBGMChange(string _value)
    {
        float value = float.Parse(_value);
        Sound.Volume.BGM = value;
        BGM.slider.SetValueWithoutNotify(value);
    }

    private void OnSFXChange(string _value)
    {
        float value = float.Parse(_value);
        Sound.Volume.SFX = value;
        SFX.slider.SetValueWithoutNotify(value);
    }
    #endregion
}
