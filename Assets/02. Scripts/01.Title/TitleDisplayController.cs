using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleDisplayController : DisplayController
{
    [SerializeField] TitleMenuController menu;

    public override void SetPosition()
    {
        menu.InitSettingButtonsLocation();
    }
}
