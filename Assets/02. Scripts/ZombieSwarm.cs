using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;
using DG.Tweening;
using TMPro;

public class ZombieSwarm : MonoBehaviour
{
    public int zombieCount;
    public int foodCount;
    public int drinkCount;
    public GameObject equipment;
    public bool isChasingPlayer;
    public int remainStunTime;

    public float zombieMovePossibility;
    public float zombieStayPossibility;
    public float specailZombiePossibility;
    public float zombieMinCount;
    public float zombieMaxCount;

    public int moveCost = 1;
    public Tile curTile;
    public Tile lastTile;
    List<Coords> movePath;

    //public SpecialZombie[] specialZombies;

    public void Init(int count, Tile tile)
    {
        DataManager.instance.gameData.TryGetValue("Data_Zombie_Move_Possibility", out GameData move);
        DataManager.instance.gameData.TryGetValue("Data_Zombie_Stay_Possibility", out GameData stay);
        DataManager.instance.gameData.TryGetValue("Data_SpecialZombie_Possibility", out GameData special);
        DataManager.instance.gameData.TryGetValue("Data_MinCount_ZombieObject", out GameData min);
        DataManager.instance.gameData.TryGetValue("Data_MaxCount_ZombieObject", out GameData max);

        zombieMovePossibility = move.value;
        zombieStayPossibility = stay.value;
        specailZombiePossibility = special.value;
        zombieMinCount = min.value;
        zombieMaxCount = max.value;

        zombieCount = (int)Random.Range(zombieMinCount, zombieMaxCount);
        curTile = tile;
        lastTile = curTile;
        CurrentTileInfoUpdate(curTile);
    }

    public void DetectionPlayer()
    {
        // 데모 컨트롤러에서 범위 가져옴.
        isChasingPlayer = DemoController.instance.CalculateDistanceToPlayer(curTile, 2);
        ActionDecision();
    }

    public void ActionDecision()
    {
        if (remainStunTime > 0)
        {
            Debug.Log(gameObject.name + "은 정신을 차리지 못하고 있다.");
            remainStunTime--;
            return;
        }

        if (isChasingPlayer)
        {
            Debug.Log(gameObject.name + "이 플레이어를 발견!");
            StartCoroutine(MoveToPlayer());
        }
        else
        {
            var randomInt = GetRandom();
            if (randomInt == 0)
            {
                Debug.Log(gameObject.name + "은 정처없이 떠돌아 다니고 있다...");
                StartCoroutine(MoveToRandom());
            }
            else
            {
                Debug.Log(gameObject.name + "은 움직임을 보이지 않는다...");
            }
        }
    }

    public IEnumerator MoveToPlayer(int num = 1, float time = 0.5f)
    {
        movePath = AStar.FindPath(curTile.Coords, DemoController.instance.playerLocationTile.Coords);


        Tile targetTile;
        Vector3 targetPos;

        for (int i = 0; i < num; i++)
        {
            if (movePath.Count <= 0)
                break;

            targetTile = DemoController.instance.GetTileFromCoords(movePath[i]);
            targetPos = ((GameObject)targetTile.GameEntity).transform.position;
            targetPos.y += 1;
            gameObject.transform.DOMove(targetPos, time);
            yield return new WaitForSeconds(time);
            curTile = targetTile;
        }
        DemoController.instance.CheckSumZombies();
        CurrentTileInfoUpdate(curTile);
        CurrentTileInfoUpdate(lastTile);
        lastTile = curTile;
    }

    public IEnumerator MoveToRandom(int num = 1, float time = 0.5f)
    {
        var candidate = DemoController.instance.GetTilesInRange(curTile, num);
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
        DemoController.instance.CheckSumZombies();
        CurrentTileInfoUpdate(curTile);
        CurrentTileInfoUpdate(lastTile);
        lastTile = curTile;
    }

    public void CurrentTileInfoUpdate(Tile tile)
    {
        var uiObject = DemoController.instance.GetUi(tile);
        var text = uiObject.transform.Find("TMPs").Find("ZombieSwarmTMP").GetComponent<TMP_Text>();

        if (tile == curTile)
            text.text = "좀비 무리 : 약 " + zombieCount + "체 이상";
        else
            text.text = "좀비 무리 : 알 수 없음";
    }

    public void SumZombies(ZombieSwarm zombie)
    {
        zombieCount += zombie.zombieCount;
        foodCount += zombie.foodCount;
        drinkCount += zombie.drinkCount;
    }

    public int GetRandom()
    {
        float num = zombieMinCount / (zombieMinCount + zombieMaxCount);
        float rate = (zombieMinCount + zombieMaxCount) - ((zombieMinCount + zombieMaxCount) * zombieMinCount);
        int tmp = (int)Random.Range(0, (zombieMinCount + zombieMaxCount));

        if (tmp <= rate - 1)
        {
            return 0;
        }
        return 1;
    }
}
