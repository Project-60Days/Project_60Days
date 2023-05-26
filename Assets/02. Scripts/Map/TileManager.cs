using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct TileInfo
{
    ETileType tileType;
    EWeatherType weatherType;
    TileInfo[] aroundTiles;
    string buildingID;
    int resourceID;
    string specialID;
    bool isCanMove;
    //ZombieSwarm tileZombieInfo;
    //Human tileHumanInfo;
}

public class TileManager : Singleton<TileManager>
{
    [SerializeField] GameObject player;


    public void MovePlayer(Vector2 pos)
    {
        player.transform.position = new Vector3(pos.x,pos.y,0);
    }
}
