using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GraphPlotter
{
    public class InitScreen
    {
        public static Graph GetGraph()
        {
            Graph graph = new Graph();
            Node node = new Node("1", 10, 10);
            Node node2 = new Node("2", 150, 150);
            Node node3 = new Node("3", 150, 300);
            graph.AddNode(node);
            graph.AddNode(node2);
            graph.AddNode(node3);
            graph.AddEdge(graph.nodes[0], graph.nodes[1], 500);
            graph.AddEdge(graph.nodes[1], graph.nodes[2], 500);
            return graph;
        }
    }
}
