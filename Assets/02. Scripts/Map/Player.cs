using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [SerializeField] FloatingEffect floating;
    public static Action<Tile> PlayerSightUpdate;

    int maxHealth = 1;
    int currentHealth;

    public int HealthPoint
    {
        get { return currentHealth; }
    }

    List<Coords> movePath;

    public List<Coords> MovePath
    {
        get { return movePath; }
    }


    TileController currentTileContorller;

    public TileController TileController
    {
        get { return currentTileContorller; }
    }

    void Start()
    {
        movePath = new List<Coords>();
        currentHealth = maxHealth;
        StartCoroutine(DelaySightGetInfo());
    }

    public IEnumerator MoveToTarget(TileController targetTileController, float time = 0.4f)
    {
        //isPlayerMoving = true;

        //DeselectAllBorderTiles();

        Tile targetTile;
        Vector3 targetPos;
        Vector3 lastTargetPos = targetTileController.transform.position;

        foreach (var item in movePath)
        {
            targetTile = MapController.instance.GetTileFromCoords(item);
            if (targetTile == null)
                break;

            targetPos = ((GameObject)targetTile.GameEntity).transform.position;
            targetPos.y += 0.5f;

            transform.DOMove(targetPos, time);
            currentHealth--;
            yield return new WaitForSeconds(time);
        }

        lastTargetPos.y += 0.5f;
        yield return transform.DOMove(lastTargetPos, time);
        yield return new WaitForSeconds(time);

        movePath.Clear();
        currentHealth = 0;

        UpdateCurrentTile(targetTileController);

        // MapManager로 이동
        //resourceManager.GetResource(playerLocationTileController);
        //arrow.OffEffect();
    }

    /// <summary>
    /// 플레이어가 서 있는 타일의 위치를 갱신할 때마다 그 타일의 정보를 넘겨주는 이벤트 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator DelaySightGetInfo()
    {
        // AdditiveScene 딜레이 
        yield return new WaitUntil(() => PlayerSightUpdate != null);
        PlayerSightUpdate?.Invoke(currentTileContorller.Model);
    }

    public void UpdateCurrentTile(TileController tileController)
    {
        currentTileContorller = tileController;
        PlayerSightUpdate?.Invoke(currentTileContorller.Model);
    }

    public void UpdateMovePath(List<Coords> path)
    {
        movePath = path;
    }

    public void SetHealth(bool isMax,int num = 0)
    {
        if(isMax == true)
            currentHealth = maxHealth;
        else
        {
            currentHealth = num;
        }
    }

    public void StartFloatingAnimation()
    {
        StartCoroutine(floating.FloatingAnimation());
    }
}
