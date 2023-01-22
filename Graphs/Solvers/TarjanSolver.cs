namespace Graphs.Solvers
{
    internal static class TarjanSolver<T>
    {
        internal static Vertex<T>[][] GetStronglyConnectedComponents(Graph<T> graph)
        {
            int[] sccs = Tarjan(graph);

            Dictionary<int, List<Vertex<T>>> vertGroupedByLowlinks = GetVerticesGroupedByLowlinks(graph, sccs);

            Vertex<T>[][] groups = new Vertex<T>[vertGroupedByLowlinks.Count][];
            int j = 0;
            foreach (var group in vertGroupedByLowlinks)
            {
                groups[j] = vertGroupedByLowlinks[j].ToArray();
                j++;
            }

            return groups;
        }

        private static Dictionary<int, List<Vertex<T>>> GetVerticesGroupedByLowlinks(Graph<T> graph, int[] sccs)
        {
            Dictionary<int, List<Vertex<T>>> groupedByLowLinks = new();
            for (int i = 0; i < sccs.Length; i++)
            {
                int scc = sccs[i];

                if (!groupedByLowLinks.ContainsKey(scc))
                {
                    groupedByLowLinks.Add(scc, new List<Vertex<T>>());
                    groupedByLowLinks[scc].Add(graph.GetVertex(i));
                }
                else
                {
                    groupedByLowLinks[scc].Add(graph.GetVertex(i));
                }
            }

            return groupedByLowLinks;
        }

        private const int kUnvisited = -1;
        private static int[] Tarjan(Graph<T> graph)
        {
            int id = 0;
            int sccsCount = 0;

            int[] ids = new int[graph.VertexCount];
            Array.Fill(ids, kUnvisited);

            int[] low = new int[graph.VertexCount];
            int[] sccs = new int[graph.VertexCount];
            bool[] visited = new bool[graph.VertexCount];

            Stack<int> stack = new();

            for (int i = 0; i < graph.VertexCount; i++)
            {
                if (ids[i] == kUnvisited)
                {
                    DepthFirstSearch(graph, i, visited, ids, id, low, stack, sccs, ref sccsCount);
                }
            }

            return sccs;
        }

        private static void DepthFirstSearch(Graph<T> graph, int at, bool[] visited, int[] ids, int id, 
            int[] low, Stack<int> stack, int[] sccs, ref int sccsCount)
        {
            ids[at] = low[at] = id++;
            stack.Push(at);
            visited[at] = true;

            foreach (var edge in graph.GetEdges(at))
            {
                if (ids[edge.To.Id] == kUnvisited)
                {
                    DepthFirstSearch(graph, edge.To.Id, visited, ids, id, low, stack, sccs, ref sccsCount);
                }

                if (visited[edge.To.Id])
                {
                    low[at] = Math.Min(low[at], low[edge.To.Id]);
                }
            }

            if (ids[at] == low[at])
            {
                for (int vertex = stack.Pop(); ; vertex = stack.Pop())
                {
                    visited[vertex] = false;
                    sccs[vertex] = sccsCount;

                    if (vertex == at)
                        break;
                }
                sccsCount++;
            }
        }
    }
}
