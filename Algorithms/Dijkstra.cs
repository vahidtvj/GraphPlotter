using System;
using System.Collections.Generic;
using System.Linq;
using Heap;
using System.Text;
using System.Threading.Tasks;

namespace GraphPlotter
{
    public static class Dijkstra
    {
        /// <summary>
        /// Finds shortest path from start to end using Dijkstra algorithm in O(E+V*Log V)
        /// </summary>
        /// <param name="nodes">All nodes in graph</param>
        /// <param name="start">Starting point</param>
        /// <param name="end">Finish line</param>
        /// <returns>A sequence of nodes representing the path from start to end</returns>
        public static List<Node> FindPath(List<Node> nodes, Node start, Node end)
        {
            List<Node> Path = new List<Node>();
            if (start == null) throw new Exception("Given start node or end node is incorrect");
            if (start == end) throw new Exception("Given start node and end node are the same");

            var q = Initialize(nodes, start);       //done in O(E)

            while (!q.IsEmpty)     //runs less than V times 
            {
                Node u = q.RemoveMin().Data;        //O(log V)
                if (u == end) break;
                foreach (Edge edge in u.edges)
                {
                    Node v = edge.endNode;
                    int alt = u.heapItem.Key + edge.Distance;
                    if (alt < v.heapItem.Key)
                    {
                        q.DecreaseKey(v.heapItem, alt);     //O(1)
                        v.parentVertex = u;
                    }
                }
            }

            Node temp = end;
            do
            {
                Path.Insert(0, temp);
                if (temp.parentVertex == null) return null;
                temp = temp.parentVertex;
            } while (temp != start);

            Path.Insert(0, temp);
            return Path;
        }

        /// <summary>
        /// Sets all required values to default in O(E)
        /// </summary>
        /// <param name="nodes">All nodes inside graph</param>
        /// <param name="start">Starting Node of Path</param>
        /// <returns>Fibonacci heap of all elements with distance as key</returns>
        private static FibonacciHeap<Node, int> Initialize(List<Node> nodes, Node start)
        {
            FibonacciHeap<Node, int> q = new FibonacciHeap<Node, int>();

            foreach (Node node in nodes)
            {
                node.parentVertex = null;
                node.isPath = false;
                if (node == start) node.heapItem = q.Insert(node, 0);
                else node.heapItem = q.Insert(node, int.MaxValue);
                foreach (Edge edge in node.edges)
                    edge.isPath = false;
            }
            return q;
        }
    }
}
