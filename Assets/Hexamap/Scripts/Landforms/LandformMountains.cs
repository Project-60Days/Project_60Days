using System.Collections.Generic;

namespace Hexamap
{
    [LandformName("Mountains")]
    public class LandformMountains : LandformTemplateNoisePerlin<LandformMountains>
    {
        public LandformMountains(Biome biome, IEnumerable<Coords> allocatedCoords, int maxSize) : base(biome, allocatedCoords, maxSize)
        {
            MinSize = (int)(biome.Size * 0.0015);
            NoiseFrequency = 0.12f;
        }
    }
}