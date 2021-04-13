using MvvmFoundation.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphPlotter
{
    [Serializable]
    public class Graph
    {
        #region Constructor
        public Graph()
        {
            nodes = new List<Node>();
        }
        #endregion

        #region Fields
        public List<Node> nodes;
        [NonSerialized]
        public bool run = false;
        [NonSerialized]
        public Task<object[]> task;
        [NonSerialized]
        public Xceed.Wpf.Toolkit.BusyIndicator Busy;
        [NonSerialized]
        public MainWindow window;
        #endregion

        #region Methods
        public void AddNode(Node node)
        {
            nodes.Add(node);
        }


        public void AddEdge(Node startNode, Node endNode, int distance)
        {
            startNode.edges.Add(new Edge(startNode, endNode, distance));
        }

        public void ClearColors()
        {
            foreach (Node node in nodes)
            {
                node.Color = 0;
                node.isPath = false;
                node.Selected = false;
                foreach (Edge edge in node.edges)
                {
                    edge.Color = 0;
                    edge.isPath = false;
                    edge.Selected = false;
                }
            }
        }

        public List<Node> GetPath(Node Start, Node End)
        {
            ClearColors();
            List<Node> path = Dijkstra.FindPath(this.nodes, Start, End);
            if (path != null)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    path[i].isPath = true;
                    if (path[i] != End)
                    {
                        Edge edge = path[i].edges.Find(x => x.endNode == path[i + 1]);
                        edge.isPath = true;
                    }
                }
            }
            return path;
        }

        public List<Node> FindMSTPrim(Node start)
        {
            ClearColors();
            List<Node> set = Prim.FindMST(this.nodes, start);
            for (int i = 0; i < set.Count; i++)
            {
                set[i].isPath = true;
                if (i > 0)
                {
                    Edge edge = set[i].parentVertex.edges.Find(x => x.endNode == set[i]);
                    edge.isPath = true;
                }
            }
            return set;
        }

        public List<Edge> FindMSTKruskal()
        {
            ClearColors();
            List<Edge> set = Kruskal.FindMST(this.nodes);
            foreach (Edge edge in set)
            {
                edge.isPath = true;
                edge.startNode.isPath = true;
                edge.endNode.isPath = true;
            }
            return set;
        }

        public List<Node> FindSalesManPath(Node start, out int distance)
        {
            ClearColors();
            List<Node> path = Salesman.GetPath(this.nodes, start, out int _distance);
            distance = _distance;
            if (path == null) return null;
            for (int i = 0; i < path.Count - 1; i++)
            {
                path[i].isPath = true;
                Edge edge = path[i].edges.Find(x => x.endNode == path[i + 1]);
                edge.isPath = true;
            }
            return path;
        }

        public int[] ColorGraph(out int count)
        {
            ClearColors();
            count = 0;
            if (!GraphColoring.GetColors(nodes, out int[] colors, out int ColorC)) return null;
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Color = colors[i];
                foreach (Edge edge in nodes[i].edges)
                    edge.Color = colors[i];
            }
            count = ColorC;
            return colors;
        }

        public void StartAntColony()
        {
            run = true;
            task = new Task<object[]>(() =>
            {
                Busy.Dispatcher.Invoke(() => Busy.IsBusy = true);
                window.Dispatcher.Invoke(() =>
                {
                    window.BusyCost = int.MaxValue;
                    window.BusyDiff = 0;
                });
                object obj = AntColony.FindPath(this, out int cost, ref run);
                return new object[] { obj, cost };
            });
            task.Start();
        }

        public Edge[] FindSalesManPathAntColony(out int distance)
        {
            distance = 0;
            if (run == false) return null;

            run = false;
            ClearColors();
            if (task.IsCompleted == false) task.Wait();
            Busy.IsBusy = false;

            Edge[] path = task.Result[0] as Edge[];
            distance = (int)task.Result[1];

            if (distance == int.MaxValue) return null;

            foreach (Edge edge in path)
            {
                edge.isPath = true;
                edge.startNode.isPath = true;
                edge.endNode.isPath = true;
            }
            return path;
        }
        #endregion

    }
}
