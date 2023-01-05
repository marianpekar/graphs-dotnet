using System.Text;

namespace Graphs
{
    public class Graph<T>
    {
        private Dictionary<Vertex<T>, List<Edge<T>>> graph = new();
        public int VertexCount { get => graph.Count; }

        public void AddEdge(int from, int to, double cost)
        {
            Vertex<T> fromVertex = GetVertex(from);
            Vertex<T> toVertex = GetVertex(to);

            graph[fromVertex].Add(new Edge<T>(toVertex, cost));
        }

        public void AddUndirectedEdge(int from, int to, double cost)
        {
            Vertex<T> fromVertex = GetVertex(from);
            Vertex<T> toVertex = GetVertex(to);

            graph[fromVertex].Add(new Edge<T>(toVertex, cost));
            graph[toVertex].Add(new Edge<T>(fromVertex, cost));
        }

        public int AddVertex(T value)
        {
            graph.Add(new Vertex<T>(VertexCount, value), new List<Edge<T>>());

            return VertexCount - 1;
        }

        public Vertex<T> GetVertex(int id)
        {
            return graph.ElementAt(id).Key;
        }

        public Edge<T>[] GetEdges(int id)
        {
            return graph.ElementAt(id).Value.ToArray();
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            foreach (var vertex in graph)
            {
                stringBuilder.Append(vertex.Key.Value + " -> ");
                foreach (var edge in vertex.Value)
                {
                    stringBuilder.Append($"[{edge.To.Value} ({edge.Cost})]");
                }
                stringBuilder.AppendLine();
            }
            return stringBuilder.ToString();
        }

        public Vertex<T>[] GetShortestPath(int from, int to, out double distance)
        {
            Vertex<T> fromVertex = GetVertex(from);
            Vertex<T> toVertex = GetVertex(to);

            distance = Dijkstra(fromVertex, toVertex, out Vertex<T>[] prev);

            List<Vertex<T>> path = new();
            for (Vertex<T> at = toVertex; at != null; at = prev[at.Id])
            {
                path.Add(at);
            }
            path.Reverse();

            return path.ToArray();
        }

        private double Dijkstra(Vertex<T> start, Vertex<T> end, out Vertex<T>[] prev)
        {
            double[] dist = new double[VertexCount];
            Array.Fill(dist, double.PositiveInfinity);
            dist[start.Id] = 0;

            PriorityQueue<Vertex<T>, double> priorityQueue = new();
            priorityQueue.Enqueue(start, 0);

            bool[] visited = new bool[VertexCount];
            prev = new Vertex<T>[VertexCount];

            while (priorityQueue.Count > 0)
            {
                priorityQueue.TryDequeue(out Vertex<T> current, out double currentCost);
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

        public double[] GetShortestDistances(int from)
        {
            Vertex<T> fromVertex = GetVertex(from);
            return BellmanFord(fromVertex);
        }

        private double[] BellmanFord(Vertex<T> start)
        {
            double[] dist = new double[VertexCount];
            Array.Fill(dist, double.PositiveInfinity);
            dist[start.Id] = 0;

            for (int i = 0; i < VertexCount; i++)
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

            for (int i = 0; i < VertexCount; i++)
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


        public void Topsort()
        {
            int[] ordering = new int[VertexCount];
            bool[] visited = new bool[VertexCount];

            int i = VertexCount - 1;
            for (int at = 0; at < VertexCount; at++)
            {
                if (!visited[at])
                {
                    i = DepthFirstSearchForTopsort(i, visited, ordering, at);
                }
            }

            Dictionary<Vertex<T>, List<Edge<T>>> sortedGraph = new();
            foreach (int id in ordering)
            {
                var fromCurrentGraph = graph.ElementAt(id);
                sortedGraph.Add(fromCurrentGraph.Key, fromCurrentGraph.Value);
            }

            graph = sortedGraph;
        }

        private int DepthFirstSearchForTopsort(int i, bool[] visited, int[] ordering, int at)
        {
            visited[at] = true;

            Vertex<T> current = GetVertex(at);
            List<Edge<T>> edges = graph[current];

            foreach (Edge<T> edge in edges)
            {
                if (!visited[edge.To.Id])
                {
                    i = DepthFirstSearchForTopsort(i, visited, ordering, edge.To.Id);
                }
            }

            ordering[i] = at;
            return i - 1;
        }

        public double[,] GetAllPairShortestDistances()
        {
            return FloydWarshall(out _);
        }

        public Vertex<T>[][] GetAllPairShortestPaths(out double[,] distances)
        {
            distances = FloydWarshall(out int[,] next);

            List<Vertex<T>[]> allPairShortestPaths = new();
            for (int i = 0; i < VertexCount; i++)
            {
                for (int j = 0; j < VertexCount; j++)
                {
                    if (i == j)
                        continue;

                    Vertex<T>[] shortestPaths = ReconstructShortestPathFrom(i, j, next, distances);

                    if (shortestPaths == null || shortestPaths.Length == 0)
                        continue;

                    allPairShortestPaths.Add(shortestPaths);
                }
            }

            return allPairShortestPaths.ToArray();
        }

        public Vertex<T>[] ReconstructShortestPathFrom(int start, int end, int[,] next, double[,] distances)
        {
            List<Vertex<T>> path = new List<Vertex<T>>();

            if (distances[start, end] == double.PositiveInfinity)
                return path.ToArray();

            for (int at = start; at != end; at = next[at, end])
            {
                if (at == -1)
                    return null;

                path.Add(GetVertex(at));
            }

            if (next[start, end] == -1)
                return null;

            path.Add(GetVertex(end));

            return path.ToArray();
        }

        private double[,] FloydWarshall(out int[,] next)
        {
            double[,] adjacencyMatrix = CreateAdjacencyMatrix();
            double[,] distances = (double[,])adjacencyMatrix.Clone();
            next = new int[VertexCount, VertexCount];

            for (int i = 0; i < VertexCount; i++)
            {
                for (int j = 0; j < VertexCount; j++)
                {
                    if (adjacencyMatrix[i, j] != double.PositiveInfinity)
                    {
                        next[i, j] = j;
                    }
                }
            }

            for (int k = 0; k < VertexCount; k++)
            {
                for (int i = 0; i < VertexCount; i++)
                {
                    for (int j = 0; j < VertexCount; j++)
                    {
                        if (distances[i, k] + distances[k, j] < distances[i, j])
                        {
                            distances[i, j] = distances[i, k] + distances[k, j];
                            next[i, j] = next[i, k];
                        }
                    }
                }
            }

            for (int k = 0; k < VertexCount; k++)
            {
                for (int i = 0; i < VertexCount; i++)
                {
                    for (int j = 0; j < VertexCount; j++)
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

        private double[,] CreateAdjacencyMatrix()
        {
            double[,] adjacencyMatrix = new double[VertexCount, VertexCount];

            for (int i = 0; i < VertexCount; i++)
            {
                for (int j = 0; j < VertexCount; j++)
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

        public (Vertex<T>, Vertex<T>)[] FindBridges()
        {
            int id = 0;
            int[] low = new int[VertexCount];
            int[] ids = new int[VertexCount];
            bool[] visited = new bool[VertexCount];

            List<(Vertex<T>, Vertex<T>)> bridges = new();

            for (int i = 0; i < VertexCount; i++)
            {
                if (!visited[i])
                {
                    DepthFirstSearchForBridges(i, -1, visited, low, ids, id, bridges);
                }
            }

            return bridges.ToArray();
        }

        private void DepthFirstSearchForBridges(int at, int parent, bool[] visited, int[] low, int[] ids, int id, List<(Vertex<T>, Vertex<T>)> bridges)
        {
            visited[at] = true;
            low[at] = ids[at] = ++id;

            foreach (var edge in GetEdges(at))
            {
                if (edge.To.Id == parent)
                    continue;

                if (!visited[edge.To.Id])
                {
                    DepthFirstSearchForBridges(edge.To.Id, at, visited, low, ids, id, bridges);

                    low[at] = Math.Min(low[at], low[edge.To.Id]);
                    if (ids[at] < low[edge.To.Id])
                    {
                        bridges.Add((GetVertex(at), GetVertex(edge.To.Id)));
                    }
                }
                else
                {
                    low[at] = Math.Min(low[at], ids[edge.To.Id]);
                }
            }
        }

        public Vertex<T>[] FindArticulationPoints()
        {
            int id = 0;
            int rootNodeOutcomingEdgeCount = 0;
            int[] low = new int[VertexCount];
            int[] ids = new int[VertexCount];
            bool[] visited = new bool[VertexCount];
            bool[] isArticulationPoint = new bool[VertexCount];

            for (int i = 0; i < VertexCount; i++)
            {
                if (!visited[i])
                {
                    rootNodeOutcomingEdgeCount = 0;
                    DepthFirstSearchForArticulationPoints(i, i, -1, ref rootNodeOutcomingEdgeCount, visited, low, ids, id, isArticulationPoint);
                    isArticulationPoint[i] = rootNodeOutcomingEdgeCount > 1;
                }
            }

            List<Vertex<T>> articulationPoints = new();
            for (int i = 0; i < isArticulationPoint.Length; i++)
            {
                if (!isArticulationPoint[i])
                    continue;

                articulationPoints.Add(GetVertex(i));
            }

            return articulationPoints.ToArray();
        }

        private void DepthFirstSearchForArticulationPoints(int root, int at, int parent, ref int rootNodeOutcomingEdgeCount, bool[] visited, int[] low, int[] ids, int id, bool[] isArticulationPoint)
        {
            if (parent == root)
            {
                rootNodeOutcomingEdgeCount++;
            }

            visited[at] = true;
            low[at] = ids[at] = id++;

            foreach (var edge in GetEdges(at))
            {
                if (edge.To.Id == parent)
                    continue;

                if (!visited[edge.To.Id])
                {
                    DepthFirstSearchForArticulationPoints(root, edge.To.Id, at, ref rootNodeOutcomingEdgeCount,
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