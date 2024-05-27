using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;

public class StructUnit : MapBase
{
    [SerializeField] GameObject towerPrefab;
    [SerializeField] GameObject productionPrefab;
    [SerializeField] GameObject armyPrefab;

    public override void Init()
    {
        GenerateTower();
        GenerateArmy();
        GenerateProduction();
    }

    public override void ReInit() 
    {
        SenseStruct()?.DetectStruct();
    }

    #region Generate Structure
    private void GenerateTower()
    {
        Tile tile = App.Manager.Map.GetTileFromCoords(new Coords(1, 3));

        var tilelist = new List<Tile>
        {
            tile
        };

        var spawnPos = tile.GameEntity.transform.position + new Vector3(0, 0.31f, 0);

        var tower = Instantiate(towerPrefab, spawnPos, Quaternion.Euler(0, 90, 0), transform).GetComponent<StructBase>();

        tower.Init(tilelist);

        tile.Ctrl.Base.SetStruct(tower);
    }

    private void GenerateProduction()
    {
        var boundaryTiles = GetInRangeTile(8);
        Tile centerTile = GetRandomTile(boundaryTiles);

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

        var spawnPos = centerTile.GameEntity.transform.position + new Vector3(0, 0.2f, 0);

        var structure = Instantiate(productionPrefab, spawnPos, Quaternion.Euler(0, 180, 0), transform).GetComponent<StructBase>();

        SetStruct(structure, tileList);
    }

    private void GenerateArmy()
    {
        var boundaryTiles = GetInRangeTile(7);
        Tile centerTile = GetRandomTile(boundaryTiles);

        var tileList = new List<Tile>
        {
            centerTile,
            centerTile.Neighbours[CompassPoint.NW],
            centerTile.Neighbours[CompassPoint.SW]
        };

        var spawnPos = centerTile.GameEntity.transform.position + new Vector3(0, 0.5f, 0);

        var structure = Instantiate(armyPrefab, spawnPos, Quaternion.Euler(0, 90, 0), transform).GetComponent<StructBase>();

        SetStruct(structure, tileList);
    }

    private List<Tile> GetInRangeTile(int range)
    {
        var tileInRange = hexaMap.Map.GetTilesInRange(tile.Model, range);
        var tileInRangeInner = hexaMap.Map.GetTilesInRange(tile.Model, range - 1);

        return tileInRange.Except(tileInRangeInner).ToList();
    }

    private Tile GetRandomTile(List<Tile> tiles)
    {
        int random = Random.Range(0, tiles.Count);
        return tiles[random];
    }

    private void SetStruct(StructBase _struct, List<Tile> _tiles)
    {
        _struct.Init(_tiles);

        for (var index = 0; index < _tiles.Count; index++)
        {
            var tileBase = _tiles[index].Ctrl.Base;
            tileBase.SetStruct(_struct);

            var position = tileBase.transform.position;
            position.y = _tiles[0].GameEntity.transform.position.y;
            tileBase.transform.position = position;
        }
    }
    #endregion

    private StructBase SenseStruct()
    {
        var tileList = tile.Model.Neighbours;

        foreach (var tile in tileList)
        {
            var tileBase = tile.Value.Ctrl.Base;

            if (tileBase.structure != null)
                return tileBase.structure;
        }

        return null;
    }
}
