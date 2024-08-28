using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class ModeCtrl : MonoBehaviour
{
    protected Dictionary<string, ItemBase> itemData;

    public abstract BenchType GetModeType();

    public virtual void Init()
    {
        itemData = App.Data.Game.ITEM.ToDictionary(x => x.Code);

        ResetSlots();
    }

    public abstract void ResetSlots();

    public virtual void Enter()
    {
        gameObject.SetActive(true);
    }

    public virtual void Exit()
    {
        gameObject.SetActive(false);
    }
}
