using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;
using DG.Tweening;

public class DroneUnit : MapBase
{
    [SerializeField] GameObject[] prefabs;

    private List<DroneBase> drones = new();
    private List<TileController> selecteTiles = new();

    public override void ReInit()
    {
        if (drones.Count <= 0) return;

        foreach (var drone in drones)
        {
            if (drone.Life < 0)
            {
                Destroy(drone);
            }
        }

        foreach (var drone in drones)
        { 
            drone.Move();
        }
    }

    private void Destroy(DroneBase _drone)
    {
        drones.Remove(_drone);
        Destroy(_drone.gameObject);
    }

    #region Prepare
    public void Prepare(DroneType _type)
    {
        SetSelectTile();
        RemoveItem();
        GenerateDrone(prefabs[(int)_type]);
    }

    private void SetSelectTile()
    {
        var neighborTiles = hexaMap.Map.GetTilesInRange(tile.Model, 1)
            .Where(tile => tile.Ctrl.Base.canMove);

        foreach (var tile in neighborTiles)
        {
            tile.Ctrl.Base.BorderOn();
            selecteTiles.Add(tile.Ctrl);
        }
    }

    private void RemoveItem()
    {

    }

    private void GenerateDrone(GameObject _prefab)
    {
        var drone = Instantiate(_prefab, App.Manager.Map.GetUnit<PlayerUnit>().PlayerTransform.position + Vector3.up * 1.5f, Quaternion.Euler(0, 90, 0), transform);
        drone.transform.parent = transform;
        drone.GetComponentInChildren<MeshRenderer>().material.DOFade(30f, 0f);
        drones.Add(drone.GetComponent<DroneBase>());
    }
    #endregion

    #region Cancel
    public void Cancel()
    {
        ResetSelectTile();
        AddItem();
        Remove();
    }

    private void ResetSelectTile()
    {
        foreach (var tile in selecteTiles)
        {
            tile.Base.BorderOff();
        }

        selecteTiles.Clear();
    }

    private void AddItem()
    {
        var itemCode = drones.Last().GetDroneType() == DroneType.Disruptor ? "ITEM_DISTURBE" : "ITEM_FINDOR";
        App.Manager.UI.GetPanel<InventoryPanel>().AddItemByItemCode(itemCode);
    }

    private void Remove()
    {
        Destroy(drones.Last().gameObject);
        drones.Remove(drones.Last());
    }
    #endregion 

    public void SetPath(TileController _ctrl)
    {
        if (selecteTiles.Contains(_ctrl))
        {
            var drone = drones.Last();

            drone.transform.position = _ctrl.Model.GameEntity.transform.position + Vector3.up;
            drone.DirectionOff();

            if (_ctrl.Base.canMove)
                _ctrl.Base.BorderOn(TileState.Moveable);

            drone.DirectionOn(GetDirection(_ctrl));
        }
        else
        {
            _ctrl.Base.BorderOn(TileState.Unable);
        }
    }

    private CompassPoint GetDirection(TileController _ctrl)
    {
        var target = tile.Model.Neighbours.Where(target => target.Value == _ctrl.Model).ToList()[0];
        return target.Key;
    }

    #region Install
    public void Install(TileController _ctrl)
    {
        var direction = GetDirection(_ctrl);
        var drone = drones.Last();
        drone.Set(tile.Model, direction);
        drone.DirectionOff();

        drone.GetComponentInChildren<MeshRenderer>().material.DOFade(100f, 0f);

        ResetSelectTile();
    }
    #endregion

    public DroneBase CheckDisruptor(Tile tile, int range)
    {
        var disruptors = drones.Where(x => x.GetDroneType() == DroneType.Disruptor).ToList();

        if (disruptors.Count < 0) return null;

        var searchTiles = hexaMap.Map.GetTilesInRange(tile, range);

        foreach (var disruptor in disruptors)
        {
            if (searchTiles.Contains(disruptor.CurrTile))
                return disruptor;
        }

        return null;
    }
}
