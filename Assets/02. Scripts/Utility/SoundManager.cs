using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : ManagementBase
{
    [SerializeField] Sound[] array_sfx = null;
    [SerializeField] Sound[] array_bgm = null;

    [SerializeField] AudioSource bgmPlayer = null;
    [SerializeField] AudioSource sfxPlayer = null;

    Dictionary<string, AudioClip> dic_BGM;
    Dictionary<string, AudioClip> dic_SFX;

    [SerializeField] float bgmVolume;
    [SerializeField] float sfxVolume;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        dic_BGM = new Dictionary<string, AudioClip>();
        dic_SFX = new Dictionary<string, AudioClip>();

        foreach (Sound sound in array_bgm)
        {
            dic_BGM.Add(sound.name, sound.clip);
        }

        foreach (Sound sound in array_sfx)
        {
            dic_SFX.Add(sound.name, sound.clip);
        }
    }

    /// <summary>
    /// sfxName ÀÌ¸§ÀÇ SFX Àç»ý
    /// </summary>
    /// <param name="sfxName"></param>
    public void PlaySFX(string sfxName)
    {
        if (!dic_SFX.ContainsKey(sfxName))
        {
            Debug.LogWarning("SoundManager - Sound not found: " + sfxName);
            return;
        }

        sfxPlayer.clip = dic_SFX[sfxName];
        sfxPlayer.volume = sfxVolume;

        sfxPlayer.Play();
    }

    /// <summary>
    /// bgmName ÀÌ¸§ÀÇ BGM Àç»ý
    /// </summary>
    /// <param name="bgmName"></param>
    public void PlayBGM(string bgmName)
    {
        if (!dic_BGM.ContainsKey(bgmName))
        {
            Debug.LogWarning("SoundManager - Sound not found: " + bgmName);
            return;
        }

        bgmPlayer.clip = dic_BGM[bgmName];
        bgmPlayer.volume = bgmVolume;

        bgmPlayer.Play();
    }

    /// <summary>
    /// BGM ¸ØÃã
    /// </summary>
    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    /// <summary>
    /// BGM º¼·ý Á¶Àý (0 ~ 1)
    /// </summary>
    /// <param name="volume"></param>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);

        bgmPlayer.volume = bgmVolume;
    }

    /// <summary>
    /// SFX º¼·ý Á¶Àý (0 ~ 1)
    /// </summary>
    /// <param name="volume"></param>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);

        sfxPlayer.volume = sfxVolume;
    }

    public override EManagerType GetManagemetType()
    {
        return EManagerType.SOUND;
    }
}