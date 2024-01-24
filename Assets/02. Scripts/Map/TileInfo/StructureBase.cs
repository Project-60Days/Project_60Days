using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class StructureBase
{
    protected string structureName;

    public string StructureName => structureName;

    protected Resource resource;
    public Resource Resource => resource;
    
    protected bool isUse;
    public bool IsUse => isUse;
    
    protected bool isAccessible;
    public bool IsAccessible => isAccessible;
    
    protected List<TileBase> neighborTiles;

    public List<TileBase> NeighborTiles => neighborTiles;

    protected List<TileBase> colleagues;

    public List<TileBase> Colleagues => colleagues;
    
    protected int visitDay;

    public int VisitDay => visitDay;

    public ItemData specialItem;
    
    public GameObject structureModel;
    
    public abstract void Init(List<TileBase> _neighborTiles, GameObject _structureModel, ItemSO _itemSO);
    public abstract void YesFunc();
    public abstract void NoFunc();
    
    public void SetIsUse(bool _isUse)
    {
        isUse = _isUse;
        visitDay = UIManager.instance.GetNoteController().dayCount;
    }
    
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