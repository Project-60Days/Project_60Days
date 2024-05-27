using System;
using UnityEngine;
using Hexamap;
using DG.Tweening;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField] Renderer rend;
    [SerializeField] Material cloakingMaterial;
    [SerializeField] Material normalMaterial;

    public void Move(TileController targetTileController)
    {
        transform.DOMove(targetTileController.transform.position, 0f);
        transform.DOMoveY(transform.position.y + 0.5f, 0f);
    }

    public void SetCloaking(bool _isActive)
    {
        if (_isActive)
        {
            rend.material = cloakingMaterial;
        }
        else
        {
            rend.material = normalMaterial;
        }
        
    }
}