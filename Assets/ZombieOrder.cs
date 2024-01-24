using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieOrder : MonoBehaviour
{
    private List<MeshRenderer> meshRenderers;

    private void Start()
    {
        meshRenderers = new List<MeshRenderer>();
        meshRenderers.AddRange(GetComponentsInChildren<MeshRenderer>());
        
        meshRenderers.ForEach(x => x.sortingOrder = 1000);
    }
}
