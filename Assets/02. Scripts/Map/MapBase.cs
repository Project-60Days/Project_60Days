using System;
using UnityEngine;
using Hexamap;

public abstract class MapBase : MonoBehaviour
{
    protected MapData data;
    protected TileController tile;

    public virtual Type GetUnitType() => GetType();

    protected virtual void Awake()
    {
        data = App.Manager.Test.mapData;
    }

    public void SetTile(TileController _tile)
    {
        tile = _tile;
    }

    public abstract void Init();

    public abstract void ReInit();
}