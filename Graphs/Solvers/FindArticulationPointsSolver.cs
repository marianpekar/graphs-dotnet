namespace Graphs.Solvers
{
    internal static class FindArticulationPointsSolver<T>
    {
        internal static Vertex<T>[] FindArticulationPoints(Graph<T> graph)
        {
            int id = 0;
            int rootNodeOutcomingEdgeCount = 0;
            int[] low = new int[graph.VertexCount];
            int[] ids = new int[graph.VertexCount];
            bool[] visited = new bool[graph.VertexCount];
            bool[] isArticulationPoint = new bool[graph.VertexCount];

            for (int i = 0; i < graph.VertexCount; i++)
            {
                if (!visited[i])
                {
                    rootNodeOutcomingEdgeCount = 0;
                    DepthFirstSearch(graph, i, i, -1, ref rootNodeOutcomingEdgeCount, 
                        visited, low, ids, id, isArticulationPoint);

                    isArticulationPoint[i] = rootNodeOutcomingEdgeCount > 1;
                }
            }

            List<Vertex<T>> articulationPoints = new();
            for (int i = 0; i < isArticulationPoint.Length; i++)
            {
                if (!isArticulationPoint[i])
                    continue;

                articulationPoints.Add(graph.GetVertex(i));
            }

            return articulationPoints.ToArray();
        }

        private static void DepthFirstSearch(Graph<T> graph, int root, int at, int parent, ref int rootNodeOutcomingEdgeCount, 
            bool[] visited, int[] low, int[] ids, int id, bool[] isArticulationPoint)
        {
            if (parent == root)
            {
                rootNodeOutcomingEdgeCount++;
            }

            visited[at] = true;
            low[at] = ids[at] = id++;

            foreach (var edge in graph.GetEdges(at))
            {
                if (edge.To.Id == parent)
                    continue;

                if (!visited[edge.To.Id])
                {
                    DepthFirstSearch(graph, root, edge.To.Id, at, ref rootNodeOutcomingEdgeCount,
                        visited, low, ids, id, isArticulationPoint);
                    if (ids[at] <= low[edge.To.Id])
                    {
                        isArticulationPoint[at] = true;
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
