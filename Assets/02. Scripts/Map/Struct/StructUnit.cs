using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;
using System;

public class StructUnit : MapBase
{
    [SerializeField] GameObject towerPrefab;
    [SerializeField] GameObject productionPrefab;
    [SerializeField] GameObject armyPrefab;

    public List<StructBase> GetStructObjects() => transform.GetComponentsInChildren<StructBase>(true).ToList();

    public override void Init()
    {
        GenerateTower();
        GenerateArmy();
        GenerateProduction();
    }

    public override void ReInit() 
    {
        SenseStruct()?.Around();
    }

    public void GenerateTower()
    {
        Tile tile = App.Manager.Map.GetTileFromCoords(new Coords(1, 3));

        var tilelist = new List<Tile>
        {
            tile
        };

        var spawnPos = ((GameObject)tile.GameEntity).transform.position + new Vector3(0, 0.31f, 0);

        var tower = Instantiate(towerPrefab, spawnPos, Quaternion.Euler(0, 90, 0), transform).GetComponent<StructBase>();

        tower.Init(tilelist);

        ((GameObject)tile.GameEntity).GetComponent<TileBase>().SetTower(tower);

        App.Manager.Map.preemptiveTiles.Add(tile);
    }

    public void GenerateProduction()
    {
        var boundaryTiles = GetInRangeTile(8);
        Tile centerTile = GetRandomTile(boundaryTiles, 1)[0];

        var tileList = new List<Tile>
        {
            centerTile,
            centerTile.Neighbours[CompassPoint.N],
            centerTile.Neighbours[CompassPoint.S],
            centerTile.Neighbours[CompassPoint.NE],
            centerTile.Neighbours[CompassPoint.SE],
            centerTile.Neighbours[CompassPoint.NW],
            centerTile.Neighbours[CompassPoint.SW]
        };

        AddPreemptiveTile(tileList);

        var spawnPos = ((GameObject)centerTile.GameEntity).transform.position + new Vector3(0, 0.2f, 0);

        var structure = Instantiate(productionPrefab, spawnPos, Quaternion.Euler(0, 180, 0), transform).GetComponent<StructBase>();

        structure.Init(tileList);

        foreach (var tile in tileList)
        {
            var tileBase = ((GameObject)tile.GameEntity).GetComponent<TileBase>();
            tileBase.SetProduction(structure);

            var position = tileBase.transform.position;
            position.y = ((GameObject)tile.GameEntity).transform.position.y;
            tileBase.transform.position = position;
        }
    }

    public void GenerateArmy()
    {
        var boundaryTiles = GetInRangeTile(7);
        Tile centerTile = GetRandomTile(boundaryTiles, 1)[0];

        var tileList = new List<Tile>
        {
            centerTile,
            centerTile.Neighbours[CompassPoint.NW],
            centerTile.Neighbours[CompassPoint.SW]
        };

        AddPreemptiveTile(tileList);

        var spawnPos = ((GameObject)centerTile.GameEntity).transform.position + new Vector3(0, 0.5f, 0);

        var structure = Instantiate(armyPrefab, spawnPos, Quaternion.Euler(0, 90, 0), transform).GetComponent<StructBase>();

        structure.Init(tileList);

        for (var index = 0; index < tileList.Count; index++)
        {
            var tileBase = ((GameObject)tileList[index].GameEntity).GetComponent<TileBase>();
            tileBase.SetArmy(structure);

            var position = tileBase.transform.position;
            position.y = ((GameObject)centerTile.GameEntity).transform.position.y;
            tileBase.transform.position = position;
        }
    }


    private List<Tile> GetInRangeTile(int range)
    {
        var tileList = App.Manager.Map.GetAllTiles();

        int maxDistance = 0;

        foreach (var tile in tileList)
        {
            int distance = Math.Max(Math.Abs(tile.Coords.X), Math.Abs(tile.Coords.Y));
            maxDistance = Math.Max(maxDistance, distance);
        }

        return App.Manager.Map.GetTilesInRange(maxDistance - range, App.Manager.Map.GetTileFromCoords(new Coords(0, 0)));
    }

    public List<Tile> GetRandomTile(List<Tile> tiles, int choiceNum = 1)
    {
        tiles.Remove(App.Manager.Map.tileCtrl.Model);

        return Shuffle(tiles, choiceNum);
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

    void AddPreemptiveTile(List<Tile> _tiles)
    {
        foreach (var tile in _tiles)
        {
            App.Manager.Map.preemptiveTiles.Add(tile);
        }
    }

    public StructBase SenseStruct()
    {
        var tileList = App.Manager.Map.tileCtrl.Model.Neighbours;

        foreach (var item in tileList)
        {
            if (App.Manager.Map.LandformCheck(App.Manager.Map.TileToTileController(item.Value)) == false)
                continue;

            var tileBase = ((GameObject)item.Value.GameEntity).GetComponent<TileBase>();

            if (tileBase.structure != null)
                return tileBase.structure;
        }

        return null;
    }

    public bool SenseTower() => SenseStruct() is StructTower;

    public bool SenseProduction() => SenseStruct() is StructProduction;
}
