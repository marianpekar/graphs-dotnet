namespace Graphs.Solvers
{
    internal static class EulerianPathSolver<T>
    {
        internal static Vertex<T>[]? FindEulerianPath(Graph<T> graph)
        {
            (int[] inEdges, int[] outEdges) = CountInAndOutEdges(graph, out int edgeCount);

            if (!HasEulerianPath(graph, inEdges, outEdges, edgeCount))
                return null;

            int startVertex = FindStartVertex(graph, inEdges, outEdges);
            List<Vertex<T>> path = new List<Vertex<T>>();
            DepthFirstSearch(graph, startVertex, outEdges, path);

            if (path.Count != edgeCount + 1)
                return null;

            path.Reverse();

            return path.ToArray();
        }

        private static (int[], int[]) CountInAndOutEdges(Graph<T> graph, out int total)
        {
            int[] inEdges = new int[graph.VertexCount];
            int[] outEdges = new int[graph.VertexCount];

            total = 0;

            for (int from = 0; from < graph.VertexCount; from++)
            {
                foreach (Edge<T> edge in graph.GetEdges(from))
                {
                    inEdges[edge.To.Id]++;
                    outEdges[from]++;
                    total++;
                }
            }

            return (inEdges, outEdges);
        }

        private static bool HasEulerianPath(Graph<T> graph, int[] inEdges, int[] outEdges, int edgeCount)
        {
            if (edgeCount == 0)
                return false;

            int startNodes = 0;
            int endNodes = 0;

            for (int i = 0; i < graph.VertexCount; i++)
            {
                if (outEdges[i] - inEdges[i] > 1 || inEdges[i] - outEdges[i] > 1)
                {
                    return false;
                }
                else if (outEdges[i] - inEdges[i] == 1)
                {
                    startNodes++;
                }
                else if (inEdges[i] - outEdges[i] == 1)
                {
                    endNodes++;
                }
            }

            return (endNodes == 0 && startNodes == 0) || (endNodes == 1 && startNodes == 1);
        }

        private static int FindStartVertex(Graph<T> graph, int[] inEdges, int[] outEdges)
        {
            int start = 0;

            for (int i = 0; i < graph.VertexCount; i++)
            {
                if (outEdges[i] - inEdges[i] == 1)
                    return i;

                if (outEdges[i] > 0)
                {
                    start = i;
                }
            }

            return start;
        }

        private static void DepthFirstSearch(Graph<T> graph, int at, int[] outEdges, List<Vertex<T>> path)
        {
            while (outEdges[at] != 0)
            {
                int next = graph.GetEdges(at)[--outEdges[at]].To.Id;
                DepthFirstSearch(graph, next, outEdges, path);
            }
            path.Add(graph.GetVertex(at));
        }
    }
}
