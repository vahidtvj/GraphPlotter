using Heap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPlotter
{
    public static class Prim
    {
        /// <summary>
        /// Finds Minimum Spanning Tree of the given Nodes Using Prim's algorithm in O(E+V*Log V)
        /// </summary>
        /// <param name="nodes">All nodes in graph</param>
        /// <param name="start">Starting point</param>
        /// <returns>A list of possible nodes with each nodes parent set to another</returns>
        public static List<Node> FindMST(List<Node> nodes, Node start)
        {
            if (start == null) throw new Exception("Given start node or end node is incorrect");
            List<Node> set = new List<Node>();

            var q = Initialize(nodes, start);           //done in O(E)

            while (!q.IsEmpty)                          //runs V times
            {
                Node u = q.RemoveMin().Data;            //O(log V)
                if (u.heapItem.Key == int.MaxValue) return set;
                set.Add(u);

                foreach (Edge item in u.edges)
                {
                    Node v = item.endNode;
                    int alt = u.heapItem.Key + item.Distance;
                    if (alt < v.heapItem.Key)
                    {
                        q.DecreaseKey(v.heapItem, alt); //O(1)
                        v.parentVertex = u;
                    }
                }
            }
            return set;
        }

        /// <summary>
        /// Sets all required values to default in O(E)
        /// </summary>
        /// <param name="nodes">All nodes inside graph</param>
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
