using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;
using System;

public class StructCtrl : MonoBehaviour
{
    [SerializeField] GameObject towerPrefab;
    [SerializeField] GameObject productionPrefab;
    [SerializeField] GameObject armyPrefab;

    [SerializeField] Transform objectsTransform;

    public List<StructureObject> GetStructureObjects() => objectsTransform.GetComponentsInChildren<StructureObject>(true).ToList();

    public void Init()
    {
        GenerateTower();
        GenerateArmy(new Coords(0, 0));
        GenerateProduction(new Coords(0, 0));
    }
    public void GenerateTower()
    {
        // 경계선으로부터 2칸 이내 범위 
        // List<int> selectedTiles = RandomTileSelect(ObjectSpawnDistanceCalculate(2),
        //     EObjectSpawnType.ExcludeEntites, 1);

        var tilelist = new List<Tile>();

        // 튜토리얼 용 위치 고정
        Tile tile = App.Manager.Map.mapCtrl.GetTileFromCoords(new Coords(1, 3));

        tilelist.Add(tile);

        List<Tile> neighborList = SetNeighborStructure(tilelist);

        var spawnPos = ((GameObject)tile.GameEntity).transform.position;
        spawnPos.y += 0.31f;

        var tower = Instantiate(towerPrefab, spawnPos, Quaternion.Euler(0, 90, 0),
            objectsTransform);

        tower.GetComponent<StructureObject>().Init(tile);

        ((GameObject)tile.GameEntity).GetComponent<TileBase>().SpawnQuestStructure(neighborList, tower);

        App.Manager.Map.mapCtrl.preemptiveTiles.Add(tile);
    }

    public void GenerateProduction(Coords _coords)
    {
        //경계선으로부터 5칸 이내 범위 
        var boundaryTiles = ObjectSpawnDistanceCalculate(8);
        List<int> selectNumber = RandomTileSelect(boundaryTiles, EObjectSpawnType.ExcludeEntites, 1);

        var tilelist = new List<Tile>();

        GameObject structureObject = productionPrefab;

        // 튜토리얼 용 위치 고정
        //Tile tile = GetTileFromCoords(_coords);

        Tile tile = boundaryTiles[selectNumber[0]];

        tilelist.Add(tile);
        tilelist.Add(tile.Neighbours[CompassPoint.N]);
        tilelist.Add(tile.Neighbours[CompassPoint.S]);
        tilelist.Add(tile.Neighbours[CompassPoint.NE]);
        tilelist.Add(tile.Neighbours[CompassPoint.SE]);
        tilelist.Add(tile.Neighbours[CompassPoint.NW]);
        tilelist.Add(tile.Neighbours[CompassPoint.SW]);

        foreach (var item in tilelist)
        {
            App.Manager.Map.mapCtrl.preemptiveTiles.Add(item);
        }

        List<Tile> neighborList = SetNeighborStructure(tilelist);

        var spawnPos = ((GameObject)tile.GameEntity).transform.position;
        spawnPos.y += 0.2f;

        var structure = Instantiate(structureObject, spawnPos, Quaternion.Euler(0, 180, 0),
            objectsTransform);

        structure.GetComponent<StructureObject>().Init(tile);

        structure.name = "Production";

        StructureInfo structureInfo = new StructureInfo(neighborList, tilelist, structure, EStructure.Production);

        for (var index = 0; index < tilelist.Count; index++)
        {
            var tileBase = ((GameObject)tilelist[index].GameEntity).GetComponent<TileBase>();
            tileBase.SpawnNormalStructure(structureInfo);

            var position = tileBase.transform.position;
            position.y = ((GameObject)tile.GameEntity).transform.position.y;
            tileBase.transform.position = position;
        }
    }

    public void GenerateArmy(Coords _coords)
    {
        //경계선으로부터 5칸 이내 범위 
        var boundaryTiles = ObjectSpawnDistanceCalculate(7);
        List<int> selectNumber = RandomTileSelect(boundaryTiles, EObjectSpawnType.ExcludeEntites, 1);

        var tilelist = new List<Tile>();

        Tile tile = boundaryTiles[selectNumber[0]];

        // 튜토리얼 용 위치 고정
        //Tile tile = GetTileFromCoords(_coords);

        tilelist.Add(tile);
        tilelist.Add(tile.Neighbours[CompassPoint.NW]);
        tilelist.Add(tile.Neighbours[CompassPoint.SW]);

        foreach (var item in tilelist)
        {
            App.Manager.Map.mapCtrl.preemptiveTiles.Add(item);
        }

        List<Tile> neighborList = SetNeighborStructure(tilelist);

        var spawnPos = ((GameObject)tile.GameEntity).transform.position;
        spawnPos.y += 0.5f;

        var structure = Instantiate(armyPrefab, spawnPos, Quaternion.Euler(0, 90, 0),
            objectsTransform);

        structure.GetComponent<StructureObject>().Init(tile);

        structure.name = "Army";
        StructureInfo structureInfo = new StructureInfo(neighborList, tilelist, structure, EStructure.Army);

        for (var index = 0; index < tilelist.Count; index++)
        {
            var tileBase = ((GameObject)tilelist[index].GameEntity).GetComponent<TileBase>();
            tileBase.SpawnNormalStructure(structureInfo);

            var position = tileBase.transform.position;
            position.y = ((GameObject)tile.GameEntity).transform.position.y;
            tileBase.transform.position = position;
        }
    }


    List<Tile> ObjectSpawnDistanceCalculate(int range)
    {
        var tileList = App.Manager.Map.mapCtrl.GetAllTiles();

        int biggerInt = 0;
        int maxInt = 0;

        for (int i = 0; i < tileList.Count; i++)
        {
            Tile lastIndex = tileList[i];

            biggerInt = Math.Abs(lastIndex.Coords.X) > Math.Abs(lastIndex.Coords.Y)
                ? Math.Abs(lastIndex.Coords.X)
                : Math.Abs(lastIndex.Coords.Y);

            if (biggerInt > maxInt)
                maxInt = biggerInt;
        }

        List<Tile> excludeTileList = App.Manager.Map.mapCtrl.GetTilesInRange(maxInt - range, App.Manager.Map.mapCtrl.GetTileFromCoords(new Coords(0, 0)));
        return excludeTileList;
    }



    List<Tile> SetNeighborStructure(List<Tile> tiles)
    {
        List<Tile> neighborTiles = new List<Tile>();

        for (int i = 0; i < tiles.Count; i++)
        {
            var tile = tiles[i];
            var list = tile.Neighbours.Select(kvp => kvp.Value).ToList();
            neighborTiles.AddRange(list);
        }

        neighborTiles = neighborTiles.Distinct().ToList();

        for (var index = 0; index < neighborTiles.Count; index++)
        {
            var tile = neighborTiles[index];
        }

        return neighborTiles;
    }


    public List<int> RandomTileSelect(List<Tile> tiles, EObjectSpawnType type, int choiceNum = 1)
    {
        List<int> selectTileNumber = new List<int>();

        if (tiles == null || tiles.Count == 0)
        {
            selectTileNumber.Add(5);
            return selectTileNumber;
        }

        // 플레이어와 겹치지 않는 랜덤 타일 뽑기.
        while (selectTileNumber.Count != choiceNum)
        {
            int randomInt = UnityEngine.Random.Range(0, tiles.Count);

            if (App.Manager.Map.mapCtrl.ConditionalBranch(type, tiles[randomInt]))
            {
                if (selectTileNumber.Contains(randomInt) == false)
                {
                    selectTileNumber.Add(randomInt);
                    App.Manager.Map.mapCtrl.preemptiveTiles.Add(tiles[randomInt]);
                }
            }
        }

        return selectTileNumber;
    }

    public bool SensingSignalTower()
    {
        var structure = SensingStructure();

        if (structure is Tower)
            return true;
        else
            return false;
    }

    public bool SensingProductionStructure()
    {
        var structure = SensingStructure();

        if (structure is ProductionStructure)
            return true;
        else
            return false;
    }

    public StructureBase SensingStructure()
    {
        var tileList = App.Manager.Map.mapCtrl.tileCtrl.Model.Neighbours;

        foreach (var item in tileList)
        {
            if (App.Manager.Map.mapCtrl.LandformCheck(App.Manager.Map.mapCtrl.TileToTileController(item.Value)) == false)
                continue;

            var tileBase = ((GameObject)item.Value.GameEntity).GetComponent<TileBase>();

            if (tileBase.structure != null)
                return tileBase.structure;
        }

        return null;
    }

    /// <summary>
    /// 현재 타일이 구조물 인접타일인지 확인
    /// </summary>
    public void CheckStructureNeighbor()
    {
        var structure = SensingStructure();
        if (structure != null)
        {
            if (structure is Tower)
                if (App.Manager.UI.GetPanel<InventoryPanel>().CheckNetCardUsage() == false) return;

            if (structure.isUse == false)
                App.Manager.UI.GetPanel<PagePanel>().SetSelectPage("structureSelect", structure);
        }
    }
}
