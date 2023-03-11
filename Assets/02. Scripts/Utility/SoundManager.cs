using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] Sound[] sfx = null;
    [SerializeField] Sound[] bgm = null;

    [SerializeField] AudioSource bgmPlayer = null;
    [SerializeField] AudioSource sfxPlayer = null;

    Dictionary<string, AudioClip> dic_BGM;
    Dictionary<string, AudioClip> dic_SFX;

    [SerializeField] float BGM_Volume = 0.5f;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        dic_BGM = new Dictionary<string, AudioClip>();
        dic_SFX = new Dictionary<string, AudioClip>();
    }

    void Start()
    {
        foreach (Sound sound in bgm)
        {
            dic_BGM.Add(sound.name, sound.clip);
        }

        foreach (Sound sound in sfx)
        {
            dic_SFX.Add(sound.name, sound.clip);
        }
    }

    public void PlaySFX(string sfxName)
    {
        sfxPlayer.clip = dic_SFX[sfxName];

        sfxPlayer.Play();
    }

    public void PlayBGM(string bgmName)
    {
        bgmPlayer.clip = dic_BGM[bgmName];

        bgmPlayer.Play();
    }

    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

}