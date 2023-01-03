namespace Graphs
{
    public class Vertex<T>
    {
        public int Id { get; private set; }
        public T Value { get; private set; }

        public Vertex(int id, T value)
        {
            Id = id;
            Value = value;
        }
    }
}
