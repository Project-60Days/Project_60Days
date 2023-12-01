using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Hexamap;
using DG.Tweening;
using TMPro;

public class ZombieData
{
    public int count;
    public float movePossibility;
    public float stayPossibility;
    public float specailZombiePossibility;
    public float minCount;
    public float maxCount;
}

public class ZombieBase : MonoBehaviour
{
    [SerializeField] GameObject[] zombieModels;
    public ZombieData zombieData = new ZombieData();
    public Disturbtor nearthDistrubtor;
    public Tile curTile;
    public Tile lastTile;
    public Tile targetTile;
    public bool isChasingPlayer;
    
    List<Coords> movePath;
    int remainStunTime;
    int moveCost = 1;

    public void Init(Tile tile)
    {
        App.instance.GetDataManager().gameData.TryGetValue("Data_Zombie_Move_Possibility", out GameData move);
        App.instance.GetDataManager().gameData.TryGetValue("Data_Zombie_Stay_Possibility", out GameData stay);
        App.instance.GetDataManager().gameData.TryGetValue("Data_SpecialZombie_Possibility", out GameData special);
        App.instance.GetDataManager().gameData.TryGetValue("Data_MinCount_ZombieSwarm", out GameData min);
        App.instance.GetDataManager().gameData.TryGetValue("Data_MaxCount_ZombieSwarm", out GameData max);

        zombieData.movePossibility = move.value;
        zombieData.stayPossibility = stay.value;
        zombieData.specailZombiePossibility = special.value;
        zombieData.minCount = min.value;
        zombieData.maxCount = max.value;

        ZombieCountChoice();
        curTile = tile;
        lastTile = curTile;

        CurrentTileUpdate(curTile);
    }

    void ZombieCountChoice()
    {
        var randomInt = (int)Random.Range(zombieData.minCount, zombieData.maxCount);
        zombieData.count = randomInt;
        ZombieModelChoice(randomInt);
    }

    public void ZombieModelChoice(int count)
    {
        var num = (int)Mathf.Lerp(zombieData.minCount, zombieData.maxCount, 0.3f);
        num -= (int)zombieData.minCount;
        
        if (zombieData.minCount <= count && count <= zombieData.minCount + num)
        {
            for (int i = 0; i < zombieModels.Length; i++)
            {
                if(i==0)
                    zombieModels[i].SetActive(true);
                else
                    zombieModels[i].SetActive(false);
            }
        }
        else if (zombieData.minCount + num <= count && count <=zombieData.minCount + num * 2)
        {
            for (int i = 0; i < zombieModels.Length; i++)
            {
                if(i==1)
                    zombieModels[i].SetActive(true);
                else
                    zombieModels[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < zombieModels.Length; i++)
            {
                if(i==2)
                    zombieModels[i].SetActive(true);
                else
                    zombieModels[i].SetActive(false);
            }
        }
    }

    public void DetectionAndAct()
    {
        isChasingPlayer = MapController.instance.CalculateDistanceToPlayer(curTile, 2);
        nearthDistrubtor = MapController.instance.CalculateDistanceToDistrubtor(curTile, 2);
        ActionDecision();
    }

    public void ActionDecision()
    {
        if (remainStunTime > 0)
        {
            remainStunTime--;
            return;
        }

        if (nearthDistrubtor != null)
        {
            Debug.Log(gameObject.name + "가 교란기를 쫓아갑니다!");
            StartCoroutine(MoveToTarget(nearthDistrubtor.currentTile));

            return;
        }

        if (isChasingPlayer)
        {
            Debug.Log(gameObject.name + "가 플레이어를 발견했습니다!");
            StartCoroutine(MoveToTarget(App.instance.GetMapManager().mapController.Player.TileController.Model));
            
            // 플레이어 바라보기
            var updatePos = App.instance.GetMapManager().mapController.Player.transform.position;
            updatePos.y += 0.5f;
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

    public IEnumerator MoveToTarget(Tile target, int walkCount = 1, float time = 0.25f)
    {
        movePath = AStar.FindPath(curTile.Coords, target.Coords);

        Tile pointTile;
        Vector3 pointPos;

        for (int i = 0; i < walkCount; i++)
        {
            if (movePath.Count == 0)
            {
                pointTile = target;
            }
            else
            {
                pointTile = MapController.instance.GetTileFromCoords(movePath[i]);
            }
            
            pointPos = ((GameObject)pointTile.GameEntity).transform.position;
            pointPos.y += 1;
            
            gameObject.transform.DOMove(pointPos, time);
            
            yield return new WaitForSeconds(time);
            
            curTile = pointTile;
        }
        
        
        MapController.instance.CheckSumZombies();
        
        CurrentTileUpdate(curTile);
        CurrentTileUpdate(lastTile);
        
        lastTile = curTile;

        //이동한 타일에 플레이어가 있다면 공격
        if (curTile == target)
        {
            // 게임 오버
            Debug.Log("좀비 -> 플레이어 공격. 게임 오버.");
            UIManager.instance.GetNextDayController().isOver = true;
            //AttackPlayer(App.instance.GetMapManager().mapController.Player);
        }
    }

    public IEnumerator MoveToRandom(int num = 1, float time = 0.5f)
    {
        var candidate = MapController.instance.GetTilesInRange(curTile, num);
        int rand = UnityEngine.Random.Range(0, candidate.Count);

        while (((GameObject)candidate[rand].GameEntity).gameObject.layer == 8)
        {
            candidate.RemoveAt(rand);
            rand = UnityEngine.Random.Range(0, candidate.Count);
        }

        var targetPos = ((GameObject)candidate[rand].GameEntity).transform.position;
        targetPos.y += 1;

        yield return gameObject.transform.DOMove(targetPos, time);

        curTile = candidate[rand];
        MapController.instance.CheckSumZombies();
        CurrentTileUpdate(curTile);
        CurrentTileUpdate(lastTile);
        lastTile = curTile;
    }

    public void CurrentTileUpdate(Tile tile)
    {
        if (tile == curTile)
        {
            ((GameObject)(curTile.GameEntity)).GetComponent<TileBase>().UpdateZombieInfo(this);
        }
        else
        {
            ((GameObject)(curTile.GameEntity)).GetComponent<TileBase>().UpdateZombieInfo(null);
        }
    }

    public void SumZombies(ZombieBase zombie)
    {
        zombieData.count += zombie.zombieData.count;
        ZombieModelChoice(zombieData.count);
        CurrentTileUpdate(curTile);
    }

    public int GetRandom()
    {
        float percentage = zombieData.movePossibility + zombieData.stayPossibility;
        float probability = zombieData.movePossibility / percentage;
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
        player.TakeDamage(zombieData.count);
    }

    public void TakeDamage(int bulletNum)
    {
        // 피격 애니메이션

        // 내구도 감소
        zombieData.count -= bulletNum;
        
        // 좀비 수가 0이 되면 게임 오버
        if(zombieData.count <= 0)
        {
            zombieData.count = 0;
            Debug.Log(gameObject.name+" 처치 완료.");
            App.instance.GetMapManager().mapController.DeleteZombie(this);
        }
    }
}
