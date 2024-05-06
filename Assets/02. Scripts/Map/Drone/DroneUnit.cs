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
    [SerializeField] Transform mapTransform;

    List<GameObject> disruptors = new List<GameObject>();
    GameObject currDisruptor;

    List<GameObject> explorers = new List<GameObject>();
    GameObject currExplorer;

    List<DroneBase> drones = new List<DroneBase>();

    [SerializeField] GameObject disruptorPrefab;
    [SerializeField] GameObject explorerPrefab;

    List<TileController> droneSelectedTiles = new List<TileController>();

    public override void Init()
    {
        
    }
    public override void ReInit()
    {
        if (drones.Count <= 0) return;

        foreach (var drone in drones)
        {
            drone.Move();
        }
    }

    public void PreparingDistrubtor()
    {
        var neighborTiles = hexaMap.Map.GetTilesInRange(App.Manager.Map.tileCtrl.Model, 1);

        var neighborController = neighborTiles
            .Select(x => ((GameObject)x.GameEntity).GetComponent<TileController>()).ToList();

        for (var index = 0; index < neighborController.Count; index++)
        {
            var value = neighborController[index];
            if (App.Manager.Map.LandformCheck(value) == false)
                continue;
            droneSelectedTiles.Add(value);
            SelectTargetBorder(value);
        }

        GenerateExampleDisturbtor();
        App.Manager.Map.SetIsDronePrepared(true, "Distrubtor");
    }

    void GenerateExampleDisturbtor()
    {
        Debug.Log("예시 교란기");

        currDisruptor = Instantiate(disruptorPrefab,
            App.Manager.Map.playerCtrl.PlayerTransform + Vector3.up * 1.5f, Quaternion.Euler(0, 90, 0));

        currDisruptor.transform.parent = mapTransform;
        currDisruptor.GetComponentInChildren<MeshRenderer>(true).material.DOFade(50, 0);
        disruptors.Add(currDisruptor);
        drones.Add(currDisruptor.GetComponent<DroneBase>());
    }

    public void CancelDisrubtor()
    {
        disruptors.Remove(currDisruptor);
        drones.Remove(currDisruptor.GetComponent<DroneBase>());
        App.Manager.Map.SetIsDronePrepared(false, "Distrubtor");
        App.Manager.UI.GetPanel<InventoryPanel>().AddItemByItemCode("ITEM_DISTURBE");
        Destroy(currDisruptor);
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
        GenerateExampleExplorer();
        App.Manager.Map.SetIsDronePrepared(true, "Explorer");
    }

    public void CancelExplorer()
    {
        explorers.Remove(currExplorer);
        drones.Remove(currExplorer.GetComponent<DroneBase>());
        App.Manager.Map.SetIsDronePrepared(false, "Explorer");
        App.Manager.UI.GetPanel<InventoryPanel>().AddItemByItemCode("ITEM_FINDOR");
        Destroy(currExplorer);
    }

    void GenerateExampleExplorer()
    {
        currExplorer = Instantiate(explorerPrefab,
            App.Manager.Map.playerCtrl.PlayerTransform + Vector3.up * 1.5f, Quaternion.Euler(0, 90, 0));

        currExplorer.transform.parent = mapTransform;

        currExplorer.GetComponentInChildren<MeshRenderer>().material.DOFade(50, 0);
        currExplorer.GetComponent<Explorer>().Set(App.Manager.Map.tileCtrl.Model);

        explorers.Add(currExplorer);
        drones.Add(currExplorer.GetComponent<DroneBase>());
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
        tileController.GetComponent<Borders>().GetDisturbanceBorder().SetActive(true);
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
