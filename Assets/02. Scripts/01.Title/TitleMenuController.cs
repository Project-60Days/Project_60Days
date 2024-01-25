using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TitleMenuController : MonoBehaviour
{
    [SerializeField] Transform text;
    [SerializeField] Transform settingDetails;
    MenuButtonBase[] buttons;

    float textInitialY;
    float settingInitialY;

    void Start()
    {
        buttons = GetComponentsInChildren<MenuButtonBase>();

        textInitialY = text.transform.localPosition.y;
        settingInitialY = settingDetails.transform.localPosition.y;
    }

    public void InitSettingButtonsLocation()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].gameObject.name == "Setting_Btn") continue;
            buttons[i].gameObject.GetComponent<Transform>().DOLocalMoveY(buttons[i].initialY, 0f);
        }

        text.localPosition = new Vector3(text.localPosition.x, textInitialY, text.localPosition.z);
        settingDetails.localPosition = new Vector3(settingDetails.localPosition.x, settingInitialY, settingDetails.localPosition.z);
    }
}
