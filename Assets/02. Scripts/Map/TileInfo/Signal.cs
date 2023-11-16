using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal : Structure
{
    public override void Init()
    {
        structureName = "신호기";
        isPlayerInTile = false;
        Visit = false;
    }

    public override void NoFunction()
    {
        App.instance.GetMapManager().ResearchCancel();
        UIManager.instance.GetPageController().SetResultPage("Signal_No");
    }

    public override void YesFunction()
    {
        App.instance.GetMapManager().ResearchStart();
        UIManager.instance.GetPageController().SetResultPage("Signal_Yes");
    }
}
