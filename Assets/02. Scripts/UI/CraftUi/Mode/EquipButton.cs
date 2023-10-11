using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipButton : ModeButtonBase
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.GetCraftModeController().SetEquipActive();
    }
}
