using System.Collections;
using System.Collections.Generic;
using Hexamap;
using UnityEngine;

public class JungleTile : TileBase, ITileLandformEffect
{
    public void Buff(Player player)
    {
        throw new System.NotImplementedException();
    }

    public void DeBuff(Player _player)
    {
        if (RandomPercent.GetRandom(30))
        {
            // 랜덤 이동
        }

        if (RandomPercent.GetRandom(5))
        {
            _player.SetHealth(false);
        }
    }
}