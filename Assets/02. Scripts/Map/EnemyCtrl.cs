using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;

public class EnemyCtrl : MonoBehaviour
{
    List<GameObject> enemyList = new List<GameObject>();
    [SerializeField] GameObject enemyPrefab;

    [Header("트랜스폼")]
    [Space(5f)]
    [SerializeField]
    Transform enemyTrans;


    public void SpawnZombies(List<Tile> _tileList, List<int> _selectList)
    {
        // 오브젝트 생성.
        for (int i = 0; i < _selectList.Count; i++)
        {
            var tile = _tileList[_selectList[i]];
            var spawnPos = ((GameObject)tile.GameEntity).transform.position;
            spawnPos.y += 0.6f;

            var zombie = Instantiate(enemyPrefab, spawnPos,
                Quaternion.Euler(0, Random.Range(0, 360), 0), enemyTrans);
            zombie.name = "Zombie " + (i + 1);
            zombie.GetComponent<ZombieBase>().Init(tile);
            zombie.GetComponent<ZombieBase>().SetValue(App.Manager.Map.data.playerMovementPoint, App.Manager.Map.data.zombieDetectionRange);
            enemyList.Add(zombie);
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
        var playerNearthTiles = App.Manager.Map.mapCtrl.GetTilesInRange(2);

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
}
