using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Hexamap;

public class Explorer : DroneBase
{
    public Tile targetTile;
    private bool goToMap;
    private bool isIdle;
    List<Coords> movePath;

    WaitForSeconds delay05 = new WaitForSeconds(0.5f);
    WaitForSeconds delay1 = new WaitForSeconds(1.5f);

    public void Set(Tile tile)
    {
        lifeTime = 1f;
        currTile = tile;
    }

    public void Targeting(Tile tile)
    {
        targetTile = tile;
        GetComponentInChildren<MeshRenderer>().material.DOFade(100, 1f);
    }

    public IEnumerator Move(int walkCount = 2)
    {
        Tile nextTile;
        Vector3 targetPos;

        if (currTile != targetTile)
            movePath = AStar.FindPath(currTile.Coords, targetTile.Coords);

        if (lifeTime > 0)
        {
            if (movePath.Count < walkCount)
            {
                nextTile = App.Manager.Map.mapCtrl.GetTileFromCoords(targetTile.Coords);
                targetPos = ((GameObject)nextTile.GameEntity).transform.position;
                targetPos.y += 0.5f;
                
                gameObject.transform.DOMove(targetPos, 0.5f);
                yield return delay05;
                currTile = nextTile;
            }
            else if (currTile != targetTile)
            {
                for (int i = 0; i < walkCount; i++)
                {
                    nextTile = App.Manager.Map.mapCtrl.GetTileFromCoords(movePath[i]);
                    targetPos = ((GameObject)nextTile.GameEntity).transform.position;
                    targetPos.y += 0.5f;
                    
                    gameObject.transform.DOMove(targetPos, 0.5f);
                    yield return delay05;
                    currTile = nextTile;
                }
            }

            if (currTile == targetTile)
            {
                // 자원
                App.Manager.Map.mapCtrl.fog.AddFogRevealer(new FischlWorks_FogWar.csFogWar.FogRevealer(gameObject.transform, 2, false));
                lifeTime -= 1;
            }

        }
        else
        {
            // ����
            Debug.Log("탐색기 대기중");
            
            isIdle = true;
            StartCoroutine(ExplorerEffect());
        }
        movePath.Clear();
    }

    public IEnumerator ExplorerEffect()
    {
        yield return new WaitUntil(()=> goToMap == true);

        App.Manager.Map.mapCtrl.GetSightTiles(currTile);
        App.Manager.Map.mapCtrl.droneCtrl.RemoveExplorer(this);
        App.Manager.Map.mapCtrl.fog._FogRevealers[App.Manager.Map.mapCtrl.fog._FogRevealers.Count - 1].sightRange = 0;

        goToMap = false;
        isIdle = false;
        yield return delay1;
        App.Manager.Map.mapCtrl.fog.RemoveFogRevealer(1);
        Destroy(gameObject);
    }
    
    public void Invocation()
    {
        goToMap = true;
    }

    public bool GetIsIdle()
    {
        return isIdle;
    }
}
