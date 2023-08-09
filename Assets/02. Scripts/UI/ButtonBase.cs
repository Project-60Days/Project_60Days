using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBase : MonoBehaviour
{
    [SerializeField] string SFXName_btnClick = "SFX_ButtonClick_01";

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(PlayClickSFX);
    }

    public void PlayClickSFX()
    {
        App.instance.GetSoundManager().PlaySFX(SFXName_btnClick);
    }
}
