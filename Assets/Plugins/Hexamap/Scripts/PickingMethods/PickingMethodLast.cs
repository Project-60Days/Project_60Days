using Hexamap;
using System;
using System.Linq;
using UnityEngine;

[PickingMethodName("Last")]
public static class PickingMethodLast
{
    public static Tuple<GameObject, int> Pick(Tile tile, GameObject[] prefabs)
    {
        return new Tuple<GameObject, int>(prefabs.Last(), 60);
    }
}