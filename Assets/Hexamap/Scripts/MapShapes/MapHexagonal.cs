using System.Collections.Generic;
using System.Linq;

namespace Hexamap
{
    [MapShapeName("Hexagonal")]
    public class MapHexagonal : Map
    {
        private HashSet<Coords> _limits = new HashSet<Coords>();

        public MapHexagonal(SettingsMap settingsMap) : base(settingsMap) { }

        protected override IEnumerable<Coords> AllocateMap(int size)
        {
            // Start with a Coords at 0;0 then expand until expected size is reached
            var allocatedCoords = new List<Coords> { new Coords(0, 0) };

            while (allocatedCoords.Count < size)
            {
                IEnumerable<Coords> expansion = Coords.Expand(allocatedCoords, 1);

                if (expansion.Count() >= size)
                    _limits = expansion.Except(allocatedCoords).ToHashSet();

                allocatedCoords = expansion.ToList();
            }

            return allocatedCoords;
        }
        protected override IEnumerable<Coords> FindLimits() => _limits;
    }
}
