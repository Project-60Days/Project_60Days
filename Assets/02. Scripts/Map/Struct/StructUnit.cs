using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;

public class StructUnit : MapBase
{
    [SerializeField] GameObject[] structPrefabs;

    public override void Init()
    {
        base.Init();

        GenerateStruct();
    }

    public override void ReInit() 
    {
        var senseStruct = SenseStruct();

        senseStruct.DetectStruct();
    }

    #region Generate Structure
    private void GenerateStruct()
    {
        foreach (var prefab in structPrefabs)
        {
            var structure = Instantiate(prefab, Vector3.zero, Quaternion.Euler(0, 90, 0), transform).GetComponent<StructBase>();
            structure.SetData();

            var boundaryTiles = GetInRangeTile(structure.Data.SpawnRange);
            Tile centerTile = GetRandomTile(boundaryTiles);

            var tileList = GenerateTileList(centerTile, structure.Data.Direction);

            var spawnPos = centerTile.GameEntity.transform.position + new Vector3(0, 0.31f, 0);
            structure.transform.position = spawnPos;

            SetStruct(structure, tileList);
        }
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

    private List<Tile> GenerateTileList(Tile centerTile, int directionCount)
    {
        List<Tile> tileList = new() { centerTile };
        for (int i = 0; i < directionCount - 1; i++)
        {
            if (centerTile.Neighbours.TryGetValue((CompassPoint)i, out var neighbourTile))
            {
                tileList.Add(neighbourTile);
            }
        }
        
        return tileList;
    }

    private void SetStruct(StructBase _struct, List<Tile> _tiles)
    {
        _struct.Init(_tiles);

        for (var index = 0; index < _tiles.Count; index++)
        {
            var tileBase = _tiles[index].Ctrl;
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
            var tileBase = tile.Value.Ctrl;

            if (tileBase.structure != null)
            {
                return tileBase.structure;
            }
        }

        return null;
    }
}
