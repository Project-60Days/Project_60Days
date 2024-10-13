using System.Collections.Generic;

namespace Hexamap
{
    public abstract class LandformAdaptable : Landform
    {
        protected LandformAdaptable(Biome biome, IEnumerable<Coords> allocatedCoords, bool subdivide) : base(biome, allocatedCoords, subdivide)
        {

        }
    }
}