using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllBar : SoundButton
{
    public override void SetVolume(float _newVolume)
    {
        App.instance.GetSoundManager().SetBGMVolume(_newVolume);
        App.instance.GetSoundManager().SetSFXVolume(_newVolume);
    }
}
