using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPlotter
{
    public static class Kruskal
    {
        /// <summary>
        /// Finds root parent of an element in O(Log V)
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Root parent of given node</returns>
        private static Node find_set(Node node)
        {
            while (node.parentVertex != node)
                node = node.parentVertex.parentVertex;
            return node;
        }
        /// <summary>
        /// Unites two set of nodes if they are separate in O(Log V)
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        private static void union(Node node1, Node node2)
        {
            node1 = find_set(node1);
            node2 = find_set(node2);
            if (node1 == node2) return;

            if (node1.rank < node2.rank)    //swap
            {
                Node temp = node2;
                node2 = node1;
                node1 = temp;
            }

            node2.parentVertex = node1;
            if (node1.rank == node2.rank) node1.rank++;
        }
        /// <summary>
        /// Finds Minimum Spanning Tree of the given Nodes using Kruskal's algorithm in O(E*Log V)
        /// </summary>
        /// <param name="nodes">All nodes in graph</param>
        /// <returns>A set of edges whose nodes can be reached (Possible Nodes)</returns>
        public static List<Edge> FindMST(List<Node> nodes)
        {
            List<Edge> set = initialize(nodes);
            List<Edge> A = new List<Edge>();
            set.Sort();                         //O(E*Log E) -> O(E*Log V)

            while (set.Count > 0)               //runs E times
            {
                Edge e = set[0];
                set.RemoveAt(0);
                Node u = find_set(e.startNode);
                Node v = find_set(e.endNode);
                if (u != v)
                {
                    A.Add(e);
                    union(u, v);                //O(1) because both u and v are root nodes
                }
            }
            return A;
        }

        /// <summary>
        /// Sets all required values to default in O(E)
        /// </summary>
        /// <param name="nodes">All nodes inside graph</param>
        /// <returns>List of all Edges in graph</returns>
        private static List<Edge> initialize(List<Node> nodes)
        {
            List<Edge> set = new List<Edge>();
            foreach (Node node in nodes)
            {
                node.rank = 0;
                node.isPath = false;
                node.parentVertex = node;
                foreach (Edge edge in node.edges)
                {
                    edge.isPath = false;
                    set.Add(edge);
                }
            }
            return set;
        }

    }
}
