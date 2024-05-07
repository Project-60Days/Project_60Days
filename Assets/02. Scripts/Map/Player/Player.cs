using System;
using UnityEngine;
using Hexamap;
using DG.Tweening;
using System.Collections;

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

    private bool isJungleDebuff;

    int clockBuffDuration = 0;

    [SerializeField] Renderer rend;
    [SerializeField] Material cloakingMaterial;
    [SerializeField] Material normalMaterial;

    void Start()
    {
        StartCoroutine(DelaySightGetInfo());
    }

    IEnumerator DelaySightGetInfo()
    {
        // AdditiveScene 딜레이 
        yield return new WaitUntil(() => PlayerSightUpdate != null);
        PlayerSightUpdate?.Invoke();
    }

    public void InputDefaultData(int _moveRange)
    {
        maxMoveRange = _moveRange;
        moveRange = maxMoveRange;
    }

    public void ActionDecision(TileController targetTileController)
    {
        if (isJungleDebuff)
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
        transform.DOMove(targetTileController.transform.position, 0f);
        transform.DOMoveY(transform.position.y + 0.5f, 0f);

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

        App.Manager.Map.UpdateCurrentTile(((GameObject)tile.GameEntity).GetComponent<TileController>());

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

    public void ChangeMoveRange(int num)
    {
        moveRange += num;
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

    public void JungleDebuffOn()
    {
        isJungleDebuff = true;
    }
}