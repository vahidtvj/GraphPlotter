using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPlotter
{
    public static class GraphColoring
    {
        private static int ColorC;

        /// <summary>
        /// Colors all given nodes with minimum number of different colors
        /// </summary>
        /// <param name="nodes">All nodes inside graph</param>
        /// <param name="colors">Outputs an array of numbers each referring to a color</param>
        /// <returns></returns>
        public static bool GetColors(List<Node> nodes, out int[] colors, out int count)
        {
            int v = nodes.Count;
            colors = new int[v];
            bool[,] g = Initialize(nodes, v);

            ColorC = 1000;
            bool result= getColors(ref g, ref colors, 0, v, 0);
            count = ColorC;
            return result;
        }


        private static bool getColors(ref bool[,] g, ref int[] colors, int i, int v, int maxColor)
        {
            if (i == v)
            {
                ColorC = maxColor;
                return true;
            }

            bool result = false;

            for (int c = 1; c < ColorC; c++)
            {
                if (isSafe(ref g, ref colors, c, i, v))
                {
                    int temp = colors[i];
                    colors[i] = c;
                    if (getColors(ref g, ref colors, i + 1, v, (c < maxColor) ? maxColor : c)) result = true;
                    else colors[i] = temp;
                }
            }
            return result;
        }

        private static bool isSafe(ref bool[,] g, ref int[] colors, int c, int i, int v)
        {
            for (int k = 0; k < v; k++)
                if (g[i, k] == true || g[k, i] == true)
                    if (c == colors[k]) return false;
            return true;
        }

        /// <summary>
        /// Sets all required values to default in O(E)
        /// </summary>
        /// <param name="nodes">All nodes inside graph</param>
        /// <param name="v">Graph Node Count</param>
        /// <returns>Adjacency matrix of the graph</returns>
        private static bool[,] Initialize(List<Node> nodes, int v)
        {
            bool[,] g = new bool[v, v];
            for (int i = 0; i < v; i++)
                nodes[i].rank = i;

            foreach (Node node in nodes)
                foreach (Edge edge in node.edges)
                    g[node.rank, edge.endNode.rank] = true;

            return g;
        }
    }
}
