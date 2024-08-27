using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNone : TileBase
{
    public override TileType GetTileType()
        => TileType.None;

    protected override void Buff() { }

    protected override void DeBuff() { }
}
