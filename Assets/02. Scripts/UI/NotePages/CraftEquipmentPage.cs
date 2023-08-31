using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class CraftEquipmentPage : NotePage
{
    [SerializeField] GameObject inventoryUi;

    bool isNeedToday;

    public override ENotePageType GetENotePageType()
    {
        return ENotePageType.CraftEquipment;
    }

    public override void PlayPageAction()
    {
        GameManager.instance.SetPrioryty(false);
        var pos = inventoryUi.transform.position;
        pos.x = 450;
        inventoryUi.transform.position = pos;
        inventoryUi.SetActive(true);
    }

    public override void SetPageEnabled(bool isNeedToday)
    {
        this.isNeedToday = isNeedToday;
    }

    public override bool GetPageEnabled()
    {
        return true;
    }

    public override void StopDialogue() { }
}
