using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Structure : MonoBehaviour
{
    protected string structureName;
    public bool isPlayerInTile;
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