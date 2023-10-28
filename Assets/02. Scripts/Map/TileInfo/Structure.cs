using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Structure : MonoBehaviour
{
    protected string structureName = "구조물 없음";
    public abstract void Init();
    public abstract string GetstructureName();
}