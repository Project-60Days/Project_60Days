using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUiController : Singleton<MapUiController>
{
    [SerializeField] Button button;

    public void InteractableOn()
    {
        button.interactable = true;
    }
}
