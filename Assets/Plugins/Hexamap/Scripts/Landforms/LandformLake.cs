using System;
using System.Collections.Generic;
using System.Linq;

namespace Hexamap
{
    [LandformName("Lakes")]
    public class LandformLake : LandformTemplateNoiseCubic<LandformLake>
    {
        public LandformLake(Biome biome, IEnumerable<Coords> allocatedCoords, int maxSize) : base(biome, allocatedCoords, maxSize)
        {
            MinSize = (int) (biome.Size * 0.01);
            NoiseFrequency = 0.07f;
        }
    }
}