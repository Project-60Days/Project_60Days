using System;
using System.Collections.Generic;
using System.Linq;

namespace Hexamap
{
    public static class AStar
    {
        private static Dictionary<Coords, Node> _allNodes = new Dictionary<Coords, Node>();

        public static List<Coords> FindPath(Section start, Section end, HashSet<Coords> available, HashSet<Coords> others, Func<int> randomCost = null)
        {
            HashSet<Coords> allAvailable = available
                .Concat(start.Tiles.Select(t => t.Coords))
                .Concat(end.Tiles.Select(t => t.Coords))
                .ToHashSet();

            Node nodeStart = new Node() { Coords = getCentroid(start) };
            Node nodeEnd = new Node() { Coords = getCentroid(end) };

            List<Node> path = new List<Node>();
            List<Node> openList = new List<Node>();
            List<Node> closedList = new List<Node>();

            Node nodeCurrent = nodeStart;

            openList.Add(nodeStart);

            while (openList.Count != 0)
            {
                nodeCurrent = openList[0];
                openList.Remove(nodeCurrent);
                closedList.Add(nodeCurrent);

                nodeCurrent.Neighbours = getNeighbourNodes(nodeCurrent).ToList();

                foreach (var n in nodeCurrent.Neighbours)
                {
                    if (!allAvailable.Contains(n.Coords))
                        continue;

                    var n1 = n;
                    if (!closedList.Exists(node => node.Coords.Equals(n1.Coords)))
                        if (!openList.Exists(node => node.Coords.Equals(n.Coords)))
                        {
                            n.Parent = nodeCurrent;
                            n.Cost = n.Parent.Cost + 10;
                            if (others.Contains(n.Coords))
                            {
                                n.Cost -= 8;
                            }
                            if (randomCost != null)
                                n.Cost += randomCost.Invoke();
                            n.Distance = Math.Abs(n.Coords.X - nodeEnd.Coords.X) + Math.Abs(n.Coords.Y - nodeEnd.Coords.Y);
                            openList.Add(n);
                            openList = openList.OrderBy(node => node.Heuristic).ToList();
                        }
                }

                if (closedList.Last().Coords.Equals(nodeEnd.Coords))
                    break;
            }


            if (!closedList.Last().Coords.Equals(nodeEnd.Coords))
                return null;

            Node tmp = closedList.First(n => n == nodeCurrent.Parent);
            while (tmp != nodeStart && tmp != null)
            {
                path.Add(tmp);
                tmp = tmp.Parent;
            }

            truncatePath(path, start);
            path.Reverse();
            truncatePath(path, end);

            return path.Select(n => n.Coords).Intersect(available).ToList();
        }

        // Find at which index an element of the path meet the section and discard every other element after this one
        private static void truncatePath(List<Node> path, Section section)
        {
            var frontiers = Coords.Expand(section.LimitsCoords.ToList(), 1);
            int firstContactIndex = path.FindIndex(n => frontiers.Contains(n.Coords));

            if (firstContactIndex != -1 && firstContactIndex != path.Count - 1)
                path.RemoveRange(firstContactIndex + 1, path.Count - firstContactIndex - 1);
        }

        private static List<Node> getNeighbourNodes(Node node)
        {
            var neighbourNodes = new List<Node>();

            foreach (Coords c in node.Coords.Neighbours)
            {
                Node neighbourNode;

                if (_allNodes.ContainsKey(c))
                {
                    neighbourNode = _allNodes[c];
                }
                else
                {
                    neighbourNode = new Node() {Coords = c};
                    _allNodes[c] = neighbourNode;
                }

                neighbourNodes.Add(neighbourNode);
            }

            return neighbourNodes;
        }

        private static Coords getCentroid(Section section)
        {
            int sumX = section.Tiles.Sum(t => t.Coords.X);
            int sumY = section.Tiles.Sum(t => t.Coords.Y);

            int avgX = (int)Math.Round((double)sumX / section.Tiles.Count());
            int avgY = (int)Math.Round((double)sumY / section.Tiles.Count());

            return new Coords(avgX, avgY);
        }

        private class Node
        {
            public Node Parent;
            public Coords Coords;
            public List<Node> Neighbours;
            public int Cost;
            public int Distance;
            public float Heuristic => Distance + Cost;
        }
    }
}
