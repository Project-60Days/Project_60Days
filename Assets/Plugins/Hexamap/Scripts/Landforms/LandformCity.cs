using System.Collections.Generic;

namespace Hexamap
{
    [LandformName("Cities")]
    public class LandformCity : LandformTemplateSpreadPoints
    {
        public LandformCity(Biome biome, IEnumerable<Coords> allocatedCoords, int maxQuantity) : base(biome, allocatedCoords, maxQuantity)
        {
        }
    }
}