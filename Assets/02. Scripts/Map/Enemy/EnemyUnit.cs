using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;

public class EnemyUnit : MapBase
{
    [SerializeField] GameObject enemyPrefab;

    List<ZombieBase> enemyList = new List<ZombieBase>();

    public override void Init()
    {
        var tiles = App.Manager.Map.GetAllTiles();
        tiles.Remove(tile.Model);
        var selectList = Shuffle(tiles, App.Manager.Test.Map.zombieCount);

        foreach (var tile in selectList)
        { 
            var enemy = SpawnEnemy(tile);

            enemy.Init(tile);
            enemyList.Add(enemy);
        }
    }

    public override void ReInit()
    {
        MoveEnemy();
        CheckSumZombies();
        CheckZombies();
    }

    public void SpawnStructureZombies(List<Tile> tiles)
    {
        if (Random.Range(1, 4) != 1) return;

        var selectTile = Shuffle(tiles, 1)[0];

        var zombie = SpawnEnemy(selectTile);

        zombie.Init(selectTile);
        zombie.Stun();

        enemyList.Add(zombie);
    }

    ZombieBase SpawnEnemy(Tile tile)
    {
        var spawnPos = ((GameObject)tile.GameEntity).transform.position;
        spawnPos.y += 0.6f;

        return Instantiate(enemyPrefab, spawnPos,
            Quaternion.Euler(0, Random.Range(0, 360), 0), transform).GetComponent<ZombieBase>();
    }

    public void MoveEnemy()
    {
        foreach (var enemy in enemyList) 
        {
            enemy.DetectionAndAct();
        }
    }

    public void CheckSumZombies()
    {
        List<ZombieBase> removeZombies = new List<ZombieBase>();

        for (int i = 0; i < enemyList.Count - 1; i++)
        {
            for (int j = i + 1; j < enemyList.Count; j++)
            {
                var firstZombies = enemyList[i];
                var secondZombies = enemyList[j];

                if (firstZombies.count == 0 || secondZombies.count == 0)
                    continue;

                if (firstZombies.currTile == secondZombies.currTile)
                {
                    firstZombies.SumZombies(secondZombies);
                    removeZombies.Add(secondZombies);
                }
            }
        }

        foreach (var zombie in removeZombies)
        {
            enemyList.Remove(zombie);
            Destroy(zombie.gameObject);
        }
    }

    public void CheckZombies()
    {
        var playerNearthTiles = App.Manager.Map.GetTilesInRange(2);

        foreach (var enemy in enemyList) 
        {
            if (playerNearthTiles.Contains(enemy.currTile))
            {
                App.Manager.UI.GetPanel<FixedPanel>().SetAlert(AlertType.Caution, true);
                return;
            }
        }

        App.Manager.UI.GetPanel<FixedPanel>().SetAlert(AlertType.Caution, false);
    }

    private List<T> Shuffle<T>(List<T> _list, int _range)
    {
        System.Random rand = new();

        int n = _list.Count;

        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);

            T value = _list[k];
            _list[k] = _list[n];
            _list[n] = value;
        }

        return _list.GetRange(0, _range);
    }
}
