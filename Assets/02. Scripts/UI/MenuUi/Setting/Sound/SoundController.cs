using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    SoundButton[] soundButtons;
    SoundButton sfx;
    SoundButton bgm;
    SoundButton all;

    void Awake()
    {
        soundButtons = GetComponentsInChildren<SoundButton>();
        foreach (var button in soundButtons) 
        {
            if (button.type == SoundType.BGM)
                bgm = button;
            else if (button.type == SoundType.SFX)
                sfx = button;
            else
                all = button;
        }
    }

    public void SetVolume(SoundButton _btn)
    {
        if (_btn.type == SoundType.BGM)
            SetBGMVolume();
        else if (_btn.type == SoundType.SFX)
            SetSFXVolume();
        else
        {
            SetBGMVolume();
            SetSFXVolume();
        }
    }

    void SetBGMVolume()
    {
        var newVolume = bgm.currentWidth * all.currentWidth;
        App.Manager.Sound.SetBGMVolume(newVolume);
    }

    void SetSFXVolume()
    {
        var newVolume = sfx.currentWidth * all.currentWidth;
        App.Manager.Sound.SetSFXVolume(newVolume);
    }

}
