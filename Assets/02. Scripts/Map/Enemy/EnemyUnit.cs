using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;

public class EnemyUnit : MapBase
{
    List<GameObject> enemyList = new List<GameObject>();
    [SerializeField] GameObject enemyPrefab;

    [Header("Æ®·£½ºÆû")]
    [Space(5f)]
    [SerializeField]
    Transform enemyTrans;

    public override void Init()
    {
        var tiles = App.Manager.Map.GetAllTiles();
        var selectList = Shuffle(tiles);

        foreach (var tile in selectList)
        { 
            var enemy = SpawnEnemy(tile);

            enemy.GetComponent<ZombieBase>().Init(tile);
            enemy.GetComponent<ZombieBase>().SetValue(data.playerMovementPoint, data.zombieDetectionRange);
            enemyList.Add(enemy);
        }
    }

    public void SpawnStructureZombies(List<TileBase> tiles)
    {
        var randomInt = Random.Range(0, tiles.Count);
        var tile = tiles[randomInt];

        var spawnPos = tile.transform.position;
        spawnPos.y += 0.6f;

        var zombie = Instantiate(enemyPrefab, spawnPos,
            Quaternion.Euler(0, Random.Range(0, 360), 0), enemyTrans);

        zombie.name = "Structure Zombie";
        zombie.GetComponent<ZombieBase>().Init(tile.GetComponent<TileController>().Model);
        zombie.GetComponent<ZombieBase>().Stun();

        enemyList.Add(zombie);
    }

    GameObject SpawnEnemy(Tile tile)
    {
        var spawnPos = ((GameObject)tile.GameEntity).transform.position;
        spawnPos.y += 0.6f;


        return Instantiate(enemyPrefab, spawnPos,
            Quaternion.Euler(0, Random.Range(0, 360), 0), enemyTrans);
    }

    public override void ReInit()
    {
        MoveEnemy();
        CheckSumZombies();
    }

    public void MoveEnemy()
    {
        for (var index = 0; index < enemyList.Count; index++)
        {
            var zombie = enemyList[index];

            zombie.GetComponent<ZombieBase>().DetectionAndAct();
        }
    }

    public void CheckSumZombies()
    {
        List<ZombieBase> zombieBases = enemyList.Select(x => x.GetComponent<ZombieBase>()).ToList();
        List<ZombieBase> removeZombies = new List<ZombieBase>();

        for (int i = 0; i < zombieBases.Count; i++)
        {
            for (int j = i + 1; j < zombieBases.Count; j++)
            {
                var firstZombies = zombieBases[i];
                var secondZombies = zombieBases[j];

                if (firstZombies.count == 0 || secondZombies.count == 0)
                    continue;

                if (firstZombies.currTile == secondZombies.currTile)
                {
                    firstZombies.SumZombies(secondZombies);
                    removeZombies.Add(secondZombies);
                }
            }
        }

        for (int i = 0; i < removeZombies.Count(); i++)
        {
            var item = removeZombies[i];
            enemyList.Remove(item.gameObject);
            Destroy(item.gameObject);
        }
    }

    public bool CheckZombies()
    {
        var playerNearthTiles = App.Manager.Map.GetTilesInRange(2);

        for (int i = 0; i < enemyList.Count; i++)
        {
            GameObject item = enemyList[i];
            if (playerNearthTiles.Contains(item.GetComponent<ZombieBase>().currTile))
            {
                return true;
            }
        }

        return false;
    }

    public List<T> Shuffle<T>(List<T> list)
    {
        System.Random rand = new System.Random();

        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);

            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list.GetRange(0, data.zombieCount);
    }
}
