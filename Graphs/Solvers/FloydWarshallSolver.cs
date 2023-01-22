namespace Graphs.Solvers
{
    internal static class FloydWarshallSolver<T>
    {
        internal static double[,] GetAllPairShortestDistances(Graph<T> graph)
        {
            return FloydWarshallSolver<T>.FloydWarshall(graph, out _);
        }

        internal static Vertex<T>[][]? GetAllPairShortestPaths(Graph<T> graph, out double[,] distances)
        {
            distances = FloydWarshall(graph, out int[,] next);

            List<Vertex<T>[]> allPairShortestPaths = new();
            for (int i = 0; i < graph.VertexCount; i++)
            {
                for (int j = 0; j < graph.VertexCount; j++)
                {
                    if (i == j)
                        continue;

                    Vertex<T>[]? shortestPaths = ReconstructShortestPathFrom(graph, i, j, next, distances);

                    if (shortestPaths == null || shortestPaths.Length == 0)
                        continue;

                    allPairShortestPaths.Add(shortestPaths);
                }
            }

            return allPairShortestPaths.ToArray();
        }

        private static Vertex<T>[]? ReconstructShortestPathFrom(Graph<T> graph, int start, int end, int[,] next, double[,] distances)
        {
            List<Vertex<T>> path = new List<Vertex<T>>();

            if (distances[start, end] == double.PositiveInfinity)
                return path.ToArray();

            for (int at = start; at != end; at = next[at, end])
            {
                if (at == -1)
                    return null;

                path.Add(graph.GetVertex(at));
            }

            if (next[start, end] == -1)
                return null;

            path.Add(graph.GetVertex(end));

            return path.ToArray();
        }

        private static double[,] FloydWarshall(Graph<T> graph, out int[,] next)
        {
            double[,] adjacencyMatrix = CreateAdjacencyMatrix(graph);
            double[,] distances = (double[,])adjacencyMatrix.Clone();
            next = new int[graph.VertexCount, graph.VertexCount];

            for (int i = 0; i < graph.VertexCount; i++)
            {
                for (int j = 0; j < graph.VertexCount; j++)
                {
                    if (adjacencyMatrix[i, j] != double.PositiveInfinity)
                    {
                        next[i, j] = j;
                    }
                }
            }

            for (int k = 0; k < graph.VertexCount; k++)
            {
                for (int i = 0; i < graph.VertexCount; i++)
                {
                    for (int j = 0; j < graph.VertexCount; j++)
                    {
                        if (distances[i, k] + distances[k, j] < distances[i, j])
                        {
                            distances[i, j] = distances[i, k] + distances[k, j];
                            next[i, j] = next[i, k];
                        }
                    }
                }
            }

            for (int k = 0; k < graph.VertexCount; k++)
            {
                for (int i = 0; i < graph.VertexCount; i++)
                {
                    for (int j = 0; j < graph.VertexCount; j++)
                    {
                        if (distances[i, k] != double.PositiveInfinity &&
                            distances[k, j] != double.PositiveInfinity &&
                            distances[k, k] < 0)
                        {
                            distances[i, j] = double.NegativeInfinity;
                            next[i, k] = -1;
                        }

                    }
                }
            }

            return distances;
        }

        private static double[,] CreateAdjacencyMatrix(Graph<T> graph)
        {
            double[,] adjacencyMatrix = new double[graph.VertexCount, graph.VertexCount];

            for (int i = 0; i < graph.VertexCount; i++)
            {
                for (int j = 0; j < graph.VertexCount; j++)
                {
                    adjacencyMatrix[i, j] = i == j ? 0 : double.PositiveInfinity;
                }
            }

            foreach (var item in graph)
            {
                Vertex<T> vertex = item.Key;
                List<Edge<T>> edges = item.Value;

                foreach (Edge<T> edge in edges)
                {
                    adjacencyMatrix[vertex.Id, edge.To.Id] = edge.Cost;
                }
            }

            return adjacencyMatrix;
        }
    }
}
