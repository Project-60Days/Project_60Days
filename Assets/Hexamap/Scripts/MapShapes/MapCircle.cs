using Hexamap;
using System;
using System.Collections.Generic;

[MapShapeName("Circle")]
public class MapCircle : Map
{
    public MapCircle(SettingsMap settingsMap) : base(settingsMap)
    {
    }

    protected override IEnumerable<Coords> AllocateMap(int size)
    {
        var allocatedCoords = new List<Coords>();

        int radius = (int)Math.Ceiling(Math.Sqrt(size / Math.PI)) + 1;

        for (int y = -radius; y <= radius; y++)
            for (int x = -radius; x <= radius; x++)
                if (x * x + y * y <= radius * radius)
                    allocatedCoords.Add(new Coords(x, y));

        return allocatedCoords;
    }
}