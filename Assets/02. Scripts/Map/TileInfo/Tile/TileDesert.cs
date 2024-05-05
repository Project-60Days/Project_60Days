using System.Collections;
using System.Collections.Generic;
using Hexamap;
using UnityEngine;

public class TileDesert : TileBase
{
    public override void Buff(Player player) { }

    public override void DeBuff(Player _player)
    {
        App.Manager.Game.ChangeDurbility(-1);

        if (RandomPercent.GetRandom(10))
        {
            Debug.Log("모래폭풍 디버프");
            App.Manager.UI.GetPanel<PagePanel>().SetResultPage("DESERT_STORM",false);
            _player.SetHealth(false);
        }
    }
}
