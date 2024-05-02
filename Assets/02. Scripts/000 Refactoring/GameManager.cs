using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Manager
{
    public ItemSO itemSO;
    public List<ItemBase> itemData => itemSO.items.ToList();

    protected override void Awake()
    {
        base.Awake();

        InitItemSO();
    }

    private void InitItemSO()
    {
        var itemData = App.Data.Game.itemData;

        foreach (var item in itemSO.items)
        {
            item.data = itemData[item.Code];
            item.Init();
        }
    }
}
