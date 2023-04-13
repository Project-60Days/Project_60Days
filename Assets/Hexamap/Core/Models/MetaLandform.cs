using System.Collections.Generic;
using System.Linq;

namespace Hexamap
{
    public class MetaLandform : Section
    {
        private readonly List<Landform> _landforms = new List<Landform>();

        public override IReadOnlyList<Tile> Tiles => _landforms.SelectMany(b => b.Tiles).ToList();
        public override int Size => Tiles.Count;
        public IReadOnlyList<Landform> Landforms => _landforms;

        public void AddLandform(Landform landformToAdd)
        {
            _landforms.Add(landformToAdd);
            landformToAdd.MetaLandform = this;
        }
    }
}
