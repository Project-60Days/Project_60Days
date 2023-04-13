using System;
using System.Collections.Generic;
using System.Linq;

namespace Hexamap
{
    public class Tile
    {
        private static readonly Array _enumCompassPointValues = Enum.GetValues(typeof(CompassPoint));
        private readonly Dictionary<CompassPoint, Tile> _neighbours = new Dictionary<CompassPoint, Tile>();

        public Coords Coords { get; }
        public object GameEntity { get; set; }
        public IReadOnlyDictionary<CompassPoint, Tile> Neighbours => _neighbours;
        public Map Map => Biome.Map;
        public Biome Biome => Landform.Biome;
        public Landform Landform { get; set;  }

        public Tile(Landform landform, Coords coords)
        {
            Coords = coords;
            Landform = landform;
            Map.RegisterTile(coords, this);
            findNeighbours();
        }

        public void SetNeighbour(CompassPoint compass, Tile neighbour)
        {
            _neighbours[compass] = neighbour;
        }


        private void findNeighbours()
        {
            foreach (CompassPoint compass in _enumCompassPointValues)
            {
                Tile neighbour = Map.GetTileFromCoords(Coords.Neighbours.ElementAt((int)compass));

                if (neighbour != null)
                {
                    SetNeighbour(compass, neighbour);
                    CompassPoint compassOpposite = (CompassPoint)(((int)compass + 3) % 6);
                    neighbour.SetNeighbour(compassOpposite, this);
                }
            }
        }
    }
}
