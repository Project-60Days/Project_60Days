using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class StructureBase
{
    protected StructData data;

    protected abstract string GetCode();

    public string name { get; protected set; }

    public Resource resource { get; protected set; }

    public bool isUse { get; protected set; }

    public bool isAccessible { get; protected set; }

    public List<TileBase> neighborTiles { get; protected set; }

    public List<TileBase> colleagues { get; protected set; }

    public int visitDay { get; protected set; }

    public ItemData specialItem;
    
    public GameObject structureModel;
    
    public virtual void Init(List<TileBase> _neighborTiles, GameObject _structureModel, ItemSO _itemSO)
    {
        data = App.Data.Game.structData[GetCode()];

        name = data.Korean;

        resource = new Resource(data.Item, data.Count);

        isUse = false;
        isAccessible = false;

        neighborTiles = _neighborTiles;
        structureModel = _structureModel;
    }

    public abstract void YesFunc();
    public abstract void NoFunc();

    public void SetColleagues(List<TileBase> _colleagues)
    {
        colleagues = _colleagues;
    }
    
    public void AllowAccess()
    {
        isUse = true;
        isAccessible = true;
    }
}