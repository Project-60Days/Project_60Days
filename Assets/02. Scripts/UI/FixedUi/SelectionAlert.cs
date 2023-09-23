using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionAlert : AlertBase
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.GetNoteController().OpenNote();
    }
}
