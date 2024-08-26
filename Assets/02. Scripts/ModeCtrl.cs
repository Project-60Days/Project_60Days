using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class ModeCtrl : MonoBehaviour
{
    protected List<ItemBase> itemData;
    protected List<ItemCombineData> itemCombineData;

    [SerializeField] protected Transform slotParent;

    public virtual void Init()
    {
        itemData = App.Manager.Game.itemData;

        InitSlots();
    }

    public abstract void InitSlots();

    public abstract void Exit();

    protected string[] GetCombinationCodes(ItemCombineData _combineData)
    {
        string[] codes = new string[4];

        codes[0] = _combineData.Material_1;
        codes[1] = _combineData.Material_2;
        codes[2] = _combineData.Material_3;
        codes[3] = _combineData.Result;

        return codes;
    }
}
