using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Hexamap;

public class Distrubtor : MonoBehaviour
{
    float lifeTime;
    public Tile curTile;
    CompassPoint direction;

    public void Set(Tile tile, CompassPoint cp)
    {
        //App.instance.GetDataManager().gameData.TryGetValue("DISRUBTOR_LIFETIME", out GameData time);
        lifeTime = 3f;
        curTile = tile;
        direction = cp;

        GetComponentInChildren<MeshRenderer>().material.DOFade(100, 1f);
    }

    public void Move()
    {
        curTile.Neighbours.TryGetValue(direction, out Tile nextTile);



        if (lifeTime > 0)
        {
            if (nextTile.Landform.GetType().Name == "LandformWorldLimit")
            {
                lifeTime -= 1;
                return;
            }
            else
            {
                transform.DOMove(((GameObject)nextTile.GameEntity).transform.position + Vector3.up, 0.5f);
                curTile = nextTile;
                lifeTime -= 1;
            }
        }
        else
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }
}
