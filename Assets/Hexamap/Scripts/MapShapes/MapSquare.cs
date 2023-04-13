using System;
using System.Collections.Generic;

namespace Hexamap
{
    [MapShapeName("Square")]
    public class MapSquare : Map
    {
        private int _sidesLength;
        private int _half;
        private readonly HashSet<Coords> _limits = new HashSet<Coords>();

        public MapSquare(SettingsMap settingsMap) : base(settingsMap) { }

        protected override IEnumerable<Coords> AllocateMap(int size)
        {
            var allocatedCoords = new List<Coords>();
            _sidesLength = (int)Math.Sqrt(size);
            _half = (int) Math.Ceiling(_sidesLength / 2f);

            for (int i = -_half; i <= _half; i++)
            {
                for (int j = -_half; j <= _half; j++)
                    allocatedCoords.Add(new Coords(i, j));

                _limits.Add(new Coords(-_half, i));
                _limits.Add(new Coords(i, -_half));
                _limits.Add(new Coords(_half, i));
                _limits.Add(new Coords(i, _half));
            }

            return allocatedCoords;
        }
        protected override IEnumerable<Coords> AllocateWorldLimits()
        {
            var worldLimitsCoords = new List<Coords>();
            for (float i = -_half - WorldLimitSize; i <= _half + WorldLimitSize; i++)
            {
                for (float j = -_half - WorldLimitSize; j <= _half + WorldLimitSize; j++)
                {
                    Coords coords = new Coords((int)i, (int)j);

                    if ((i < -_half || i > _half) || (j < -_half || j > _half))
                        worldLimitsCoords.Add(coords);
                }
            }

            return worldLimitsCoords;
        }
        protected override IEnumerable<Coords> FindLimits() => _limits;
    }
}