using System.Collections.Generic;

namespace Hexamap
{
    public abstract class LandformDistribution : Landform
    {
        protected int MaxSize { get; }

        protected LandformDistribution(Biome biome, IEnumerable<Coords> allocatedCoords, int maxSize, bool subdivide) : base(biome, allocatedCoords, subdivide)
        {
            MaxSize = maxSize;
        }
    }
}
