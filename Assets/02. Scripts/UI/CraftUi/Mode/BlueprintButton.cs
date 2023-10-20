using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlueprintButton : ModeButtonBase
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.GetCraftModeController().SetBlueprintActive();
    }
}
