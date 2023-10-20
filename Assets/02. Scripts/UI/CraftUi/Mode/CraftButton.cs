using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftButton : ModeButtonBase
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.GetCraftModeController().SetCraftActive();
    }
}
