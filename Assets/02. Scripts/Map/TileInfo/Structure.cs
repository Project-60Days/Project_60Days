using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Structure : MonoBehaviour
{
    public string structureName;
    public Resource resource;
    public bool isPlayerInTile;
    public Action YesFunc;
    public Action NoFunc;
    bool isVisit;

    public bool Visit
    {
        get => isVisit;
        set
        {
            if(isPlayerInTile == false)
                return;
            else
                isVisit = value;
        }
    }

    public abstract void Init();
}