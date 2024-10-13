using System.Collections.Generic;

namespace Hexamap
{
    [LandformName("Forests")]
    public class LandformForest : LandformTemplateNoisePerlin<LandformForest>
    {
        public LandformForest(Biome biome, List<Coords> allocatedCoords, int maxSize) : base(biome, allocatedCoords, maxSize)
        {
            MinSize = (int)(biome.Size * 0.001);
            NoiseFrequency = 0.155f;
        }
    }
}