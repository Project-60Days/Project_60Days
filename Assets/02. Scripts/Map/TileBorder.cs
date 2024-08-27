using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBorder : MonoBehaviour
{
    [SerializeField] MeshRenderer[] borders;
    [SerializeField] Material[] materials;

    public TileState TileState { get; private set; } = TileState.None;

    public void BorderOn(TileState _state = TileState.Moveable)
    {
        TileState = _state;

        switch (_state)
        {
            case TileState.None:
                borders[0].material = materials[0];
                break;

            case TileState.Moveable:
                borders[0].material = materials[1];
                break;

            case TileState.Unable:
                borders[0].material = materials[2];
                break;
        }

        borders[0].gameObject.SetActive(true);
    }

    public void BorderOff()
    {
        borders[0].gameObject.SetActive(false);
    }
}
