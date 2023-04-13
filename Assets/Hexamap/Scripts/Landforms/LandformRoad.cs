using System.Collections.Generic;

namespace Hexamap
{
    [LandformName("Roads")]
    [LandformDependency(typeof(LandformCity))]
    public class LandformRoad : LandformTemplateConnection<LandformCity>
    {
        public LandformRoad(Biome biome, IEnumerable<Coords> allocatedCoords) : base(biome, allocatedCoords)
        {
            RandomFunction = () => Map.RandomObject.Next(0, 5);
        }
    }
}