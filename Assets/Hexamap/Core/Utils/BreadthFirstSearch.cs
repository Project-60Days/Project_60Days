using System.Collections.Generic;
using System.Linq;

namespace Hexamap
{
    public static class BreadthFirstSearch
    {
        public static IEnumerable<Coords> Search(Coords start, HashSet<Coords> pool, HashSet<Coords> bounds = null)
        {
            if (bounds == null)
                bounds = new HashSet<Coords>();

            HashSet<Coords> discovered = new HashSet<Coords>() { start };
            Queue<Coords> toExplore = new Queue<Coords>();

            toExplore.Enqueue(start);

            while (toExplore.Any())
            {
                Coords current = toExplore.Dequeue();
                IEnumerable<Coords> neighbours = current
                    .Neighbours
                    .Intersect(pool)
                    .Except(bounds)
                    .Except(discovered);

                foreach (Coords n in neighbours)
                {
                    discovered.Add(n);
                    toExplore.Enqueue(n);
                }
            }

            return discovered;
        }
    }
}
