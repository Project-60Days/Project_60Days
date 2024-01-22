using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileLandformEffect
{
    void Buff(Player player, ZombieBase zombie);
    void DeBuff(Player player, ZombieBase zombie);
}
