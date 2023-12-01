using Hexamap;
using System.Collections.Generic;

[LandformName("Rocks")]
public class LandformRocks : LandformTemplateRandomPoints
{
    public LandformRocks(Biome biome, IEnumerable<Coords> allocatedCoords, int maxQuantity) 
        : base(biome, allocatedCoords, maxQuantity)
    {
    }
}