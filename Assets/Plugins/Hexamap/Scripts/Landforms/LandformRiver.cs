using System.Collections.Generic;

namespace Hexamap
{
    [LandformName("Rivers")]
    [LandformDependency(typeof(LandformLake))]
    public class LandformRiver : LandformTemplateConnection<LandformLake>
    {
        public LandformRiver(Biome biome, IEnumerable<Coords> allocatedCoords) : base(biome, allocatedCoords)
        {
            RandomFunction = () => Map.RandomObject.Next(0, 100);
        }
    }
}