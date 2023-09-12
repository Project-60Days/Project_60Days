using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftMode_Craft : CraftModeBase
{
    public CraftMode_Craft()
    {
        eCraftModeType = ECraftModeType.Craft;
    }

    public override void ActiveMode()
    {
        UIManager.instance.GetCraftingUiController().enabled = true;
        UIManager.instance.GetCraftingRawImageController().DestroyObject();
        gameObject.SetActive(true);
    }

    public override void InActiveMode()
    {
        UIManager.instance.GetCraftingUiController().enabled = false;
        UIManager.instance.GetCraftingUiController().ExitUi();
        gameObject.SetActive(false);
    }
}
