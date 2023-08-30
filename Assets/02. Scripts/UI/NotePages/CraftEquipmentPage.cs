using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftEquipmentPage : NotePage
{
    [SerializeField] GameObject inventoryUi;

    public override ENotePageType GetENotePageType()
    {
        return ENotePageType.CraftEquipment;
    }

    public override int GetPriority()
    {
        return 4;
    }

    public override void playPageAction()
    {

        var pos = inventoryUi.transform.position;
        pos.x = 450;
        inventoryUi.transform.position = pos;
        inventoryUi.SetActive(true);
    }
}
