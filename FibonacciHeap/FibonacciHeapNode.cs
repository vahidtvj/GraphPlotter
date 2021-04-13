using System;

namespace Heap
{
    /// <summary>
    /// Represents the one node in the Fibonacci Heap.
    /// </summary>
    /// <typeparam name="T">Type of the object to be stored.</typeparam>
    /// <typeparam name="TKey">Type of the key to be used for the stored object. 
    /// Has to implement the <see cref="IComparable"/> interface.</typeparam>
    public class FibonacciHeapNode<T, TKey> : IComparable where TKey : IComparable<TKey>
    {
        #region Constructor
        public FibonacciHeapNode(T data, TKey key)
        {
            Right = this;
            Left = this;
            Data = data;
            Key = key;
            Parent = null;
            Child = null;
        }
        #endregion

        #region Fields
        public T Data { get; set; }

        public FibonacciHeapNode<T, TKey> Child { get; set; }
        public FibonacciHeapNode<T, TKey> Left { get; set; }
        public FibonacciHeapNode<T, TKey> Parent { get; set; }
        public FibonacciHeapNode<T, TKey> Right { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whatever node is marked (visited).
        /// </summary>
        public bool Mark { get; set; }

        /// <summary>
        /// Gets or sets the value of the node key.
        /// </summary>
        public TKey Key { get; set; }

        /// <summary>
        /// Gets or sets the value of the node degree.
        /// </summary>
        public int Degree { get; set; }
        #endregion

        #region comparer implementation
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            FibonacciHeapNode<T, TKey> otherNode = obj as FibonacciHeapNode<T, TKey>;
            if (otherNode == null) throw new ArgumentException("Object is not of type 'FibonacciHeapNode<T, TKey>'");
            return this.Key.CompareTo(otherNode.Key);
        }
        #endregion
    }
}
