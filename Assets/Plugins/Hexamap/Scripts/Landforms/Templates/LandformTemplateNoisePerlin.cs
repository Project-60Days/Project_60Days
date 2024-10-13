using System.Collections.Generic;

namespace Hexamap
{
    public class LandformTemplateNoisePerlin<T> : LandformTemplateNoise<T> where T : Landform
    {
        public LandformTemplateNoisePerlin(Biome biome, IEnumerable<Coords> allocatedCoords, int maxSize) : base(biome, allocatedCoords, FastNoise.NoiseType.Perlin, maxSize)
        {

        }
    }
}