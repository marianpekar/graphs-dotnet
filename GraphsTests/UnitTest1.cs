using Graphs;

namespace GraphsTests
{
    public class Tests
    {
        /*
            A -> [B(1)][C(3)]
            B -> [D(2)][E(8)][C(1)]
            C -> [D(1)][E(3)]
            D -> [E(4)]
            E -> [C(3)]
        */
        private Graph<string> CreateGraph()
        {
            Graph<string> graph = new();

            int a = graph.AddVertex("A");
            int b = graph.AddVertex("B");
            int c = graph.AddVertex("C");
            int d = graph.AddVertex("D");
            int e = graph.AddVertex("E");

            graph.AddEdge(a, b, 1);
            graph.AddEdge(a, c, 3);

            graph.AddEdge(b, d, 2);
            graph.AddEdge(b, e, 8);
            graph.AddEdge(b, c, 1);

            graph.AddEdge(c, d, 1);
            graph.AddEdge(d, e, 4);

            graph.AddUndirectedEdge(e, c, 3);

            return graph;
        }

        /* 
            A -> [B (1)][C (1)]
            B -> [D (4)]
            C -> [B (1)]
            D -> [C (-6)][E (1)][F (1)]
            E ->
            F ->
        */
        private Graph<string> CreateGraphWithNegativeCycles()
        {
            Graph<string> graph = new();

            int a = graph.AddVertex("A");
            int b = graph.AddVertex("B");
            int c = graph.AddVertex("C");
            int d = graph.AddVertex("D");
            int e = graph.AddVertex("E");
            int f = graph.AddVertex("F");

            graph.AddEdge(a, b, 1);
            graph.AddEdge(a, c, 1);

            graph.AddEdge(b, d, 4);

            graph.AddEdge(c, b, 1);

            graph.AddEdge(d, c, -6);

            graph.AddEdge(d, e, 1);
            graph.AddEdge(d, f, 1);

            return graph;
        }

        /*
            A -> [B (1)]
            B -> [C (1)][E (1)]
            C -> [A (1)]
            D -> [E (1)]
            E -> [B (1)][F (1)]
            F -> [D (1)][I (1)]
            G -> [H (1)]
            H -> [I (1)]
            I -> [G (1)]
        */
        private Graph<string> CreateGraphWithBridges()
        {
            Graph<string> graph = new();

            int a = graph.AddVertex("A");
            int b = graph.AddVertex("B");
            int c = graph.AddVertex("C");
            int d = graph.AddVertex("D");
            int e = graph.AddVertex("E");
            int f = graph.AddVertex("F");
            int g = graph.AddVertex("G");
            int h = graph.AddVertex("H");
            int i = graph.AddVertex("I");

            graph.AddEdge(a, b, 1);
            graph.AddEdge(b, c, 1);
            graph.AddEdge(c, a, 1);

            graph.AddUndirectedEdge(b, e, 1);

            graph.AddEdge(d, e, 1);
            graph.AddEdge(e, f, 1);
            graph.AddEdge(f, d, 1);

            graph.AddEdge(f, i, 1);

            graph.AddEdge(g, h, 1);
            graph.AddEdge(h, i, 1);
            graph.AddEdge(i, g, 1);

            return graph;
        }

        [Test]
        public void DijkstraDistance()
        {
            Graph<string> graph = CreateGraph();

            _ = graph.GetShortestPath(0, 4, out double distance);

            Assert.That(distance, Is.EqualTo(5.0));
        }

        [Test]
        public void DijkstraPath()
        {
            Graph<string> graph = CreateGraph();

            Vertex<string>[] shortestPathFromAtoE = graph.GetShortestPath(0, 4, out double _);

            Assert.Multiple(() =>
            {
                Assert.That(shortestPathFromAtoE[0].Value, Is.EqualTo("A"));
                Assert.That(shortestPathFromAtoE[1].Value, Is.EqualTo("B"));
                Assert.That(shortestPathFromAtoE[2].Value, Is.EqualTo("C"));
                Assert.That(shortestPathFromAtoE[3].Value, Is.EqualTo("E"));
            });
        }

        [Test]
        public void Topsort()
        {
            Graph<string> graph = CreateGraph();
            graph.Topsort();

            Assert.Multiple(() =>
            {
                Assert.That(graph.GetVertex(0).Value, Is.EqualTo("A"));
                Assert.That(graph.GetVertex(1).Value, Is.EqualTo("B"));
                Assert.That(graph.GetVertex(2).Value, Is.EqualTo("D"));
                Assert.That(graph.GetVertex(3).Value, Is.EqualTo("E"));
                Assert.That(graph.GetVertex(4).Value, Is.EqualTo("C"));
            });
        }

        [Test]
        public void BellmanFordShortestDistancesFrom()
        {
            Graph<string> graph = CreateGraph();
            double[] expected = { 0, 1, 2, 3, 5 };

            double[] shortestDistancesFromA = graph.GetShortestDistances(0);
          
            Assert.That(shortestDistancesFromA, Is.EqualTo(expected));         
        }

        [Test]
        public void BellmanFordNegativeCycles()
        {
            Graph<string> graph = CreateGraphWithNegativeCycles();
            double[] expected = { 0, double.NegativeInfinity, -1, 5, 6, 6 };

            double[] shortestDistancesFromA = graph.GetShortestDistances(0);

            Assert.That(shortestDistancesFromA, Is.EqualTo(expected));
        }

        [Test]
        public void FloydWashallAllPairDistances()
        {
            const double i = double.PositiveInfinity;
            double[,] expected = { { 0, 1, 2, 3, 5 },
                                   { i, 0, 1, 2, 4 },
                                   { i, i, 0, 1, 3 },
                                   { i, i, 7, 0, 4 },
                                   { i, i, 3, 4, 0 } };

            Graph<string> graph = CreateGraph();
            double[,] distances = graph.GetAllPairShortestDistances();

            Assert.That(distances, Is.EqualTo(expected));
        }

        [Test]
        public void FloydWashallAllPairPaths()
        {
            // TODO
        }

        [Test]
        public void FindBridges()
        {
            Graph<string> graph = CreateGraphWithBridges();

            (Vertex<string>, Vertex<string>)[] bridges = graph.FindBridges();

            Assert.Multiple(() =>
            {
                Assert.That(bridges[0].Item1.Value, Is.EqualTo("F"));
                Assert.That(bridges[0].Item2.Value, Is.EqualTo("I"));
                Assert.That(bridges[1].Item1.Value, Is.EqualTo("B"));
                Assert.That(bridges[1].Item2.Value, Is.EqualTo("E"));
            });
        }

        [Test]
        public void FindArticulationPoints()
        {
            Graph<string> graph = CreateGraphWithBridges();

            Vertex<string>[] articulationPoints = graph.FindArticulationPoints();

            Assert.Multiple(() =>
            {
                Assert.That(articulationPoints[0].Value, Is.EqualTo("B"));
                Assert.That(articulationPoints[1].Value, Is.EqualTo("E"));
                Assert.That(articulationPoints[2].Value, Is.EqualTo("F"));
                Assert.That(articulationPoints[3].Value, Is.EqualTo("I"));
            });
        }
    }
}