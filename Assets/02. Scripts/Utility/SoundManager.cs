using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : Manager
{

    [SerializeField] Sound[] BGM = null;
    [SerializeField] Sound[] SFX = null;    
    [SerializeField] Sound[] array_sfx = null;

    [SerializeField] AudioSource bgmPlayer = null;
    [SerializeField] AudioSource sfxPlayer = null;
    [SerializeField] AudioSource typeWritePlayer = null;

    Dictionary<string, AudioClip> dic_BGM;
    Dictionary<string, AudioClip> dic_SFX;

    [SerializeField] float bgmVolume;
    [SerializeField] float sfxVolume;


    protected override void Awake()
    {
        base.Awake();

        dic_BGM = new Dictionary<string, AudioClip>();
        dic_SFX = new Dictionary<string, AudioClip>();

        foreach (Sound sound in BGM)
        {
            dic_BGM.Add(sound.name, sound.clip);
        }

        foreach (Sound sound in array_sfx)
        {
            dic_SFX.Add(sound.name, sound.clip);
        }
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
        bgmPlayer.DOFade(0.5f, 1f).SetEase(Ease.Linear);
    }

    public void ResumeBGM()
    {
        if (bgmPlayer.isPlaying) return;

        bgmPlayer.Play();
        bgmPlayer.DOFade(0.5f, 1f).SetEase(Ease.Linear);
    }

    public void StopBGM()
    {
        bgmPlayer.DOFade(0f, 1f).OnComplete(() => bgmPlayer.Stop());
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

    /// <summary>
    /// BGM 볼륨 조절 (0 ~ 1)
    /// </summary>
    /// <param name="volume"></param>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);

        bgmPlayer.volume = bgmVolume;
    }

    /// <summary>
    /// SFX 볼륨 조절 (0 ~ 1)
    /// </summary>
    /// <param name="volume"></param>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume * 0.5f);

        sfxPlayer.volume = sfxVolume;
        typeWritePlayer.volume = sfxVolume;
    }

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

    /// <summary>
    /// SFX 목록에 해당 SFX 있는지 확인
    /// </summary>
    /// <param name="sfxName"></param>
    /// <returns></returns>
    public bool CheckSFXExist(string sfxName)
    {
        if (dic_SFX.ContainsKey(sfxName)) return true;
        else return false;
    }
}