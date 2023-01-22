namespace Graphs.Solvers
{
    internal static class BellmanFordSolver<T>
    {
        internal static double[] GetShortestDistances(Graph<T> graph, int from)
        {
            Vertex<T> fromVertex = graph.GetVertex(from);
            return BellmanFord(graph, fromVertex);
        }

        private static double[] BellmanFord(Graph<T> graph, Vertex<T> start)
        {
            double[] dist = new double[graph.VertexCount];
            Array.Fill(dist, double.PositiveInfinity);
            dist[start.Id] = 0;

            for (int i = 0; i < graph.VertexCount; i++)
            {
                var current = graph.ElementAt(i);

                Vertex<T> vertex = current.Key;
                List<Edge<T>> edges = current.Value;

                foreach (Edge<T> edge in edges)
                {
                    if (dist[vertex.Id] + edge.Cost < dist[edge.To.Id])
                    {
                        dist[edge.To.Id] = dist[vertex.Id] + edge.Cost;
                    }
                }
            }

            for (int i = 0; i < graph.VertexCount; i++)
            {
                var current = graph.ElementAt(i);

                Vertex<T> vertex = current.Key;
                List<Edge<T>> edges = current.Value;

                foreach (Edge<T> edge in edges)
                {
                    if (dist[vertex.Id] + edge.Cost < dist[edge.To.Id])
                    {
                        dist[edge.To.Id] = double.NegativeInfinity;
                    }
                }
            }

            return dist;
        }
    }
}
