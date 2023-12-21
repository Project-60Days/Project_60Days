using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [SerializeField] FloatingEffect floating;
    public static Action PlayerSightUpdate;

    int maxHealth = 1;
    int durability = 100;
    public int Durability => durability;
    
    int bulletsNum = 5;
    public int BulletsNum => bulletsNum;
    
    int currentHealth;
    public int HealthPoint => currentHealth;
    
    List<Coords> movePath;
    public List<Coords> MovePath => movePath;

    TileController currentTileContorller;
    public TileController TileController => currentTileContorller;

    bool isDead;

    void Start()
    {
        movePath = new List<Coords>();
        currentHealth = maxHealth;
        StartCoroutine(DelaySightGetInfo());
    }

    public IEnumerator MoveToTarget(TileController targetTileController, float time = 0.4f)
    {
        Tile targetTile;
        Vector3 targetPos;
        Vector3 lastTargetPos = targetTileController.transform.position;

        var zombies = targetTileController.GetComponent<TileBase>().CurZombies;

        // 이동한 타일에 좀비가 있다면 공격
        if(zombies != null)
        {
            Debug.Log("플레이어 -> 좀비 공격.");
            AttackZombies(zombies);
        }
        else
        {
            // 이동한 타일에 좀비가 없다면 이동
            for (int i = 0; i < movePath.Count; i++)
            {
                Coords coords = movePath[i];
                targetTile = MapController.instance.GetTileFromCoords(coords);

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
        }

        movePath.Clear();
        currentHealth = 0;

        UpdateCurrentTile(targetTileController);
        
    }

    /// <summary>
    /// 플레이어가 서 있는 타일의 위치를 갱신할 때마다 그 타일의 정보를 넘겨주는 이벤트 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator DelaySightGetInfo()
    {
        // AdditiveScene 딜레이 
        yield return new WaitUntil(() => PlayerSightUpdate != null);
        PlayerSightUpdate?.Invoke();
    }

    public void UpdateCurrentTile(TileController tileController)
    {
        currentTileContorller = tileController;
        PlayerSightUpdate?.Invoke();
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
        // 좀비 제거
        if(bulletsNum > 0)
        {
            // 공격 애니메이션
            zombies.TakeDamage();
            bulletsNum -= 1;
            Debug.Log("남은 펄스탄 개수 : " + bulletsNum);
        }
        else if(bulletsNum <= 0)
        {
            // 탄 없을 시 행동 불가
            return;
        }
    }

    public void TakeDamage(int zombieCount)
    {
        if(isDead)
            return;
        
        // 피격 애니메이션
  
        if( durability - zombieCount > 0)
        {
            durability -= zombieCount;
        }
        else if(durability - zombieCount <= 0)
        {
            // 내구도가 0이 되면 게임 오버
            durability = 0;
            isDead=true;
            Debug.Log("내구도 부족. 게임 오버");

            // 게임 오버
            UIManager.instance.GetNextDayController().isOver = true;
        }
    }
}
