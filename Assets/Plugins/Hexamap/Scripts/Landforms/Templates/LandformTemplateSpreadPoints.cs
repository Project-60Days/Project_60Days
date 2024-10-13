using System;
using System.Collections.Generic;
using System.Linq;

namespace Hexamap
{
    public abstract class LandformTemplateSpreadPoints : LandformQuantity
    {
        protected LandformTemplateSpreadPoints(Biome biome, IEnumerable<Coords> allocatedCoords, int maxQuantity) : base(biome, allocatedCoords, maxQuantity, true)
        {

        }

        protected override IEnumerable<Coords> AllocateLandform()
        {
            List<Coords> landform = new List<Coords>();
            List<Coords> possibleCoords = AllocatedCoords.ToList();

            int minDistance = (int) Math.Ceiling(AllocatedCoords.Count * 0.01f);

            while (landform.Count < MaxQuantity && possibleCoords.Any())
            {
                Coords randCoords = possibleCoords[Map.RandomObject.Next(0, possibleCoords.Count)];
                possibleCoords.Remove(randCoords);

                bool isValid = true;
                foreach (Coords c in landform)
                {
                    if (randCoords.GetDistanceFromCoords(c) < minDistance)
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                    landform.Add(randCoords);
            }

            return landform;
        }
    }
}