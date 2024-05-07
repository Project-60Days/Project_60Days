using System.Collections;
using System.Collections.Generic;
using Hexamap;
using UnityEngine;

public class TileJungle : TileBase
{
    public override TileType GetTileType() => TileType.Jungle;

    protected override void Awake()
    {
        base.Awake();

        resourceCount = 3;
    }

    public override void Buff() { }

    public override void DeBuff()
    {
        if (RandomPercent.GetRandom(30))
        {
            // 랜덤 이동
            App.Manager.UI.GetPanel<PagePanel>().SetResultPage("LOOSE_WAY", false);

            App.Manager.Map.SetRandomTile();
        }

        if (RandomPercent.GetRandom(10))
        {
            App.Manager.UI.GetPanel<PagePanel>().SetResultPage("SWAMP", false);

            App.Manager.Map.SetMoveRange(0);
        }
    }
}