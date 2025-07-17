using System;
using System.Collections.Generic;
using System.Linq;

namespace Hexamap
{
    // Gapotchenko Algorithm for stable topological sort.
    // Property of public domain.
    //
    // Oleksiy Gapotchenko, 2014
    //
    // History of changes:
    //   - August 2014: Initial version
    //   - August 2018: Fixed wording. "transient" changed to "transitive"
    //
    // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    // EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
    // MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
    // IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
    // OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
    // ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    // OTHER DEALINGS IN THE SOFTWARE.
    internal static class TopologicalSort
    {
        /// <summary>
        /// Delegate definition for dependency function.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="a">The A.</param>
        /// <param name="b">The B.</param>
        /// <returns>
        /// Returns <c>true</c> when A depends on B. Otherwise, <c>false</c>.
        /// </returns>
        public delegate bool TopologicalDependencyFunction<in T>(T a, T b);

        /// <summary>
        /// Sorts the elements of a sequence in dependency order according to comparison function with Gapotchenko algorithm.
        /// The sort is stable. Cyclic dependencies are tolerated and resolved according to original order of elements in sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="dependencyFunction">The dependency function.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The ordered sequence.</returns>
        public static IEnumerable<T> StableOrder<T>(
            IEnumerable<T> source,
            TopologicalDependencyFunction<T> dependencyFunction,
            IEqualityComparer<T> equalityComparer)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (dependencyFunction == null)
                throw new ArgumentNullException("dependencyFunction");
            if (equalityComparer == null)
                throw new ArgumentNullException("equalityComparer");

            var graph = DependencyGraph<T>.TryCreate(source, dependencyFunction, equalityComparer);
            if (graph == null)
                return source;

            var list = source.ToList();
            int n = list.Count;

            Restart:
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < i; ++j)
                {
                    if (graph.DoesXHaveDirectDependencyOnY(list[j], list[i]))
                    {
                        bool jOnI = graph.DoesXHaveTransitiveDependencyOnY(list[j], list[i]);
                        bool iOnJ = graph.DoesXHaveTransitiveDependencyOnY(list[i], list[j]);

                        bool circularDependency = jOnI && iOnJ;

                        if (!circularDependency)
                        {
                            var t = list[i];
                            list.RemoveAt(i);

                            list.Insert(j, t);
                            goto Restart;
                        }
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Sorts the elements of a sequence in dependency order according to comparison function with Gapotchenko algorithm.
        /// The sort is stable. Cyclic dependencies are tolerated and resolved according to original order of elements in sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="dependencyFunction">The dependency function.</param>
        /// <returns>The ordered sequence.</returns>
        public static IEnumerable<T> StableOrder<T>(
            IEnumerable<T> source,
            TopologicalDependencyFunction<T> dependencyFunction)
        {
            return StableOrder(source, dependencyFunction, EqualityComparer<T>.Default);
        }

        sealed class DependencyGraph<T>
        {
            private DependencyGraph()
            {
            }

            public IEqualityComparer<T> EqualityComparer
            {
                get;
                private set;
            }

            public sealed class Node
            {
                public int Position
                {
                    get;
                    set;
                }

                List<T> _Children = new List<T>();

                public IList<T> Children
                {
                    get
                    {
                        return _Children;
                    }
                }
            }

            public IDictionary<T, Node> Nodes
            {
                get;
                private set;
            }

            public static DependencyGraph<T> TryCreate(
                IEnumerable<T> source,
                TopologicalDependencyFunction<T> dependencyFunction,
                IEqualityComparer<T> equalityComparer)
            {
                var list = source as IList<T>;
                if (list == null)
                    list = source.ToArray();

                int n = list.Count;
                if (n < 2)
                    return null;

                var graph = new DependencyGraph<T>();
                graph.EqualityComparer = equalityComparer;
                graph.Nodes = new Dictionary<T, Node>(n, equalityComparer);

                bool hasDependencies = false;

                for (int position = 0; position < n; ++position)
                {
                    var element = list[position];

                    Node node;
                    if (!graph.Nodes.TryGetValue(element, out node))
                    {
                        node = new Node();
                        node.Position = position;
                        graph.Nodes.Add(element, node);
                    }

                    foreach (var anotherElement in list)
                    {
                        if (equalityComparer.Equals(element, anotherElement))
                            continue;

                        if (dependencyFunction(element, anotherElement))
                        {
                            node.Children.Add(anotherElement);
                            hasDependencies = true;
                        }
                    }
                }

                if (!hasDependencies)
                    return null;

                return graph;
            }

            public bool DoesXHaveDirectDependencyOnY(T x, T y)
            {
                Node node;
                if (Nodes.TryGetValue(x, out node))
                {
                    if (node.Children.Contains(y, EqualityComparer))
                        return true;
                }
                return false;
            }

            sealed class DependencyTraverser
            {
                public DependencyTraverser(DependencyGraph<T> graph)
                {
                    _Graph = graph;
                    _VisitedNodes = new HashSet<T>(graph.EqualityComparer);
                }

                DependencyGraph<T> _Graph;
                HashSet<T> _VisitedNodes;

                public bool DoesXHaveTransitiveDependencyOnY(T x, T y)
                {
                    if (!_VisitedNodes.Add(x))
                        return false;

                    Node node;
                    if (_Graph.Nodes.TryGetValue(x, out node))
                    {
                        if (node.Children.Contains(y, _Graph.EqualityComparer))
                            return true;

                        foreach (var i in node.Children)
                        {
                            if (DoesXHaveTransitiveDependencyOnY(i, y))
                                return true;
                        }
                    }

                    return false;
                }
            }

            public bool DoesXHaveTransitiveDependencyOnY(T x, T y)
            {
                var traverser = new DependencyTraverser(this);
                return traverser.DoesXHaveTransitiveDependencyOnY(x, y);
            }
        }
    }
}
