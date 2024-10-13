using Hexamap;
using System.Collections.Generic;
using System.Linq;

[MapShapeName("Random")]
public class MapRandom : Map
{
    public MapRandom(SettingsMap settingsMap) : base(settingsMap) { }

    protected override IEnumerable<Coords> AllocateMap(int size)
    {
        var allocatedCoords = new HashSet<Coords>();
        allocatedCoords.Add(new Coords(0, 0));

        while (allocatedCoords.Count < size)
        {
            Coords origin = allocatedCoords.ElementAt(RandomObject.Next(0, allocatedCoords.Count()));

            var toAdd = Coords.Expand(origin, 1);

            foreach (Coords c in toAdd)
                if (!allocatedCoords.Contains(c))
                    allocatedCoords.Add(c);
        }

        return allocatedCoords;
    }
}
