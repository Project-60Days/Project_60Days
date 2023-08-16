using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUiController : ControllerBase
{
    [SerializeField] Button button;

    public override EControllerType GetControllerType()
    {
        return EControllerType.MAP;
    }

    public void InteractableOn()
    {
        button.interactable = true;
    }
}
