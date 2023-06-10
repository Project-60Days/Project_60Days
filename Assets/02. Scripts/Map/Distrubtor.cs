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
        //DataManager.instance.gameData.TryGetValue("DISRUBTOR_LIFETIME", out GameData time);
        lifeTime = 3f;
        curTile = tile;
        direction = cp;

        GetComponent<MeshRenderer>().material.DOColor(Color.black, 1f);
    }

    public void Move()
    {
        if (lifeTime > 0)
        {
            curTile.Neighbours.TryGetValue(direction, out Tile nextTile);
            transform.DOMove(((GameObject)nextTile.GameEntity).transform.position + Vector3.up, 1f);
            curTile = nextTile;
            lifeTime -= 1;
        }
        else
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }
}
