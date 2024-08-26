using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : Manager
{
    public struct VolumeData
    {
        public float volume;
        public float scale;

        public readonly float calculated => volume * scale;

        public VolumeData(float _volume, float _scale)
        {
            volume = _volume;
            scale = _scale;
        }
    }

    public class VolumeAccessor
    {
        readonly SoundManager Sound = App.Manager.Sound;
        readonly SettingData Setting = App.Data.Setting;

        public float Master
        {
            get => Sound.BGMData.scale;
            set
            {
                Sound.BGMData.scale = value;
                Sound.SFXData.scale = value;
                Sound.SetMasterVolume();
            }
        }

        public float BGM
        {
            get => Sound.BGMData.volume;
            set
            {
                Sound.BGMData.volume = value;
                Sound.SetBGMVolume();
            }
        }

        public float SFX
        {
            get => Sound.SFXData.volume;
            set
            {
                Sound.SFXData.volume = value;
                Sound.SetSFXVolume();
            }
        }
    }

    [Header("Audio Clips")]
    [SerializeField] Sound[] BGM = null;
    [SerializeField] Sound[] SFX = null;

    [Header("Audio Sources")]
    [SerializeField] AudioSource bgmPlayer = null;
    [SerializeField] AudioSource sfxPlayer = null;
    [SerializeField] AudioSource typeWritePlayer = null;

    [Header("Audio Mixer")]
    [SerializeField] AudioMixer mixer;

    private Dictionary<string, AudioClip> dic_BGM;
    private Dictionary<string, AudioClip> dic_SFX;

    private VolumeData BGMData;
    private VolumeData SFXData;

    [SerializeField] float bgmVolume; //todo
    [SerializeField] float sfxVolume; //todo

    [HideInInspector] public VolumeAccessor Volume;

    protected override void Awake()
    {
        base.Awake();

        Volume = new();

        dic_BGM = new Dictionary<string, AudioClip>();
        dic_SFX = new Dictionary<string, AudioClip>();

        foreach (Sound sound in BGM)
        {
            dic_BGM.Add(sound.name, sound.clip);
        }

        foreach (Sound sound in SFX)
        {
            dic_SFX.Add(sound.name, sound.clip);
        }
    }

    private void Start()
    {
        var setting = App.Data.Setting.Sound;

        BGMData = new(setting.BGM, setting.Master);
        SFXData = new(setting.SFX, setting.Master);

        SetMasterVolume();
        SetBGMVolume();
        SetSFXVolume();
    }

    #region Play & Stop Sound

    #region BGM
    public void PlayBGM(string _name)
    {
        if (!dic_BGM.TryGetValue(_name, out var clip))
        {
            Debug.LogError("ERROR: Failed to play BGM. Unable to find " + _name);
            return;
        }

        bgmPlayer.clip = clip;

        bgmPlayer.Play();
        bgmPlayer.DOFade(1f, 0.5f).SetEase(Ease.Linear);
    }

    public void ResumeBGM()
    {
        if (bgmPlayer.isPlaying) return;

        bgmPlayer.Play();
        bgmPlayer.DOFade(1f, 0.5f).SetEase(Ease.Linear);
    }

    public void StopBGM()
    {
        bgmPlayer.DOFade(0f, 0.5f).OnComplete(() => bgmPlayer.Stop());
    }
    #endregion

    #region SFX
    public void PlaySFX(string _name)
    {
        if (!dic_SFX.TryGetValue(_name, out var clip))
        {
            Debug.LogError("ERROR: Failed to play SFX. Unable to find " + _name);
            return;
        }

        sfxPlayer.clip = clip;

        sfxPlayer.Play();
    }

    public void StopSFX()
    {
        sfxPlayer.Stop();
    }

    public bool IsPlayingSFX()
    {
        return sfxPlayer.isPlaying;
    }
    #endregion

    #region TypeWrite
    public void PlayTypeWriteSFX(string _name)
    {
        if (!dic_SFX.TryGetValue(_name, out var clip))
        {
            Debug.LogError("ERROR: Failed to play SFX. Unable to find " + _name);
            return;
        }

        typeWritePlayer.clip = clip;

        typeWritePlayer.Play();
    }

    public bool IsPlayingTypeWriteSFX()
    {
        return typeWritePlayer.isPlaying;
    }
    #endregion

    #endregion

    #region Set Volume
    private void SetMasterVolume()
    {
        SetVolume("BGM", BGMData.calculated);
        SetVolume("SFX", SFXData.calculated);
    }
    private void SetBGMVolume() => SetVolume("BGM", BGMData.calculated);
    private void SetSFXVolume() => SetVolume("SFX", SFXData.calculated);

    private void SetVolume(string _param, float _value)
    {
        if (_value < 0.001f)
            _value = 0.00001f;

        mixer.SetFloat(_param, ValueToDecibel(_value));
    }

    private float ValueToDecibel(float value) => Mathf.Log10(value * 2) * 20;
    #endregion

    public float SetBGMVolumeTweening(float _duration)
    {
        float volume = bgmPlayer.volume;

        var bgmTween = DOTween.To(() => bgmVolume, x => bgmVolume = x, 0f, _duration)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                bgmPlayer.volume = bgmVolume;
            });

        return volume;
    }

    public bool CheckSFXExist(string sfxName)
        => dic_SFX.ContainsKey(sfxName);
}