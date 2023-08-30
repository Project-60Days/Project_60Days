using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPage : NotePage
{
    public override ENotePageType GetENotePageType()
    {
        return ENotePageType.Map;
    }

    public override int GetPriority()
    {
        return 6;
    }

    public override void playPageAction()
    {
        GameManager.instance.SetPrioryty(true);
    }
}
