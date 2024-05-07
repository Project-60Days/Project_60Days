using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public static Action PlayerSightUpdate;

    int maxMoveRange = 1;

    int moveRange;

    public int MoveRange
    {
        get => moveRange;
        set => moveRange = value;
    }

    public List<Coords> MovePath { get; private set; }

    private bool isJungleDebuff;

    public bool JungleDebuff => isJungleDebuff;

    int clockBuffDuration;

    [SerializeField] Renderer rend;
    [SerializeField] Material cloakingMaterial;
    [SerializeField] Material normalMaterial;

    void Start()
    {
        MovePath = new List<Coords>();
        moveRange = maxMoveRange;
        clockBuffDuration = 0;
        StartCoroutine(DelaySightGetInfo());
    }

    public void InputDefaultData(int _moveRange, int _durability)
    {
        maxMoveRange = _moveRange;
        moveRange = maxMoveRange;
    }

    public void ActionDecision(TileController targetTileController)
    {
        if (JungleDebuff)
        {
            // 이동
            Debug.Log("랜덤 이동");
            MoveToRandom();
            
            isJungleDebuff = false;
        }
        else
        {
            // 공격
            MoveToTarget(targetTileController);
        }
    }
    
    public void MoveToTarget(TileController targetTileController)
    {
        Tile targetTile;
        Vector3 targetPos;
        Vector3 lastTargetPos = targetTileController.transform.position;

        var zombies = targetTileController.GetComponent<TileBase>().currZombies;

        // 이동한 타일에 좀비가 있다면 공격
        if (zombies != null)
        {
            Debug.Log("플레이어 -> 좀비 공격.");
            AttackZombies(zombies);
        }
        else
        {
            // 이동한 타일에 좀비가 없다면 이동
            for (int i = 0; i < MovePath.Count; i++)
            {
                Coords coords = MovePath[i];
                targetTile = App.Manager.Map.GetTileFromCoords(coords);

                if (targetTile == null)
                    break;

                targetPos = ((GameObject)targetTile.GameEntity).transform.position;
                targetPos.y += 0.5f;

                transform.DOMove(targetPos, 0f);
                moveRange--;
            }

            lastTargetPos.y += 0.5f;
            transform.DOMove(lastTargetPos, 0f);
        }

        MovePath.Clear();
        moveRange = 0;

        App.Manager.Map.UpdateCurrentTile(targetTileController);
    }

    public void MoveToRandom(int num = 1)
    {
        var candidate = App.Manager.Map.GetTilesInRange(num, App.Manager.Map.tileCtrl.Model);

        Vector3 targetPos = App.Manager.Map.tileCtrl.transform.position;
        Tile tile = candidate[0];
        
        for (int i = 0; i < candidate.Count; i++)
        {
            if (App.Manager.Map.CheckTileType(candidate[i], "LandformPlain")) 
            {
                if (((GameObject)candidate[i].GameEntity).GetComponent<TileBase>().structure == null &&
                    ((GameObject)candidate[i].GameEntity).GetComponent<TileBase>().currZombies == null)
                {
                    targetPos = ((GameObject)candidate[i].GameEntity).transform.position;
                    tile = candidate[i];
                    break;
                }
            }
        }
        
        targetPos.y += 0.6f;

        gameObject.transform.DOMove(targetPos, 0f);

        MovePath.Clear();
        moveRange = 0;

        App.Manager.Map.UpdateCurrentTile(((GameObject)tile.GameEntity).GetComponent<TileController>());

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

    public void UpdateMovePath(List<Coords> path)
    {
        MovePath = path;
    }

    public void SetHealth(bool isMax, int num = 0)
    {
        if (isMax)
            moveRange = maxMoveRange;
        else
        {
            moveRange = num;
        }
    }

    public void AttackZombies(ZombieBase zombies)
    {
        // 공격 애니메이션
        zombies.TakeDamage();
    }

    public void ChangeMoveRange(TileType _type)
    {
        switch (_type)
        {
            case TileType.City:
                moveRange += 1;
                break;
            case TileType.Desert:
                moveRange = 0;
                break;
            default:
                break;
        }
    }

    public void ClockUntil(int _duration)
    {
        clockBuffDuration = _duration;
        Debug.Log(_duration);
        rend.material = cloakingMaterial;
        Debug.Log(rend.material);
    }

    public bool GetIsClocking()
    {
        if (clockBuffDuration > 0)
        {
            return true;
        }
        else
        {
            clockBuffDuration = 0;
            rend.material = normalMaterial;
            return false;
        }
    }

    public void ChangeClockBuffDuration()
    {
        if (clockBuffDuration > 0) 
            clockBuffDuration--;
    }


    public void AddSightRange(int _amount)
    {
        App.Manager.Map.fog.AddRange(_amount);
    }
    
    public void TileEffectCheck()
    {
        var tileBase = App.Manager.Map.tileCtrl.GetComponent<TileBase>();

        tileBase.Buff(this);
        tileBase.DeBuff(this);
    }

    public void JungleDebuffOn()
    {
        isJungleDebuff = true;
    }
}