using System.Collections;
using System.Collections.Generic;
using Hexamap;
using UnityEngine;

public class TileJungle : TileBase
{
    public override TileType GetTileType() => TileType.Jungle;

    public override void Buff(Player player) { }

    public override void DeBuff(Player _player)
    {
        if (RandomPercent.GetRandom(30))
        {
            // 랜덤 이동
            App.Manager.UI.GetPanel<PagePanel>().SetResultPage("LOOSE_WAY", false);

            _player.JungleDebuffOn();
        }

        if (RandomPercent.GetRandom(10))
        {
            App.Manager.UI.GetPanel<PagePanel>().SetResultPage("SWAMP", false);
            
            _player.SetHealth(false);
        }
    }
}