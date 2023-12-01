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
    int durability = 10;
    public int Durability => durability;
    
    int bulletsNum = 10;
    public int BulletsNum => bulletsNum;
    
    int currentHealth;
    public int HealthPoint => currentHealth;
    
    List<Coords> movePath;
    public List<Coords> MovePath => movePath;

    TileController currentTileContorller;
    public TileController TileController => currentTileContorller;

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

        
        // 이동한 타일에 좀비가 있다면 공격
        if(currentTileContorller.GetComponent<TileBase>().CurZombies != null)
        {
            // 게임 오버
            
            //AttackZombies(currentTileContorller.GetComponent<TileBase>().CurZombies);
        }
        
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

    public void AttackZombies(ZombieBase zombies)
    {
        // 공격 애니메이션
        // 탄의 개수 0이면 게임 오버
        
        // 플레이어 내구도 - 무리 개체 수 감소
        if(bulletsNum - zombies.zombieData.count > 0)
        {
            zombies.TakeDamage(bulletsNum);
            bulletsNum -= zombies.zombieData.count;
        }
        else if(bulletsNum - zombies.zombieData.count < 0)
        {
            bulletsNum = 0;
            Debug.Log("탄이 없습니다. 게임 오버");
            Application.Quit();
        }
        
        // 좀비 제거
    }

    public void TakeDamage(int zombieCount)
    {
        // 피격 애니메이션
        // 내구도 감소

        // 내구도가 0이 되면 게임 오버
        if( durability - zombieCount > 0)
        {
            durability -= zombieCount;
        }
        else if(durability - zombieCount < 0)
        {
            durability = 0;
            Debug.Log("내구도 부족. 게임 오버");
            Application.Quit();
        }
    }
}
