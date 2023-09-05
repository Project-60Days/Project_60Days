using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToBaseButton : MonoBehaviour
{
    public void ButtonClick()
    {
        App.instance.GetMapManager().SetMapCameraPriority(false);
    }
}
