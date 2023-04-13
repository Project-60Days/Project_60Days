using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hexamap
{
    [CreateAssetMenu(fileName = "HexamapSettings", menuName = "Hexamap/Map Settings", order = 1)]
    public class AssetsMap : ScriptableObject
    {
        public string ShapeAsString;
        public int ShapePopupIndex;
        public int Seed;
        public float Padding;

        public int WorldLimitsSize;
        public string WorldLimitsPrefabPickingMethodAsString;
        public int WorldLimitsPrefabPickingMethodPopupIndex;
        public OrientationMethod WorldLimitsPrefabOrientationMethod;
        public GameObject[] WorldLimitsPrefabs;

        public AssetsBiome[] BiomeSettings;

        public Type WorldLimitsPrefabPickingMethod => Type.GetType(WorldLimitsPrefabPickingMethodAsString);

        public static explicit operator SettingsMap(AssetsMap unity)
        {
            var settingsBiome = new List<SettingsBiome>();

            foreach (var s in unity.BiomeSettings)
                settingsBiome.Add((SettingsBiome)s);
            
            return new SettingsMap(unity.Seed, Type.GetType(unity.ShapeAsString), unity.WorldLimitsSize, settingsBiome);
        }
    }
}