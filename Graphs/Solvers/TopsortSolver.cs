namespace Graphs.Solvers
{
    internal static class TopsortSolver<T>
    {
        internal static Dictionary<Vertex<T>, List<Edge<T>>> Topsort(Graph<T> graph)
        {
            int[] ordering = new int[graph.VertexCount];
            bool[] visited = new bool[graph.VertexCount];

            int i = graph.VertexCount - 1;
            for (int at = 0; at < graph.VertexCount; at++)
            {
                if (!visited[at])
                {
                    i = DepthFirstSearch(graph, i, visited, ordering, at);
                }
            }

            Dictionary<Vertex<T>, List<Edge<T>>> sortedGraph = new();
            foreach (int id in ordering)
            {
                var fromCurrentGraph = graph.ElementAt(id);
                sortedGraph.Add(fromCurrentGraph.Key, fromCurrentGraph.Value);
            }

            return sortedGraph;
        }

        private static int DepthFirstSearch(Graph<T> graph, int i, bool[] visited, int[] ordering, int at)
        {
            visited[at] = true;

            Vertex<T> current = graph.GetVertex(at);
            List<Edge<T>> edges = graph[current];

            foreach (Edge<T> edge in edges)
            {
                if (!visited[edge.To.Id])
                {
                    i = DepthFirstSearch(graph, i, visited, ordering, edge.To.Id);
                }
            }

            ordering[i] = at;
            return i - 1;
        }
    }
}
