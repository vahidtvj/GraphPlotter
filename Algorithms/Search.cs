using System;
using System.Collections.Generic;
using System.Text;

namespace Search
{
    public static class Search<T> where T : IComparable
    {
        /// <summary>
        /// Binary Search an Array of items
        /// </summary>
        /// <param name="Array">Input Array</param>
        /// <param name="key">Item to search for</param>
        /// <returns>Index of the found item. if not found returns upper bound</returns>
        public static int BinarySearch(IList<T> Array, T key)
        {
            return BinarySearch(ref Array, ref key, 0, Array.Count - 1);
        }
        /// <summary>
        /// Binary Search an Array of items
        /// </summary>
        /// <param name="Array">Input Array</param>
        /// <param name="key">Item to search for</param>
        /// <param name="Start">Search lower bound</param>
        /// <param name="End">Search upper bound</param>
        /// <returns>Index of the found item. if not found returns upper bound</returns>
        public static int BinarySearch(IList<T> Array, T key, int Start, int End)
        {
            return BinarySearch(ref Array, ref key, 0, Array.Count - 1);
        }
       
        private static int BinarySearch(ref IList<T> Array, ref T key, int Low, int High)
        {
            while (Low < High)
            {
                int mid = (Low + High) / 2;
                int ans = Array[mid].CompareTo(key);
                if (ans > 0) High = mid - 1;
                else if (ans < 0) Low = mid + 1;
                else return mid;
            }
            return Low;
        }
    }
}
