using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal : Structure
{
    public Signal()
    {
        structureName = "신호기";
    }

    public override void Init()
    {
        isPlayerInTile = false;
        Visit = false;
    }
    
}
