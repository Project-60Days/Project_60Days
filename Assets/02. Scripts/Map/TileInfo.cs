using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    public bool isDistrubtorOn { get; set; }
    ETileType tileType;
    EWeatherType weatherType;
    TileInfo[] aroundTiles;
    string buildingID;
    int resourceID;
    string specialID;
    bool isCanMove;

    public void DistrubtorOnOff(bool onoff)
    {
        isDistrubtorOn = onoff;
    }
}
