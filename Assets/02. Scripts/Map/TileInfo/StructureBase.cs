using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class StructureBase
{
    protected string structureName;

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
    
    public abstract void Init(List<TileBase> _neighborTiles);
    public abstract void YesFunc();
    public abstract void NoFunc();
    

}