using System.Collections.Generic;
using System.Linq;

namespace Hexamap
{
    public abstract class Section
    {
        private readonly HashSet<Coords> _allocatedCoords = new HashSet<Coords>();
        private HashSet<Coords> _limitsCoords;

        protected IReadOnlyCollection<Coords> AllocatedCoords => _allocatedCoords;
        protected IReadOnlyList<Coords> OccupedCoords => Tiles.Select(t => t.Coords).ToList();
        protected IReadOnlyList<Coords> AvailableCoords => AllocatedCoords.Except(OccupedCoords).ToList();

        public abstract IReadOnlyList<Tile> Tiles { get; }
        public abstract int Size { get; }
        public IReadOnlyCollection<Coords> LimitsCoords => _limitsCoords;

        public virtual void Generate()
        {
            _limitsCoords = FindLimits().ToHashSet();
        }

        protected void Allocate(IEnumerable<Coords> coordsToAdd)
        {
            foreach (Coords c in coordsToAdd)
            {
                Allocate(c);
            }
        }
        protected void Allocate(Coords coordsToAdd)
        {
            _allocatedCoords.Add(coordsToAdd);
        }
        protected void SetLimits(IEnumerable<Coords> limits)
        {
            _limitsCoords = limits.ToHashSet();
        }
        protected virtual IEnumerable<Coords> FindLimits()
        {
            var limits = new HashSet<Coords>();

            if (!OccupedCoords.Any())
                return limits;

            var occuped = OccupedCoords.ToHashSet();
            var toCheck = occuped.OrderByDescending(c => c.Y).ToHashSet();

            while (toCheck.Count != 0)
            {
                Coords start = toCheck.First();
                Coords current = start;

                toCheck.Remove(current);

                if (Coords.IsBorder(current, occuped))
                {
                    var subLimits = new HashSet<Coords> { current };
                    findLimitsInNeighbours(current, subLimits, toCheck, occuped);

                    foreach (Coords c in subLimits)
                        limits.Add(c);
                }
            }

            return limits;
        }

        private void findLimitsInNeighbours(Coords frontier, HashSet<Coords> subLimits, HashSet<Coords> toCheck, HashSet<Coords> occuped)
        {
            var neighbours = frontier.Neighbours;

            foreach (Coords n in neighbours)
            {
                if (toCheck.Contains(n))
                {
                    toCheck.Remove(n);

                    if (Coords.IsBorder(n, occuped))
                    {
                        subLimits.Add(n);
                        findLimitsInNeighbours(n, subLimits, toCheck, occuped);
                    }
                }
            }
        }
    }
}