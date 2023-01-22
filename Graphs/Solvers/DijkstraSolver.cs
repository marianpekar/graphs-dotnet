namespace Graphs.Solvers
{
    internal static class DijkstraSolver<T>
    {
        internal static Vertex<T>[] GetShortestPath(Graph<T> graph, int from, int to, out double distance)
        {
            Vertex<T> fromVertex = graph.GetVertex(from);
            Vertex<T> toVertex = graph.GetVertex(to);

            distance = Dijkstra(graph, fromVertex, toVertex, out Vertex<T>[] prev);

            List<Vertex<T>> path = new();
            for (Vertex<T> at = toVertex; at != null; at = prev[at.Id])
            {
                path.Add(at);
            }
            path.Reverse();

            return path.ToArray();
        }

        private static double Dijkstra(Graph<T> graph, Vertex<T> start, Vertex<T> end, out Vertex<T>[] prev)
        {
            double[] dist = new double[graph.VertexCount];
            Array.Fill(dist, double.PositiveInfinity);
            dist[start.Id] = 0;

            PriorityQueue<Vertex<T>, double> priorityQueue = new();
            priorityQueue.Enqueue(start, 0);

            bool[] visited = new bool[graph.VertexCount];
            prev = new Vertex<T>[graph.VertexCount];

            while (priorityQueue.Count > 0)
            {
                priorityQueue.TryDequeue(out Vertex<T>? current, out double currentCost);

                if (current == null)
                    throw new ArgumentNullException(nameof(current));
                
                visited[current.Id] = true;
                
                if (dist[current.Id] < currentCost)
                    continue;

                List<Edge<T>> edges = graph[current];
                foreach (Edge<T> edge in edges)
                {
                    if (visited[edge.To.Id])
                        continue;

                    double newDist = dist[current.Id] + edge.Cost;
                    if (newDist < dist[edge.To.Id])
                    {
                        prev[edge.To.Id] = current;
                        dist[edge.To.Id] = newDist;
                        priorityQueue.Enqueue(edge.To, newDist);
                    }
                }
                if (current.Id == end.Id)
                    return dist[end.Id];
            }

            return double.PositiveInfinity;
        }
    }
}
