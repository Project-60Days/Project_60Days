using System.Collections.Generic;
using System.Linq;

namespace Hexamap
{
    public abstract class Landform : Section
    {
        private List<Tile> _tiles = new List<Tile>();
        private bool _isFiller;
        private readonly bool _subdivide;
        private List<Landform> _adjacentLandforms = new List<Landform>();

        public Map Map => Biome.Map;
        public Biome Biome { get; }
        public override IReadOnlyList<Tile> Tiles => _tiles;
        public override int Size => OccupedCoords.Count;
        public IReadOnlyList<Landform> AdjacentLandforms => _adjacentLandforms;
        public MetaLandform MetaLandform { get; set; }

        protected Landform(Biome biome, IEnumerable<Coords> allocatedCoords, bool subdivide)
        {
            _subdivide = subdivide;
            Allocate(allocatedCoords);
            Biome = biome;
        }
        protected abstract IEnumerable<Coords> AllocateLandform();
        protected int MinSize { get; set; } = 0;

        public void Generate(bool isFiller)
        {
            _isFiller = isFiller;
            Generate();
        }
        public override void Generate()
        {
            // If the landform is a filler, consider the coords to allocate are every allocated coords
            IEnumerable<Coords> allocated = _isFiller ? AllocatedCoords : AllocateLandform();

            if (allocated != null && allocated.Any())
            {
                foreach (Coords coords in allocated)
                    _tiles.Add(new Tile(this, coords));

                base.Generate();
            }
        }
        public IEnumerable<Landform> Subdivide()
        {
            // If the flag is false, cancel subdivision and just return the base landform
            if (!_subdivide)
                return new List<Landform>() { this };

            List<Landform> landforms = new List<Landform>();

            // BFS to find clusters
            var shapes = Coords.FindClusters(LimitsCoords.ToHashSet());

            // If the landform can't be subdvided
            if (shapes.Count() == 1)
                return new List<Landform>() { this };

            // Foreach clusters found with BFS, create a new landform from the base one and update its properties
            foreach (HashSet<Coords> shape in shapes)
            {
                Landform l = (Landform) MemberwiseClone();

                l.SetLimits(shape);

                // Find bounding box
                int maxY = shape.Max(c => c.Y);
                int maxX = shape.Max(c => c.X);
                int minY = shape.Min(c => c.Y);
                int minX = shape.Min(c => c.X);
                HashSet<Coords> boundingBox = OccupedCoords
                    .Where(c => minX <= c.X && c.X <= maxX && minY <= c.Y && c.Y <= maxY)
                    .ToHashSet();

                List<Tile> tiles = new List<Tile>();

                // Flood-Fill to extract the cluster&
                IEnumerable<Coords> filled = Coords.Fill(shape, boundingBox);

                // If the subdivided landform is bigger than the MinSize required by this template
                if (filled.Count() >= MinSize)
                {
                    foreach (Coords c in filled)
                    {
                        Tile t = Map.GetTileFromCoords(c);
                        t.Landform = l;
                        tiles.Add(t);
                    }

                    l._tiles = tiles;
                    landforms.Add(l);
                }
            }

            return landforms;
        }
        public void FindAdjacentLandforms()
        {
            _adjacentLandforms = new List<Landform>();
            foreach (Landform other in Map.Landforms)
            {
                if (isAdjacent(other))
                {
                    _adjacentLandforms.Add(other);
                    other._adjacentLandforms.Add(this);
                }
            }
        }

        private bool isAdjacent(Landform other)
        {
            // If they are the same landforms
            if (other == this)
                return false;

            // If the landform is already listed as adjacent 
            if (_adjacentLandforms.Contains(other) || other._adjacentLandforms.Contains(this))
                return true;

            // Find which landform is the smallest to gain a bit on performance below
            IEnumerable<Coords> smallest, largest;
            if (LimitsCoords.Count > other.LimitsCoords.Count)
            {
                smallest = other.LimitsCoords;
                largest = LimitsCoords;
            }
            else
            {
                smallest = LimitsCoords;
                largest = other.LimitsCoords;
            }

            // If a tile of the smallest landform is neighbour of any tile of the largest landform, they are adjacent
            return largest
                .Intersect(smallest.SelectMany(c => c.Neighbours))
                .Any();
        }
    }
}