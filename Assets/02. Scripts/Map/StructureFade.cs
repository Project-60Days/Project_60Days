using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StructureFade : MonoBehaviour
{
    Sequence sequence;

    [SerializeField] private MeshRenderer _renderer;

    private void Start()
    {
        FadeOut();
    }

    public void FadeOut()
    {
        _renderer.material.DOFade(0.5f, 0.25f);
    }

    public void FadeIn()
    {
        _renderer.material.DOFade(1, 0.25f);
    }
}