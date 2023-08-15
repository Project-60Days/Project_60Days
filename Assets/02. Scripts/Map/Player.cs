using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;
using DG.Tweening;

public class Player : MonoBehaviour
{
    int maxHealth = 1;
    int currentHealth;

    List<Coords> movePath;

    public static Action<Tile> PlayerSightUpdate;

    TileController currentTile;

    public TileController Tile
    {
        get { return currentTile; }
        set { currentTile = value; }
    }

    void Start()
    {
        movePath = new List<Coords>();
        currentHealth = maxHealth;
        StartCoroutine(DelaySightGetInfo());
    }

    /// <summary>
    /// 플레이어가 서 있는 타일의 위치를 갱신할 때마다 그 타일의 정보를 넘겨주는 이벤트 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator DelaySightGetInfo()
    {
        // AdditiveScene 딜레이 
        yield return new WaitForEndOfFrame();
        PlayerSightUpdate?.Invoke(currentTile.Model);
    }

    public IEnumerator MoveToTargetTile(Vector3 lastTargetPos, float time = 0.4f)
    {
        //isPlayerMoving = true;

        //DeselectAllBorderTiles();

        Tile targetTile;
        Vector3 targetPos;

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
        PlayerSightUpdate?.Invoke(currentTile.Model);

        // MapManager로 이동
        //resourceManager.GetResource(playerLocationTileController);
        //arrow.OffEffect();
    }
}
