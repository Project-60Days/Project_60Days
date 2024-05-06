using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Hexamap;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;

[SelectionBase]
public class ZombieBase : MonoBehaviour
{
    [SerializeField] GameObject[] zombieModels;
    public ValueData data;
    public Distrubtor nearthDistrubtor;
    public Tile currTile;
    public Tile lastTile;
    public Tile targetTile;
    public bool isChasingPlayer;
    private bool noneTileBuff;

    private int lastZombieCount;

    List<Coords> movePath;
    int remainStunTime;
    int moveCost = 1;
    int dectectionRange = 2;
    int debuffCoolTime = 0;

    public int count { get; private set; }

    private Vector3 initScale = new Vector3(0,0,0);

    public void Init(Tile tile)
    {
        App.Data.Game.valueData.TryGetValue("Enemy", out ValueData enemy);
        data = enemy;

        ZombieCountChoice();
        currTile = tile;
        lastTile = currTile;

        CheckTileEffect(currTile);
        CurrentTileUpdate(currTile);

        initScale = transform.localScale;
        lastZombieCount = count;
    }

    TileType CheckTileType(Tile _tile)
    {
        return ((GameObject)_tile.GameEntity).GetComponent<TileBase>().GetTileType();
    }

    void CheckTileEffect(Tile _tile)
    {
        switch (CheckTileType(_tile))
        {
            case TileType.City:
                if (noneTileBuff == false)
                {
                    count += 5;
                    ZombieModelChoice(count);
                    noneTileBuff = true;
                }

                break;
            case TileType.Desert:
                if (debuffCoolTime <= 0)
                {
                    debuffCoolTime = 1;
                }
                else if (debuffCoolTime > 0)
                {
                    debuffCoolTime -= 1;
                }

                break;
            case TileType.Tundra:
                if (debuffCoolTime <= 0)
                {
                    debuffCoolTime = 1;
                }
                else if (debuffCoolTime > 0)
                {
                    debuffCoolTime -= 1;
                }

                break;
            case TileType.Jungle:
                break;
            default:
                break;
        }
    }

    void ZombieCountChoice()
    {
        var randomInt = (int)Random.Range(data.MinCount, data.MaxCount);
        count = randomInt;
        ZombieModelChoice(randomInt);
    }

    public void ZombieModelChoice(int count)
    {
        var num = (int)Mathf.Lerp(data.MinCount, data.MaxCount, 0.3f);
        num -= (int)data.MinCount;

        if (data.MinCount <= count && count <= data.MinCount + num)
        {
            for (int i = 0; i < zombieModels.Length; i++)
            {
                if (i == 0)
                    zombieModels[i].SetActive(true);
                else
                    zombieModels[i].SetActive(false);
            }
        }
        else if (data.MinCount + num <= count && count <= data.MinCount + num * 2)
        {
            for (int i = 0; i < zombieModels.Length; i++)
            {
                if (i == 1)
                    zombieModels[i].SetActive(true);
                else
                    zombieModels[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < zombieModels.Length; i++)
            {
                if (i == 2)
                    zombieModels[i].SetActive(true);
                else
                    zombieModels[i].SetActive(false);
            }
        }
    }

    public void SizeUpCheck()
    {
        if (lastZombieCount != count)
        {
            var scale = (count / 10) * 0.1f;

            if (scale > 0.7f)
            {
                scale = 0.7f;
            }

            transform.localScale = initScale + new Vector3(scale, scale, scale);
            lastZombieCount = count;
        }
    }

    public void SetValue(int cost, int _detectionRange)
    {
        moveCost = cost;
        dectectionRange = _detectionRange;
    }

    public void DetectionAndAct()
    {
        CheckTileEffect(currTile);

        isChasingPlayer = App.Manager.Map.CalculateDistanceToPlayer(currTile, dectectionRange);

        nearthDistrubtor = App.Manager.Map.droneCtrl.CalculateDistanceToDistrubtor(currTile, dectectionRange);

        ActionDecision();
    }

    public void ActionDecision()
    {
        if (debuffCoolTime > 0)
        {
            return;
        }

        if (remainStunTime > 0)
        {
            remainStunTime--;
            return;
        }

        if (nearthDistrubtor != null)
        {
            //Debug.Log(gameObject.name + "가 교란기를 쫓아갑니다!");
            MoveToAttack(nearthDistrubtor.currTile);
            return;
        }

        if (isChasingPlayer && !App.Manager.Map.playerCtrl.player.GetIsClocking())
        {
            //Debug.Log(gameObject.name + "가 플레이어를 발견했습니다!");
            MoveToAttack(App.Manager.Map.tileCtrl.Model);

            // 플레이어 바라보기
            var updatePos = App.Manager.Map.playerCtrl.player.transform.position;
            updatePos.y += 0.6f;
            transform.LookAt(updatePos);
        }
        else
        {
            var randomInt = GetRandom();
            if (randomInt == 0)
            {
                StartCoroutine(MoveToRandom());
            }
        }
    }

    public void MoveToAttack(Tile target)
    {
        movePath = AStar.FindPath(currTile.Coords, target.Coords);

        Tile pointTile;
        Vector3 pointPos;

        if (movePath.Count == 0 && target == App.Manager.Map.tileCtrl.Model)
        {
            // 플레이어가 1칸 내에 있는 경우
            AttackPlayer(App.Manager.Map.playerCtrl.player);
        }
        else
        {
            if (movePath.Count == 0)
            {
                return;
            }

            for (int i = 0; i < moveCost; i++)
            {
                pointTile = App.Manager.Map.GetTileFromCoords(movePath[i]);
                pointPos = ((GameObject)pointTile.GameEntity).transform.position;
                pointPos.y += 0.6f;

                gameObject.transform.DOMove(pointPos, 0f);

                currTile = pointTile;

                if (movePath.Count == 1)
                    break;
            }
        }

        CurrentTileUpdate(lastTile);
        CurrentTileUpdate(currTile);

        lastTile = currTile;
    }

    public IEnumerator MoveToRandom(int num = 1, float time = 0.25f)
    {
        var candidate = App.Manager.Map.GetTilesInRange(num, currTile);
        int rand = Random.Range(0, candidate.Count);

        while (((GameObject)candidate[rand].GameEntity).gameObject.layer == 8)
        {
            candidate.RemoveAt(rand);
            rand = Random.Range(0, candidate.Count);
        }

        if (candidate[rand] == App.Manager.Map.tileCtrl.Model)
            rand--;
        var targetPos = ((GameObject)candidate[rand].GameEntity).transform.position;
        targetPos.y += 0.6f;

        yield return gameObject.transform.DOMove(targetPos, time);

        currTile = candidate[rand];

        CurrentTileUpdate(lastTile);
        CurrentTileUpdate(currTile);

        lastTile = currTile;
    }

    public void CurrentTileUpdate(Tile tile)
    {
        if (tile == null)
            return;

        if (tile == currTile)
        {
            ((GameObject)(tile.GameEntity)).GetComponent<TileBase>().UpdateZombieInfo(this);
        }
        else
        {
            ((GameObject)(tile.GameEntity)).GetComponent<TileBase>().UpdateZombieInfo(null);
        }
    }

    public void SumZombies(ZombieBase zombie)
    {
        count += zombie.count;
        zombie.count = 0;

        ZombieModelChoice(count);
        SizeUpCheck();
        CurrentTileUpdate(currTile);
    }

    public int GetRandom()
    {
        float percentage = data.Move + data.Stop;
        float probability = data.Move / percentage;
        float rate = percentage - (percentage * probability);
        int tmp = (int)Random.Range(0, percentage);

        if (tmp <= rate - 1)
        {
            return 0;
        }

        return 1;
    }

    public void AttackPlayer(Player player)
    {
        // 공격 애니메이션
        App.Manager.Game.TakeDamage(count);
    }

    public void TakeDamage()
    {
        // 피격 애니메이션

        // 사망
        count = 0;
        Debug.Log(gameObject.name + " 처치 완료.");
        CurrentTileUpdate(null);
        Destroy(this);
        // 시체 오브젝트 생성
    }


    public void Stun(int time = 1)
    {
        remainStunTime = time;
    }
}