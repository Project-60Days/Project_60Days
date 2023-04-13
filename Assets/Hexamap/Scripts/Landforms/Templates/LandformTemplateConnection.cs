using System;
using System.Collections.Generic;
using System.Linq;

namespace Hexamap
{
    public abstract class LandformTemplateConnection<T> : LandformAdaptable where T : Landform
    {
        private static List<Biome> _generatedBiomes = new List<Biome>();

        private List<Landform> _connectedLandforms = new List<Landform>();

        protected Func<int> RandomFunction { get; set; }

        public IReadOnlyList<Landform> ConnectedLandforms => _connectedLandforms;

        protected LandformTemplateConnection(Biome biome, IEnumerable<Coords> allocatedCoords) : base(biome, allocatedCoords, true)
        {
            
        }

        protected override IEnumerable<Coords> AllocateLandform()
        {
            if (_generatedBiomes.Contains(Biome))
                return null;

            List<Landform> vertices = Biome.Landforms.Where(l => l.GetType() == typeof(T)).ToList();

            if (!vertices.Any())
                return null;

            // Prim's algorithm
            List<Edge> tree = new List<Edge>();
            List<Edge> edges = new List<Edge>();
            Dictionary<Landform, Landform> parents = new Dictionary<Landform, Landform>();
            Dictionary<Landform, int> keys = new Dictionary<Landform, int>();

            HashSet<Coords> availableCoords = AllocatedCoords.ToHashSet();

            // Initialize values
            foreach (Landform v in vertices)
            {
                parents[v] = null;
                keys[v] = int.MaxValue;
            }

            // Pick root of the tree
            Landform root = vertices.First();
            keys[root] = 0;

            // Q = unconnected vertices
            List<Landform> Q = vertices.ToList();

            while (Q.Any())
            {
                // Find the vertex in Q with the smallest key
                Landform u = Q.OrderBy(v => keys[v]).First();

                Q.Remove(u);

                // Build all edges from u to every not already connected vertices
                foreach (Landform l in Q)
                    if (l != u && findEdge(l, u, edges) == null)
                        edges.Add(new Edge(l, u, availableCoords, edges.Where(e => e.Success).SelectMany(e => e.Path).ToHashSet(), RandomFunction));

                if (parents[u] != null)
                {
                    // Find edge between u and its parent and add it to the tree
                    tree.Add(findEdge(u, parents[u], edges));
                }

                foreach (Landform v in Q)
                {
                    Edge e = findEdge(v, u, edges);
                    if (e != null && e.Weight < keys[v])
                    {
                        parents[v] = u;
                        keys[v] = e.Weight;
                    }
                }
            }

            _generatedBiomes.Add(Biome);

            _connectedLandforms = tree.Select(e => e.Vertex1).Concat(tree.Select(e => e.Vertex2)).Distinct().ToList();

            return tree.SelectMany(e => e.Path).Distinct();
        }

        private Edge findEdge(Landform vertex1, Landform vertex2, List<Edge> edges)
        {
            return edges.FirstOrDefault(e => e.Success && (e.Vertex1 == vertex1 && e.Vertex2 == vertex2) || (e.Vertex1 == vertex2 && e.Vertex2 == vertex1));
        }

        private class Edge
        {
            private List<Coords> _path;

            public Landform Vertex1 { get; }
            public Landform Vertex2 { get; }
            public int Weight => _path.Count;
            public IReadOnlyList<Coords> Path => _path;
            public bool Success { get; }

            public Edge(Landform vertex1, Landform vertex2, HashSet<Coords> available, HashSet<Coords> others, Func<int> _randomFunction)
            {
                Vertex1 = vertex1;
                Vertex2 = vertex2;

                _path = AStar.FindPath(vertex1, vertex2, available, others, _randomFunction);

                Success = _path != null && _path.Any();
            }
        }
    }
}