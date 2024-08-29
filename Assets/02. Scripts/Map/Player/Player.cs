using System;
using UnityEngine;
using Hexamap;
using DG.Tweening;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField] Renderer render;
    [SerializeField] Material cloakingMaterial;
    [SerializeField] Material normalMaterial;

    private bool isFloating = false;

    private void Start()
    {
        StartFloat();

        App.Manager.Event.PostEvent(EventCode.PlayerCreate, this, transform);
    }

    private void StartFloat()
    {
        if (!isFloating)
        {
            isFloating = true;
            transform.DOMoveY(transform.position.y + 0.4f, 5)
                     .SetEase(Ease.InOutSine)
                     .SetLoops(-1, LoopType.Yoyo)
                     .OnKill(() => isFloating = false);
        }
    }

    public void Move(TileBase _tile)
    {
        transform.DOKill();

        transform.DOMove(_tile.transform.position, 0f)
                 .OnComplete(() =>
                 {
                     transform.DOMoveY(transform.position.y + 0.5f, 0f)
                              .OnComplete(StartFloat);
                 });
    }

    public void SetCloaking(bool _isActive)
    {
        render.material = _isActive ? cloakingMaterial : normalMaterial;
    }
}