using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutEquipmentPage : NotePage
{
    bool isNeedToday;

    public override ENotePageType GetENotePageType()
    {
        return ENotePageType.PutEquipment;
    }

    public override void PlayPageAction()
    {
        //    GameManager.instance.SetPrioryty(false);
        //    inventory.SetActive(false);
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
