using System.Collections.Generic;

namespace Hexamap
{
    public class LandformTemplateNoiseCubic<T> : LandformTemplateNoise<T> where T : Landform
    {
        public LandformTemplateNoiseCubic(Biome biome, IEnumerable<Coords> allocatedCoords, int maxSize) : base(biome, allocatedCoords, FastNoise.NoiseType.Cubic, maxSize)
        {

        }
    }
}