using System;
using UnityEngine;

namespace Hexamap
{
    [PickingMethodName("First")]
    public static class PickingMethodFirst
    {
        public static Tuple<GameObject, int> Pick(Tile tile, GameObject[] prefabs)
        {
            return new Tuple<GameObject, int>(prefabs[0], 0);
        }
    }
}