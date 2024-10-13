using System.Collections.Generic;
using System.Linq;

namespace Hexamap
{
    public abstract class LandformTemplateRandomPoints : LandformQuantity
    {
        protected LandformTemplateRandomPoints(Biome biome, IEnumerable<Coords> allocatedCoords, int maxQuantity) : base(biome, allocatedCoords, maxQuantity, true)
        {
            
        }

        protected override IEnumerable<Coords> AllocateLandform()
        {
            HashSet<Coords> landform = new HashSet<Coords>();

            while (landform.Count < MaxQuantity)
            {
                Coords point = AllocatedCoords.ElementAt(Map.RandomObject.Next(0, AllocatedCoords.Count));

                if (!landform.Contains(point))
                    landform.Add(point);
            }

            return landform;
        }
    }
}
