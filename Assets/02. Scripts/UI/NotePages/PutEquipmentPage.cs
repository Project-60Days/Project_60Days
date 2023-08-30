using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutEquipmentPage : NotePage
{
    public override ENotePageType GetENotePageType()
    {
        return ENotePageType.PutEquipment;
    }

    public override int GetPriority()
    {
        return 5;
    }

    public override void playPageAction()
    {
        //    GameManager.instance.SetPrioryty(false);
        //    inventory.SetActive(false);
    }
}
