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
    public TileType type;
    public Tile lastTile;
    public Tile targetTile;
    public bool isChasingPlayer;
    private bool noneTileBuff;

    private int lastZombieCount;

    List<Coords> movePath;
    int remainStunTime;
    int moveRange = 1;
    int dectectionRange = 3;
    bool isDebuff = true;

    public int count { get; private set; }

    private Vector3 initScale = new Vector3(0,0,0);

    public void Init(Tile tile)
    {
        App.Data.Game.valueData.TryGetValue("Enemy", out ValueData enemy);
        data = enemy;

        InitCount();
        currTile = tile;
        lastTile = currTile;

        CheckTileEffect(currTile);
        CurrentTileUpdate(currTile);

        initScale = transform.localScale;
        lastZombieCount = count;
    }

    void InitCount()
    {
        count = (int)Random.Range(data.MinCount, data.MaxCount); ;
        SetModel();
    }

    void CheckTileEffect(Tile _tile)
    {
        switch (CheckTileType(_tile))
        {
            case TileType.City:
                if (noneTileBuff == false)
                {
                    count += 5;
                    SetModel();
                    noneTileBuff = true;
                }

                break;

            case TileType.Desert:
            case TileType.Tundra:
                isDebuff = !isDebuff;
                break;
        }
    }

    TileType CheckTileType(Tile _tile)
    {
        return ((GameObject)_tile.GameEntity).GetComponent<TileBase>().GetTileType();
    }


    public void SetModel()
    {
        int possibility = Mathf.FloorToInt((count - data.MinCount) / ((data.MaxCount - data.MinCount) / (zombieModels.Length - 1)));
        int modelIndex = Mathf.Clamp(possibility, 0, zombieModels.Length - 1);

        for (int i = 0; i < zombieModels.Length; i++)
        {
            zombieModels[i].SetActive(i == modelIndex);
        }
    }

    public void SizeUpCheck()
    {
        if (lastZombieCount != count)
        {
            var scale = (count / 10) * 0.1f;

            if (scale > 0.7f) return;

            transform.localScale = initScale + new Vector3(scale, scale, scale);
            lastZombieCount = count;
        }
    }

    public void DetectionAndAct()
    {
        CheckTileEffect(currTile);

        isChasingPlayer = App.Manager.Map.CalculateDistanceToPlayer(currTile, dectectionRange);

        nearthDistrubtor = App.Manager.Map.GetUnit<DroneUnit>().CalculateDistanceToDistrubtor(currTile, dectectionRange);

        ActionDecision();
    }

    public void ActionDecision()
    {
        if (isDebuff) return;

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

        if (isChasingPlayer && !App.Manager.Map.Buff.canDetect)
        {
            //Debug.Log(gameObject.name + "가 플레이어를 발견했습니다!");
            MoveToAttack(App.Manager.Map.tileCtrl.Model);

            // 플레이어 바라보기
            var updatePos = App.Manager.Map.GetUnit<PlayerUnit>().PlayerTransform;
            transform.LookAt(new Vector3(updatePos.position.x, updatePos.position.y + 0.6f, updatePos.position.z));
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
            App.Manager.Game.TakeDamage(count);
        }
        else
        {
            if (movePath.Count == 0)
            {
                return;
            }

            for (int i = 0; i < moveRange; i++)
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

        SetModel();
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

    public void Stun(int time = 1)
    {
        remainStunTime = time;
    }
}