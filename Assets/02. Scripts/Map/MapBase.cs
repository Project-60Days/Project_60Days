using System;
using UnityEngine;
using Hexamap;

public abstract class MapBase : MonoBehaviour
{
    protected HexamapController hexaMap;

    protected TileController tile;

    public virtual Type GetUnitType() => GetType();

    public void SetTile(TileController _tile)
    {
        tile = _tile;
    }

    public virtual void Init()
    {
        hexaMap = App.Manager.Asset.Hexamap;
    }

    public abstract void ReInit();
}