using Graphs.Solvers;
using System.Collections;
using System.Text;

namespace Graphs
{
    public class Graph<T> : IEnumerable<KeyValuePair<Vertex<T>, List<Edge<T>>>>
    {
        private Dictionary<Vertex<T>, List<Edge<T>>> graph = new();
        public int VertexCount { get => graph.Count; }

        public List<Edge<T>> this[Vertex<T> vertex]
        {
            get => graph[vertex];
            set => graph[vertex] = value;
        }

        public IEnumerator<KeyValuePair<Vertex<T>, List<Edge<T>>>> GetEnumerator() => graph.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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

        public KeyValuePair<Vertex<T>, List<Edge<T>>> ElementAt(int i) => graph.ElementAt(i);
        public Vertex<T> GetVertex(int id) => graph.ElementAt(id).Key;
        public Edge<T>[] GetEdges(int id) => graph.ElementAt(id).Value.ToArray();

        public Vertex<T>[] GetShortestPath(int from, int to, out double distance) 
            => DijkstraSolver<T>.GetShortestPath(this, from, to, out distance);

        public double[] GetShortestDistances(int from) 
            => BellmanFordSolver<T>.GetShortestDistances(this, from);

        public void Topsort() 
            => graph = TopsortSolver<T>.Topsort(this);

        public double[,] GetAllPairShortestDistances()
            => FloydWarshallSolver<T>.GetAllPairShortestDistances(this);

        public Vertex<T>[][]? GetAllPairShortestPaths(out double[,] distances)
            => FloydWarshallSolver<T>.GetAllPairShortestPaths(this, out distances);

        public (Vertex<T>, Vertex<T>)[] FindBridges()
            => FindBridgesSolver<T>.FindBridges(this);

        public Vertex<T>[] FindArticulationPoints()
            => FindArticulationPointsSolver<T>.FindArticulationPoints(this);

        public Vertex<T>[][] GetStronglyConnectedComponents()
            => TarjanSolver<T>.GetStronglyConnectedComponents(this);

        public Vertex<T>[]? FindEulerianPath()
            => EulerianPathSolver<T>.FindEulerianPath(this);

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
    }
}