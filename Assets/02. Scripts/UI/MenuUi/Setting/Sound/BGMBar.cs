using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMBar : SoundButton
{
    public override void SetVolume(float _newVolume)
    {
        App.instance.GetSoundManager().SetBGMVolume(_newVolume);
    }
}
