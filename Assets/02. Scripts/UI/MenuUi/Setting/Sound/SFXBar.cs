using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXBar : SoundButton
{
    public override void SetVolume(float _newVolume)
    {
        App.instance.GetSoundManager().SetSFXVolume(_newVolume);
    }
}
