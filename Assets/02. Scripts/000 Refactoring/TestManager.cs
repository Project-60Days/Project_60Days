using System;
using UnityEngine;

[Serializable]
public class MapData
{
    public int resourcePercent = 70;

    public int enemyDetectRange = 3;

    public int zombieCount = 80;

    public int durability = 200;
}

[Serializable]
public class BuffData
{
    public int fogSightRange = 4;

    public int moveRange = 2;

    public int resourceCount = 2;

    public bool canDetect = true;
}

public class TestManager : Manager
{
    [Header("Map")]
    public MapData Map;
    public BuffData Buff;

    [Header("Tutorial")]
    public bool startTutorial = false;

    private BuffData defaultBuff;

    private void Start()
    {
        defaultBuff = Buff;
    }

    public void NextDay()
    {
        Buff = defaultBuff;
    }

    public void AddMoveRange(int num)
    {
        Buff.moveRange += num;
    }

    public void SetMoveRange(int num)
    {
        Buff.moveRange = num;
    }

    public void SetCloaking(int num)
    {
        Buff.canDetect = false;
        App.Manager.Map.GetUnit<PlayerUnit>().SetCloaking(num);
    }

    public void UnsetCloaking()
    {
        Buff.canDetect = true;
    }

    public void SetResourceCount(int num)
    {
        Buff.resourceCount += num;
    }
}
