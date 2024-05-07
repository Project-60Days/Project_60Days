using System;
using UnityEngine;
using Hexamap;

public abstract class MapBase : MonoBehaviour
{
    protected TileController tile;

    public virtual Type GetUnitType() => GetType();

    public void SetTile(TileController _tile)
    {
        tile = _tile;
    }

    public abstract void Init();

    public abstract void ReInit();
}