using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hexamap
{
    [PickingMethodName("Connections")]
    public static class PickingMethodConnections
    {
        /* 
         * PREFABS :
         * 00 = 110000
         * 01 = 101000
         * 02 = 100100
         * 03 = 111000
         * 04 = 110100
         * 05 = 110010
         * 06 = 101010
         * 07 = 111100
         * 08 = 111010
         * 09 = 110110
         * 10 = 111110
         * 11 = 111111
         */
        private static readonly string[] prefabsMap = {
            "110000",
            "101000",
            "100100",
            "111000",
            "110100",
            "110010",
            "101010",
            "111100",
            "111010",
            "110110",
            "111110",
            "111111"
        };

        public static Tuple<GameObject, int> Pick(Tile tile, GameObject[] prefabs)
        {
            var adjacentConnections = getAdjacentConnections(tile);
            var adjacentLandforms = getAdjacentLandforms(tile);
            var adjacentAll = adjacentLandforms
                .Concat(adjacentConnections)
                .ToDictionary(x => x.Key, x => x.Value);

            string layout = string.Empty;
            foreach (CompassPoint compassPoint in Enum.GetValues(typeof(CompassPoint)))
            {
                if (adjacentAll.ContainsKey(compassPoint))
                    layout += "1";
                else
                    layout += "0";
            }

            int shiftCounter = 0;
            while (!prefabsMap.Contains(layout))
            {
                layout = rotateString(layout);

                if (shiftCounter == 100)
                    return new Tuple<GameObject, int>(prefabs[0], 0);

                shiftCounter++;
            }

            GameObject prefab;
            int YRotation = shiftCounter * 60;

            int prefabIndex = Array.FindIndex(prefabsMap, e => e == layout);
            prefab = prefabs.ElementAt(prefabIndex);

            return new Tuple<GameObject, int>(prefab, YRotation);
        }

        private static string rotateString(string value)
        {
            return value.Substring(1, value.Length - 1) + value.Substring(0, 1);
        }
        private static Dictionary<CompassPoint, Tile> getAdjacentConnections(Tile tile, Landform landform = null)
        {
            return tile
                .Neighbours
                .Where(kpv => tile.Landform.Tiles.Contains(kpv.Value))
                .ToDictionary(x => x.Key, x => x.Value);
        }
        private static Dictionary<CompassPoint, Tile> getAdjacentLandforms(Tile tile)
        {
            // Reflection to avoid using "dynamic" keyword not working with target .net standard 2.0
            IEnumerable<Landform> connectedLandforms = tile.Landform.GetType().GetProperty("ConnectedLandforms").GetValue(tile.Landform, null) as IEnumerable<Landform>;
            HashSet<Coords> connectedLandformsLimits = connectedLandforms.SelectMany(l => l.LimitsCoords).ToHashSet();

            return tile
                .Neighbours
                .Where(kpv => connectedLandformsLimits.Contains(kpv.Value.Coords))
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}