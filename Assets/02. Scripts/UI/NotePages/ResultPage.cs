using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultPage : NotePage
{
    public override ENotePageType GetENotePageType()
    {
        return ENotePageType.Result;
    }

    public override int GetPriority()
    {
        return 2;
    }

    public override void playPageAction()
    {

    }
}
