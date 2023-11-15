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
            if (button.eSoundType == ESoundType.BGM)
                bgm = button;
            else if (button.eSoundType == ESoundType.SFX)
                sfx = button;
            else
                all = button;
        }
    }

    public void SetVolume(SoundButton _btn)
    {
        if (_btn.eSoundType == ESoundType.BGM)
            SetBGMVolume();
        else if (_btn.eSoundType == ESoundType.SFX)
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
        App.instance.GetSoundManager().SetBGMVolume(newVolume);
    }

    void SetSFXVolume()
    {
        var newVolume = sfx.currentWidth * all.currentWidth;
        App.instance.GetSoundManager().SetSFXVolume(newVolume);
    }

}
