using System.Collections.Generic;

namespace Hexamap
{
    public abstract class LandformFiller : Landform
    {
        protected LandformFiller(Biome biome, IEnumerable<Coords> allocatedCoords, bool subdivide) : base(biome, allocatedCoords, subdivide)
        {
            
        }

        protected override IEnumerable<Coords> AllocateLandform()
        {
            return AllocatedCoords;
        }
    }
}