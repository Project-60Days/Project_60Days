using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CraftModeBase : MonoBehaviour
{
    public ECraftModeType eCraftModeType;

    public abstract void ActiveMode();
    public abstract void InActiveMode();
}
