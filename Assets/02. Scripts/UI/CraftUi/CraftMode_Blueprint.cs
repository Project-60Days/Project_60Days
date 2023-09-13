using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftMode_Blueprint : CraftModeBase
{
    public CraftMode_Blueprint()
    {
        eCraftModeType = ECraftModeType.Blueprint;
    }

    public override void ActiveMode()
    {
        UIManager.instance.GetCraftingRawImageController().DestroyObject();
        gameObject.SetActive(true);
    }

    public override void InActiveMode()
    {
        gameObject.SetActive(false);
    }
}
