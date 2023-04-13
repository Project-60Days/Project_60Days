using System.Collections.Generic;

namespace Hexamap
{
    [LandformName("World Limit's")]
    public class LandformWorldLimit : LandformFiller
    {
        public LandformWorldLimit(Biome biome, IEnumerable<Coords> allocatedCoords) : base(biome, allocatedCoords, false)
        {

        }
    }
}
