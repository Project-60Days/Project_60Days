using System;
using UnityEngine;
using Hexamap;

public abstract class MapBase : MonoBehaviour, IListener
{
    protected HexamapController hexaMap;

    protected TileBase tile;

    public virtual Type GetUnitType() => GetType();

    private void Awake()
    {
        App.Manager.Event.AddListener(EventCode.TileUpdate, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.TileUpdate:
                tile = _param as TileBase;
                break;
        }
    }

    public virtual void Init()
    {
        hexaMap = App.Manager.Asset.Hexamap;
    }

    public abstract void ReInit();
}