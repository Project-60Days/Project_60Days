using UnityEngine;
using Hexamap;

public abstract class DroneBase : MonoBehaviour
{
    protected float lifeTime;
    public Tile currTile;

    public abstract void Move();
}
