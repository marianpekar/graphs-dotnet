namespace Graphs.Solvers
{
    internal class FindBridgesSolver<T>
    {
        internal static (Vertex<T>, Vertex<T>)[] FindBridges(Graph<T> graph)
        {
            int id = 0;
            int[] low = new int[graph.VertexCount];
            int[] ids = new int[graph.VertexCount];
            bool[] visited = new bool[graph.VertexCount];

            List<(Vertex<T>, Vertex<T>)> bridges = new();

            for (int i = 0; i < graph.VertexCount; i++)
            {
                if (!visited[i])
                {
                    DepthFirstSearch(graph, i, -1, visited, low, ids, id, bridges);
                }
            }

            return bridges.ToArray();
        }

        private static void DepthFirstSearch(Graph<T> graph, int at, int parent, bool[] visited, int[] low, 
            int[] ids, int id, List<(Vertex<T>, Vertex<T>)> bridges)
        {
            visited[at] = true;
            low[at] = ids[at] = ++id;

            foreach (var edge in graph.GetEdges(at))
            {
                if (edge.To.Id == parent)
                    continue;

                if (!visited[edge.To.Id])
                {
                    DepthFirstSearch(graph, edge.To.Id, at, visited, low, ids, id, bridges);

                    low[at] = Math.Min(low[at], low[edge.To.Id]);
                    if (ids[at] < low[edge.To.Id])
                    {
                        bridges.Add((graph.GetVertex(at), graph.GetVertex(edge.To.Id)));
                    }
                }
                else
                {
                    low[at] = Math.Min(low[at], ids[edge.To.Id]);
                }
            }
        }
    }
}
