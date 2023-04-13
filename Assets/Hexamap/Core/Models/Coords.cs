using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Hexamap
{
    public struct Coords : IEquatable<Coords>
    {
        private  static Dictionary<Coords, Coords[]> _neighboursCache = new Dictionary<Coords, Coords[]>();
        private static readonly Array _enumCompassPointValues = Enum.GetValues(typeof(CompassPoint));
        private Coords[] _neighbours;

        public int X { get; }
        public int Y { get; }
        // Lazy loading, some Coords will never use this property
        public IReadOnlyCollection<Coords> Neighbours => _neighbours ?? (_neighbours = getNeighbours());

        public Coords(int x, int y)
        {
            X = x;
            Y = y;
            _neighbours = null;
        }
        public List<Coords> GetCoordsInRange(int range)
        {
            var expanded = Expand(this, range).ToList();
            expanded.Remove(this);

            return expanded;
        }
        public bool IsNeighbourOf(Coords other)
        {
            foreach (var n in other.Neighbours)
                if (n.Equals(this))
                    return true;
            return false;
        }
        public bool Equals(Coords other)
        {
            if (X == other.X && Y == other.Y)
                return true;

            return false;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                return hash;
            }
        }
        public override string ToString()
        {
            // example : (-30;12)
            return $"({X:G};{Y:G})";
        }
        public int GetDistanceFromCoords(Coords other)
        {
            int dy = other.Y - Y, dx = other.X - X;

            if (Y == other.Y) return dx;
            if (X == other.X) return dy;

            if (X < other.X) return dy + dx - (int)Math.Ceiling(dy / 2.0);
            return dy + dx - (int)Math.Floor(dy / 2.0);
        }

        public static IEnumerable<Coords> Expand(IEnumerable<Coords> collection, int size)
        {
            IEnumerable<Coords> expanded = collection;

            // Grow the collection by selecting every neighbours of every Coords in the collection not already inside the collection
            for (int i = 0; i < size; i++)
            {
                IEnumerable<Coords> neighbours = expanded.SelectMany(c => c.Neighbours).ToList();
                expanded = expanded.Concat(neighbours).Distinct();
            }

            return expanded;
        }
        public static IEnumerable<Coords> Expand(Coords coords, int size)
        {
            return Expand(new List<Coords>() { coords }, size);
        }
        public static IEnumerable<Coords> Reduce(IEnumerable<Coords> collection, int size)
        {
            IEnumerable<Coords> reduced = collection;

            // Shrink the collection by removing tiles not having all their neighbours in the collection
            for (int i = 0; i < size; i++)
                reduced = reduced
                    .Where(c => c.Neighbours.All(reduced.Contains))
                    .ToList();

            return collection.Except(reduced).ToList();
        }
        public static IEnumerable<Coords> Reduce(Coords coords, int size)
        {
            return Reduce(new List<Coords>() { coords }, size);
        }
        // Given a collection, this method will return a collection of collection. Each collection is an isolated cluster of Coords.
        public static IEnumerable<IEnumerable<Coords>> FindClusters(HashSet<Coords> pool)
        {
            List<HashSet<Coords>> clusters = new List<HashSet<Coords>>();

            // This method use BSF algorithm to find clusters
            do
            {
                HashSet<Coords> left = pool.Except(clusters.SelectMany(c => c)).ToHashSet();
                Coords start = left.First();

                HashSet<Coords> cluster = BreadthFirstSearch.Search(start, left).ToHashSet();

                clusters.Add(cluster);
            } while (clusters.Sum(x => x.Count) < pool.Count()); // While some Coords are not related to any cluster

            return clusters;
        }
        public static IEnumerable<Coords> Fill(HashSet<Coords> limits, HashSet<Coords> allocated)
        {
            var possibleStarts = limits
                .SelectMany(l => l.Neighbours)
                .Intersect(allocated)
                .Except(limits)
                .ToList();

            if (!possibleStarts.Any())
                return limits;

            HashSet<Coords> discovered = limits.ToHashSet();
            while (possibleStarts.Any())
            {
                Coords start = possibleStarts.First();
                possibleStarts.Remove(start);

                HashSet<Coords> pool = allocated.Except(discovered).ToHashSet();

                if (pool.Any())
                    foreach (Coords c in BreadthFirstSearch.Search(start, pool, limits))
                        discovered.Add(c);
            }

            return discovered;
        }
        public static bool IsBorder(Coords coords, HashSet<Coords> pool)
        {
            foreach (Coords n in coords.Neighbours)
                if (!pool.Contains(n))
                    return true;
            return false;
        }
        
        private Coords[] getNeighbours()
        {
            Coords[] neighboursCoords;

            if (_neighboursCache.ContainsKey(this))
            {
                neighboursCoords = _neighboursCache[this];
            }
            else
            {
                neighboursCoords = new Coords[6];

                foreach (CompassPoint compass in _enumCompassPointValues)
                {
                    var compassCoords = getNeighbour(compass);
                    neighboursCoords[(int) compass] = compassCoords;
                }

                _neighboursCache[this] = neighboursCoords;
            }

            return neighboursCoords;
        }
        private Coords getNeighbour(CompassPoint compass)
        {
            int parity = Math.Abs(X % 2);

            switch (compass)
            {
                case CompassPoint.N:
                    return new Coords(X + 0, Y + 1);
                case CompassPoint.NE:
                    return new Coords(X + 1, Y + 1 - parity);
                case CompassPoint.SE:
                    return new Coords(X + 1, Y + 0 - parity);
                case CompassPoint.S:
                    return new Coords(X + 0, Y - 1);
                case CompassPoint.SW:
                    return new Coords(X - 1, Y + 0 - parity);
                case CompassPoint.NW:
                    return new Coords(X - 1, Y + 1 - parity);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}