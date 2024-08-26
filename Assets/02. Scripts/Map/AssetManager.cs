using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;
using FischlWorks_FogWar;

public class AssetManager : Manager
{
    public HexamapController Hexamap;
    public csFogWar Fog;

    private void Start()
    {
        Fog.Add(App.Manager.Map.GetUnit<PlayerUnit>().PlayerTransform, App.Data.Test.Buff.fogSightRange, true);
    }
}
