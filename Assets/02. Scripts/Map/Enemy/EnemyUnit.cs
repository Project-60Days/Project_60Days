using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;

public class EnemyUnit : MapBase
{
    [SerializeField] GameObject enemyPrefab;
    private List<ZombieBase> enemyList;

    public override void Init()
    {
        base.Init();

        var tiles = App.Manager.Map.AllTile;
        tiles.Remove(tile.Model);
        var selectList = Shuffle(tiles, App.Data.Test.Map.zombieCount);

        foreach (var tile in selectList)
        { 
            SpawnEnemy(tile);
        }
    }

    public override void ReInit()
    {
        enemyList = hexaMap.Map.GetTilesInRange(tile.Model, 3)
        .Select(x => x.Ctrl.Base.enemy).Where(x => x != null).ToList();

        MoveEnemy();
        SumEnemy();
        SetAlert();
    }

    #region Spwan Enemy
    private void SpawnEnemy(Tile tile)
    {
        var spawnPos = tile.GameEntity.transform.position;
        spawnPos.y += 0.6f;

        var enemy = Instantiate(enemyPrefab, spawnPos,
            Quaternion.Euler(0, Random.Range(0, 360), 0), transform).GetComponent<ZombieBase>();

        enemy.Init(tile);
    }

    public void SpawnStructureZombies(List<Tile> tiles)
    {
        if (Random.Range(1, 4) != 1) return;

        var selectTile = Shuffle(tiles, 1)[0];

        SpawnEnemy(selectTile);
    }
    #endregion

    private void MoveEnemy()
    { 
        var disruptor = App.Manager.Map.GetUnit<DroneUnit>().CheckDisruptor(tile.Model, 3);
        var targetTile = disruptor == null ? tile.Model : disruptor.CurrTile;

        foreach (var enemy in enemyList) 
        {
            enemy.Move(targetTile);
        }
    }

    private void SumEnemy()
    {
        List<ZombieBase> removeZombies = new();

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
                    firstZombies.Sum(secondZombies);
                    removeZombies.Add(secondZombies);
                }
            }
        }

        foreach (var zombie in removeZombies)
        {
            Destroy(zombie.gameObject);
        }
    }

    private void SetAlert()
    {
        var enemyNearPlayer = hexaMap.Map.GetTilesInRange(tile.Model, 2).Select(x => x.Ctrl.Base.enemy).ToList();
        bool isExist = enemyNearPlayer.Count > 0;

        App.Manager.UI.GetPanel<FixedPanel>().SetAlert(AlertType.Caution, isExist);
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
