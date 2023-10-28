using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal : Structure
{
    public override void Init()
    {
        structureName = "신호기";
    }

    public override string GetstructureName()
    {
        return structureName;
    }
}
