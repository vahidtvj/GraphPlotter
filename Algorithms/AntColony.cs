using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphPlotter
{
    public static class AntColony
    {
        private const double Alpha = .3;
        private const double Beta = .6;
        private const int AntCount = 2;
        private const double P = 0.5;
        /// <summary>
        /// Finds A Tour Path using Ant Colony Algorithm. Use inside a Task (stop using 'run = false')
        /// </summary>
        /// <param name="graph">Input Graph</param>
        /// <param name="Cost">Output minimum cost Route. infinity if no route found</param>
        /// <param name="run">Controls how many iterations the algorithm does</param>
        /// <returns>A list of selected Edges representing a Path</returns>
        public static Edge[] FindPath(Graph graph, out int Cost, ref bool run)
        {
            List<Edge> AllEdges = Initialize(graph.nodes);
            Random random = new Random(Guid.NewGuid().GetHashCode());
            Ant[] ants = new Ant[AntCount];

            Edge[] Route = new Edge[graph.nodes.Count];
            Cost = int.MaxValue;

            while (run)
            {
                // find route for all ants
                //for (int i = 0; i < AntCount; i++)
                //{
                //    ants[i] = new Ant(graph, Alpha, Beta, random);
                //    if (!ants[i].FindRoute()) ants[i] = null;
                //}

                Parallel.For(0, AntCount, i =>
                  {
                      ants[i] = new Ant(graph, Alpha, Beta, random);
                      if (!ants[i].FindRoute()) ants[i] = null;
                  });

                // For all Edges evaporate pheromone 
                foreach (Edge edge in AllEdges)
                {
                    edge.Pheromone *= (1 - P);
                }

                // for each ants route
                foreach (Ant ant in ants)
                {
                    if (ant == null) continue;
                    //find min path
                    if (ant.RouteCost < Cost)
                    {
                        graph.window.Dispatcher.Invoke(() => graph.window.BusyCost = ant.RouteCost);
                        Cost = ant.RouteCost;
                        Route = ant.Route;
                    }
                    // update pheromone
                    foreach (Edge edge in ant.Route)
                    {
                        edge.Pheromone += (1 / ant.RouteCost);
                    }
                }
            }
            return Route;
        }
        /// <summary>
        /// Sets all required values to default in O(E)
        /// </summary>
        /// <param name="nodes">All nodes inside graph</param>
        private static List<Edge> Initialize(List<Node> nodes)
        {
            List<Edge> edges = new List<Edge>();
            foreach (Node node in nodes)
            {
                //node.isPath = false;
                foreach (Edge edge in node.edges)
                {
                    edges.Add(edge);
                    //edge.isPath = false;
                    edge.Pheromone = 1;
                }
            }
            return edges;
        }
    }
}
