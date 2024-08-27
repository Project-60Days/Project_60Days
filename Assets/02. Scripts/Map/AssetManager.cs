using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;
using FischlWorks_FogWar;

public class AssetManager : Manager, IListener
{
    public HexamapController Hexamap;
    public csFogWar Fog;

    protected override void Awake()
    {
        base.Awake();

        App.Manager.Event.AddListener(EventCode.PlayerCreate, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.PlayerCreate:
                Fog.Add(_param as Transform, App.Data.Test.Buff.fogSightRange, true);
                break;
        }
    }
}
