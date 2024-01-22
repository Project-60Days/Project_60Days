using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapData
{
    [Tooltip("자원 등장 확률")] public int resourcePercent=60;
    
    [Tooltip("시야")] public int fogSightRange=4;

    [Tooltip("이동 거리")] public int playerMovementPoint=2;

    [Tooltip("좀비 감지 범위")] public int zombieDetectionRange=3;

    [Tooltip("등장 좀비 수")] public int zombieCount=80;
}