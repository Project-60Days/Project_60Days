using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBorder : MonoBehaviour
{
    [SerializeField] MeshRenderer[] borders;
    [SerializeField] Material[] materials;

    TileState currentTileState = TileState.None;

    public void BorderOn(TileState _state)
    {
        currentTileState = _state;

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
 
    public void OffNormalBorder()
    {
        borders[0].gameObject.SetActive(false);
    }
    
    public void OffTargetBorder()
    {
        borders[1].gameObject.SetActive(false);
    }

    public TileState GetTileState()
    {
        return currentTileState;
    }
}
