using System;
using UnityEngine;

namespace Hexamap
{
    [PickingMethodName("Random")]
    public static class PickingMethodRandom
    {
        public static Tuple<GameObject, int> Pick(Tile tile, GameObject[] prefabs)
        {
            return new Tuple<GameObject, int>(prefabs[tile.Map.RandomObject.Next(0, prefabs.Length)], 0);
        }
    }
}