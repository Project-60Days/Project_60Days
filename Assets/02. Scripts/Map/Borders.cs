using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Borders : MonoBehaviour
{
    [SerializeField] MeshRenderer[] borders;
    [SerializeField] Material[] materials;

    ETileState currentTileState = ETileState.None;

    public void BorderOn(ETileState _state)
    {
        currentTileState = _state;

        switch (_state)
        {
            case ETileState.None:
                borders[0].material = materials[0];
                break;

            case ETileState.Moveable:
                borders[0].material = materials[1];
                break;

            case ETileState.Unable:
                borders[0].material = materials[2];
                break;

            case ETileState.Target:
                borders[1].gameObject.SetActive(true);
                return;
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

    public ETileState GetEtileState()
    {
        return currentTileState;
    }
}
