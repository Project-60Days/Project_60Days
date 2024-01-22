using System.Collections;
using System.Collections.Generic;
using Hexamap;
using UnityEngine;

public class NoneTile : TileBase, ITileLandformEffect
{
    public override void TileEffectInit(Player player, ZombieBase zombie)
    {
        // 더 많은 구조물 -> 처음 생성 때 구조물 개수 많게, 1~2티어 아이템 얻을 확률 증가
        
        // 성장된 글리처 무리 : 처음 도시타일에서 생성되는 글리처 무리의 개수 +5
        
    }

    public void Buff(Player player, ZombieBase zombie)
    {
        // 포장된 도로 : 이동 거리 1추가
        
        throw new System.NotImplementedException();
    }

    public void DeBuff(Player player, ZombieBase zombie)
    {
        // 확률로 구현
        // 무너진 건물사고 : 선체 내구도 -3 감소
        // 도둑맞은 자원 : 가진 자원 중 1개 랜덤으로 빼앗김
        
        throw new System.NotImplementedException();
    }
}
