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

    WaitForSeconds delay05 = new WaitForSeconds(0.5f);
    WaitForSeconds delay1 = new WaitForSeconds(1f);

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
        Tile nextTile;
        Vector3 targetPos;

        if (curTile != targetTile)
            movePath = AStar.FindPath(curTile.Coords, targetTile.Coords);

        if (lifeTime > 0)
        {

            if (movePath.Count < walkCount)
            {
                nextTile = App.instance.GetMapManager().GetTileFromCoords(targetTile.Coords);
                targetPos = ((GameObject)nextTile.GameEntity).transform.position;
                targetPos.y += 0.5f;
                gameObject.transform.DOMove(targetPos, 0.5f);
                yield return delay05;
                curTile = nextTile;
            }
            else if (curTile != targetTile)
            {
                for (int i = 0; i < walkCount; i++)
                {
                    nextTile = App.instance.GetMapManager().GetTileFromCoords(movePath[i]);
                    targetPos = ((GameObject)nextTile.GameEntity).transform.position;
                    targetPos.y += 0.5f;
                    gameObject.transform.DOMove(targetPos, 0.5f);
                    yield return delay05;
                    curTile = nextTile;
                }
            }

            if (curTile == targetTile)
            {
                Debug.Log("작동");
                FischlWorks_FogWar.CsFogWar.instance.AddFogRevealer(new FischlWorks_FogWar.CsFogWar.FogRevealer(gameObject.transform, 2, false));
                lifeTime -= 1;
            }

        }
        else
        {
            // 삭제
            FischlWorks_FogWar.CsFogWar.instance._FogRevealers[FischlWorks_FogWar.CsFogWar.instance._FogRevealers.Count - 1].sightRange = 0;
            yield return delay1;
            FischlWorks_FogWar.CsFogWar.instance.RemoveFogRevealer(1);
            Destroy(gameObject);

        }
        movePath.Clear();
    }
}
