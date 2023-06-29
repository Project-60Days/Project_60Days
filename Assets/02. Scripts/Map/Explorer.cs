using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Hexamap;

public class Explorer : MonoBehaviour
{
    float lifeTime;
    public Tile curTile;
    public Tile targetTile;
    List<Coords> movePath;

    public void Set(Tile tile)
    {
        lifeTime = 1f;
        curTile = tile;
    }

    public void Targetting(Tile tile)
    {
        targetTile = tile;
        GetComponentInChildren<MeshRenderer>().material.DOFade(100, 1f);
    }

    public IEnumerator Move(int walkCount = 2)
    {
        if(curTile != targetTile)
            movePath = AStar.FindPath(curTile.Coords, targetTile.Coords);

        Tile nextTile;
        Vector3 targetPos;

        if (lifeTime > 0)
        {
            if (curTile == targetTile)
            {
                Debug.Log("¿€µø");
                FischlWorks_FogWar.csFogWar.instance.InitializeMapControllerObjects(gameObject, 2);
                lifeTime -= 1;
                yield break;
            }
            else if (movePath.Count < walkCount)
            {
                nextTile = MapController.instance.GetTileFromCoords(targetTile.Coords);
                targetPos = ((GameObject)nextTile.GameEntity).transform.position;
                targetPos.y += 0.5f;
                gameObject.transform.DOMove(targetPos, 0.5f);
                yield return new WaitForSeconds(0.5f);
                curTile = nextTile;
            }
            else
            {
                for (int i = 0; i < walkCount; i++)
                {
                    nextTile = MapController.instance.GetTileFromCoords(movePath[i]);
                    targetPos = ((GameObject)nextTile.GameEntity).transform.position;
                    targetPos.y += 0.5f;
                    gameObject.transform.DOMove(targetPos, 0.5f);
                    yield return new WaitForSeconds(0.5f);
                    curTile = nextTile;
                }
            }
        }
        else
        {
            movePath.Clear();
            
            FischlWorks_FogWar.csFogWar.instance.RemoveFogRevealer(1);
            Destroy(gameObject);
        }
        movePath.Clear();
    }
}
