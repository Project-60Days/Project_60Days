using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;
using DG.Tweening;

public class DroneUnit : MapBase
{
    [Header("컴포넌트")]
    [Space(5f)]
    [SerializeField]
    HexamapController hexaMap;

    List<GameObject> disruptors = new List<GameObject>();
    GameObject currDisruptor;

    List<GameObject> explorers = new List<GameObject>();
    GameObject currExplorer;

    List<DroneBase> drones = new List<DroneBase>();

    [SerializeField] GameObject disruptorPrefab;
    [SerializeField] GameObject explorerPrefab;

    List<TileController> droneSelectedTiles = new List<TileController>();

    public override void Init() { }

    public override void ReInit()
    {
        if (drones.Count <= 0) return;

        foreach (var drone in drones)
        {
            drone.Move();
        }
    }

    void GenerateDrone(GameObject prefab, List<GameObject> list)
    {
        var drone = Instantiate(prefab, App.Manager.Map.playerCtrl.PlayerTransform + Vector3.up * 1.5f, Quaternion.Euler(0, 90, 0), transform);
        drone.transform.parent = transform;
        drone.GetComponentInChildren<MeshRenderer>().material.DOFade(50, 0);
        list.Add(drone);
        drones.Add(drone.GetComponent<DroneBase>());
    }

    void RemoveDrone(GameObject drone, List<GameObject> list)
    {
        list.Remove(drone);
        Destroy(drone);
    }

    void SetDronePrepared(bool isPrepared, string type)
    {
        App.Manager.Map.SetIsDronePrepared(isPrepared, type);
    }

    public void PreparingDisruptor()
    {
        var neighborTiles = hexaMap.Map.GetTilesInRange(App.Manager.Map.tileCtrl.Model, 1)
            .Select(tile => ((GameObject)tile.GameEntity).GetComponent<TileController>())
            .Where(tileController => App.Manager.Map.LandformCheck(tileController));

        foreach (var tile in neighborTiles) 
        {
            SelectTargetBorder(tile);
            droneSelectedTiles.Add(tile);
        }

        GenerateDrone(disruptorPrefab, disruptors);
        SetDronePrepared(true, "Distrubtor");
    }

    public void CancelDisrubtor()
    {
        RemoveDrone(disruptors.Last(), disruptors);
        SetDronePrepared(false, "Distrubtor");
        App.Manager.UI.GetPanel<InventoryPanel>().AddItemByItemCode("ITEM_DISTURBE");
        DeselectAllTargetTiles();
    }

    public void DisrubtorPathFinder(TileController tileController)
    {
        if (droneSelectedTiles.Contains(tileController))
        {
            currDisruptor.transform.position =
                ((GameObject)tileController.Model.GameEntity).transform.position + Vector3.up;

            currDisruptor.GetComponent<Distrubtor>().DirectionObjectOff();

            if (App.Manager.Map.LandformCheck(tileController))
                App.Manager.Map.SelectBorder(tileController, ETileState.Moveable);

            foreach (var item in App.Manager.Map.tileCtrl.Model.Neighbours.Where(
                         item => item.Value == tileController.Model))
            {
                currDisruptor.GetComponent<Distrubtor>().GetDirectionObject(item.Key).SetActive(true);
            }
        }
        else
        {
            App.Manager.Map.SelectBorder(tileController, ETileState.Unable);
        }
    }

    void InstallDistrubtor(TileController tileController, CompassPoint direction)
    {
        currDisruptor.GetComponent<Distrubtor>().Set(tileController.Model, direction);
        currDisruptor.GetComponent<Distrubtor>().DirectionObjectOff();

        for (int i = 0; i < droneSelectedTiles.Count; i++)
        {
            DeselecTargetBorder(droneSelectedTiles[i]);
        }

        App.Manager.Map.SetIsDronePrepared(false, "Distrubtor");
    }

    public void PreparingExplorer()
    {
        GenerateDrone(explorerPrefab, explorers);
        SetDronePrepared(true, "Explorer");
    }

    public void CancelExplorer()
    {
        RemoveDrone(explorers.Last(), explorers);
        SetDronePrepared(false, "Explorer");
        App.Manager.UI.GetPanel<InventoryPanel>().AddItemByItemCode("ITEM_FINDOR");
    }

    void InstallExplorer(TileController tileController)
    {
        currExplorer.GetComponent<Explorer>().Targeting(tileController.Model);
        currExplorer.GetComponent<Explorer>().Move();

        App.Manager.Map.SetIsDronePrepared(false, "");
    }

    public void SelectTileForDisturbtor(TileController tileController)
    {
        if (App.Manager.Map.LandformCheck(tileController) == false)
            return;

        if (tileController.GetComponent<Borders>().GetEtileState() == ETileState.Moveable
            && App.Manager.Map.tileCtrl.Model != tileController.Model)
        {
            foreach (var item in App.Manager.Map.tileCtrl.Model.Neighbours.Where(
                         item => item.Value == tileController.Model))
            {
                Debug.Log("설치 시작");
                InstallDistrubtor(tileController, item.Key);
            }
        }
    }

    public void SelectTileForExplorer(TileController tileController)
    {
        if (tileController.GetComponent<Borders>().GetEtileState() == ETileState.Moveable
            && App.Manager.Map.tileCtrl.Model != tileController.Model)
        {
            InstallExplorer(tileController);
        }
    }

    public Distrubtor CalculateDistanceToDistrubtor(Tile tile, int range)
    {
        var searchTiles = hexaMap.Map.GetTilesInRange(tile, range);

        if (disruptors.Count <= 0)
            return null;

        for (var i = 0; i < searchTiles.Count; i++)
        {
            var item = searchTiles[i];

            for (var index = 0; index < disruptors.Count; index++)
            {
                var distrubtor = disruptors[index];

                if (distrubtor.GetComponent<Distrubtor>().currTile == item)
                    return distrubtor.GetComponent<Distrubtor>();
            }
        }

        return null;
    }

    public void RemoveDistrubtor(Distrubtor _distrubtor)
    {
        disruptors.Remove(_distrubtor.gameObject);
        drones.Remove(currExplorer.GetComponent<DroneBase>());
    }

    public void RemoveExplorer(Explorer _explorer)
    {
        explorers.Remove(_explorer.gameObject);
        drones.Remove(currExplorer.GetComponent<DroneBase>());
    }

    public void InvocationExplorers()
    {
        for (int i = 0; i < explorers.Count; i++)
        {
            var item = explorers[i].GetComponent<Explorer>();

            if (item.GetIsIdle())
                item.GetComponent<Explorer>().Invocation();
        }
    }

    void SelectTargetBorder(TileController tileController)
    {
        tileController.GetComponent<Borders>().BorderOn(ETileState.Target);
        droneSelectedTiles.Add(tileController);
    }

    void DeselecTargetBorder(TileController tileController)
    {
        tileController.GetComponent<Borders>().OffTargetBorder();

        if (droneSelectedTiles.Contains(tileController))
            droneSelectedTiles.Remove(tileController);
    }

    public void DeselectAllTargetTiles()
    {
        if (droneSelectedTiles == null)
            return;

        for (int i = 0; i < droneSelectedTiles.Count; i++)
        {
            TileController tile = droneSelectedTiles[i];
            DeselecTargetBorder(tile);
        }

        droneSelectedTiles.Clear();
    }
}
