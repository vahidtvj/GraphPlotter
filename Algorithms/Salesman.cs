using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPlotter
{
    public static class Salesman
    {
        public static List<Node> GetPath(List<Node> nodes, Node start, out int distance)
        {
            Initialize(nodes);
            distance = getpath(start, start, nodes.Count, 1, out List<Node> path);
            if (distance == int.MaxValue) path = null;
            return path;
        }
        //TODO => calc O
        private static int getpath(Node start, Node current, int n, int level, out List<Node> path)
        {
            path = new List<Node>() { current };
            current.visited = true;

            bool isEnd = true;

            List<Node> minValue = new List<Node>();
            int min = int.MaxValue;

            foreach (Edge edge in current.edges)
            {
                if (edge.endNode.visited == false)
                {
                    isEnd = false;
                    int temp = getpath(start, edge.endNode, n, level + 1, out List<Node> childpath);
                    if (temp != int.MaxValue) temp += edge.Distance;
                    if (temp < min)
                    {
                        min = temp;
                        minValue = childpath;
                    }
                }
            }

            current.visited = false;

            if (!isEnd)
                path.AddRange(minValue);
            else if (level != n)
                return int.MaxValue;
            else
            {
                Edge edge = current.edges.Find(x => x.endNode == start);
                if (edge == null) return int.MaxValue;
                path.Add(start);
                min = edge.Distance;
            }

            return min;
        }

        /// <summary>
        /// Sets all required values to default in O(E)
        /// </summary>
        /// <param name="nodes">All nodes inside graph</param>
        private static void Initialize(List<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                node.isPath = false;
                node.visited = false;
                foreach (Edge edge in node.edges)
                    edge.isPath = false;
            }
        }
    }
}
