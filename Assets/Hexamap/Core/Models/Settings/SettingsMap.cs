using System;
using System.Collections.Generic;

namespace Hexamap
{
    public class SettingsMap
    {
        public const string BiomeWorldLimitName = "__WORLDLIMITS__";
        public const string BiomeWorldLimitUUID = "0";

        public int Seed { get; }
        public Type Shape { get; }
        public int WorldLimitSize { get; }

        public IReadOnlyList<SettingsBiome> SettingsBiome { get; }

        public SettingsMap(int seed, Type shape, int worldLimitSize, List<SettingsBiome> settingsBiome)
        {
            Seed = seed;
            Shape = shape;
            WorldLimitSize = worldLimitSize;
            SettingsBiome = settingsBiome;

            // If no seed is specified, generate one randomly
            if (Seed == 0)
                Seed = new Random().Next(int.MinValue, int.MaxValue);
        }
    }
}