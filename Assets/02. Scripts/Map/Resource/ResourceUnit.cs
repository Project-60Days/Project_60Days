using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Resource
{
    public ItemBase Item { get; private set; }

    private int count;

    public int Count
    {
        get => count;
        set
        {
            if (value < 0)
                count = 0;
            else
                count = value;
        }
    }

    public Resource(ItemBase _item, int _count)
    {
        Item = _item;
        Count = _count;
    }

    public Resource(string _code, int _count)
    {
        Item = App.Manager.Game.itemSO.items.ToList().Find(x => x.data.Code == _code);
        Count = _count;
    }
}

public class ResourceUnit : MapBase
{
    private List<Resource> resources;

    public override void Init() 
    {
        var tiles = App.Manager.Map.AllTile;
        tiles.Remove(tile.Model);
        var selectList = Shuffle(tiles, App.Manager.Test.Map.resourcePercent);

        foreach (var tile in selectList)
        {
            tile.Ctrl.Base.SetResource();
        }
    }

    public override void ReInit()
    {
        resources = tile.Base.GetResources();
        var Inventory = App.Manager.UI.GetPanel<InventoryPanel>();

        foreach (var resource in resources)
        {
            Inventory.AddItem(resource.Item);
        }

        App.Manager.Sound.PlaySFX(resources[0].Item.sfxName);
    }

    public List<Resource> GetLastResources()
        => resources != null || resources.Count > 0 ? resources : null;

    private List<T> Shuffle<T>(List<T> _list, int _range)
    {
        System.Random rand = new();

        int n = _list.Count;

        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);

            T value = _list[k];
            _list[k] = _list[n];
            _list[n] = value;
        }

        return _list.GetRange(0, _range * _list.Count);
    }
}