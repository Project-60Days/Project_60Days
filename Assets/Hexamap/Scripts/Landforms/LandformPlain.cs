using System.Collections.Generic;

namespace Hexamap
{
    [LandformName("Plains")]
    public class LandformPlain : LandformFiller
    {
        public LandformPlain(Biome biome, IEnumerable<Coords> allocatedCoords) : base(biome, allocatedCoords, false)
        {

        }
    }
}