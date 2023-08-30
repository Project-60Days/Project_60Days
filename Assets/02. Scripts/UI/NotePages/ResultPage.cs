using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultPage : NotePage
{
    bool isNeedToday;

    public override ENotePageType GetENotePageType()
    {
        return ENotePageType.Result;
    }

    public override void PlayPageAction()
    {

    }
    public override void SetPageEnabled(bool isNeedToday)
    {
        this.isNeedToday = isNeedToday;
    }

    public override bool GetPageEnabled()
    {
        return isNeedToday;
    }

    public override void StopDialogue() { }
}
