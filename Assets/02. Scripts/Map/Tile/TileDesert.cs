using System.Collections;
using System.Collections.Generic;
using Hexamap;
using UnityEngine;

public class TileDesert : TileBase
{
    public override TileType GetTileType() => TileType.Desert;

    public override void Buff() { }

    public override void DeBuff()
    {
        App.Manager.Game.ChangeDurbility(-1);

        if (RandomPercent.GetRandom(10))
        {
            Debug.Log("모래폭풍 디버프");
            App.Manager.UI.GetPanel<PagePanel>().SetResultPage("DESERT_STORM",false);
            App.Manager.Map.SetMoveRange(0);
        }
    }
}
