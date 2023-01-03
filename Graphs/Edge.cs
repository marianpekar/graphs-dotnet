namespace Graphs
{
    public class Edge<T>
    {
        public Vertex<T> To { get; private set; }
        public double Cost { get; private set; }

        public Edge(Vertex<T> to, double cost)
        {
            To = to;
            Cost = cost;
        }
    }
}
