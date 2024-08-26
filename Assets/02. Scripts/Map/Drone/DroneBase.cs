using UnityEngine;
using Hexamap;
using DG.Tweening;

[System.Serializable]
public class CompassPointObjects : SerializableDictionary<CompassPoint, GameObject> { };

public abstract class DroneBase : MonoBehaviour
{
    [SerializeField] CompassPointObjects compassObj;

    public float Life { get; private set; }
    public Tile CurrTile { get; private set; }

    protected CompassPoint direction;

    public abstract DroneType GetDroneType();

    public void Set(Tile tile, CompassPoint cp)
    {
        Life = 4f;
        CurrTile = tile;
        direction = cp;
    }

    public void Move()
    {
        CurrTile.Neighbours.TryGetValue(direction, out Tile nextTile);

        Life -= 1;

        if (nextTile.Landform.GetType().Name == "LandformWorldLimit")
        {
            return;
        }
        else
        {
            transform.DOMove(nextTile.GameEntity.transform.position + Vector3.up, 0f);
            CurrTile = nextTile;
        }
    }

    public void DirectionOn(CompassPoint compass)
    {
        compassObj.TryGetValue(compass, out GameObject value);
        value.SetActive(true);
    }

    public void DirectionOff()
    {
        foreach (var compass in compassObj)
        {
            compass.Value.SetActive(false);
        }
    }
}
