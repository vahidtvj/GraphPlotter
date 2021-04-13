using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Search;

namespace GraphPlotter
{
    public class Ant
    {
        #region Fields
        public readonly double Alpha = 1;
        public readonly double Beta = 1;

        private HashSet<Node> visited;

        private Node StartNode;
        private readonly int NodeCount;

        private readonly Graph graph;
        private readonly Random random;
        #endregion

        #region Properties
        public Edge[] Route { get; }
        public int RouteCost { get; private set; }
        #endregion

        #region Constructors
        public Ant(Graph Graph, double Alpha, double Beta, Random random)
        {
            this.graph = Graph;
            this.Alpha = Alpha;
            this.Beta = Beta;
            this.random = random;
            this.NodeCount = graph.nodes.Count;
            visited = new HashSet<Node>();

            //  Randomly place the ant on a node
            this.StartNode = graph.nodes[random.Next(NodeCount)];
            Route = new Edge[NodeCount];
            RouteCost = 0;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Searches for a route and stores in Route and RouteCost
        /// </summary>
        /// <returns>Whether it is a valid route or not</returns>
        public bool FindRoute()
        {
            return findRoute(StartNode, 0);
        }
        #endregion

        #region Helpers
        private bool findRoute(Node current, int index)
        {
            visited.Add(current);
            List<Edge> edges = new List<Edge>();
            foreach (Edge edge in current.edges)
                if (!visited.Contains(edge.endNode)) edges.Add(edge);   // if already visited don't visit again -> contains O(1)

            if (edges.Count > 0)
            {
                int x = ChooseAPath(edges);
                if (x == -1) return false;
                Edge edge = edges.ElementAt(x);
                Route[index] = edge;
                RouteCost += edge.Distance;
                return findRoute(edge.endNode, index + 1);
            }
            else
            {
                if (index == NodeCount - 1)
                {
                    Edge edge = current.edges.Find(x => x.endNode == StartNode);
                    if (edge != null)
                    {
                        Route[index] = edge;
                        RouteCost += edge.Distance;
                        return true;
                    }
                }
                return false;
            }

        }

        private int ChooseAPath(List<Edge> edges)
        {
            double sum = 0;
            int n = edges.Count;
            double[] CDF = new double[n];
            int i = 0;
            // Fill Array of Probabilities (not divided by sum yet)
            foreach (Edge edge in edges)
            {
                double x = edge.Distance;
                CDF[i] = Math.Pow(edge.Pheromone, Alpha) * Math.Pow(1 / x, Beta);
                sum += CDF[i];
            }
            if (sum == 0) return -1;
            CDF[0] /= sum;
            for (i = 1; i < n; i++)
                CDF[i] = (CDF[i] / sum) + CDF[i - 1];
            // At this point CDF is Cumulative distribution function of all probabilities on current's Edges
            double rand = random.NextDouble();
            int result = Search<double>.BinarySearch(CDF, rand);
            return result;
        }
        #endregion
    }
}
