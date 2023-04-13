using System.Collections.Generic;

namespace Hexamap
{
    public abstract class LandformQuantity : Landform
    {
        protected int MaxQuantity { get; }

        protected LandformQuantity(Biome biome, IEnumerable<Coords> allocatedCoords, int maxQuantity, bool subdivide) : base(biome, allocatedCoords, subdivide)
        {
            MaxQuantity = maxQuantity;
        }
    }
}