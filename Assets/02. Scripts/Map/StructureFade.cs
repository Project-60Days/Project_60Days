using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using DG.Tweening;
using UnityEngine.EventSystems;
using Sequence = DG.Tweening.Sequence;
using UnityEngine.EventSystems;

public class StructureFade : MonoBehaviour
{
    private Material curMaterial;
    [SerializeField] private Renderer rend;
    [SerializeField] Material cloakingMaterial;

    void Start()
    {
        curMaterial = rend.material;
    }

    public void FadeIn()
    {
        rend.material = cloakingMaterial;
    }
    
    public void FadeOut()
    {
        rend.material = curMaterial;
    }
}