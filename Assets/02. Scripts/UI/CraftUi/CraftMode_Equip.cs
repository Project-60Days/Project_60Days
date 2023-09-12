using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftMode_Equip : CraftModeBase
{
    [SerializeField] GameObject player;

    public CraftMode_Equip()
    {
        eCraftModeType = ECraftModeType.Equip;
    }

    public override void ActiveMode()
    {
        UIManager.instance.GetCraftingRawImageController().ChangerTarget(player);
        gameObject.SetActive(true);
    }

    public override void InActiveMode()
    {
        gameObject.SetActive(false);
    }


    void Start()
    {
        gameObject.SetActive(false);
    }
}
