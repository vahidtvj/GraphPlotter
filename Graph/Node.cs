using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Media;
using Heap;

namespace GraphPlotter
{
    [Serializable]
    public class Node : NodeShape, INotifyPropertyChanged, IComparable, ISerializable
    {
        #region Fields
        public string name { get; set; }
        double _locationX, _locationY;
        public List<Edge> edges;
        private bool _isPath;
        private bool _isSelected;

        private readonly SolidColorBrush SelectedFill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#e59432"));
        private readonly SolidColorBrush PathFill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#629947"));

        private readonly SolidColorBrush DefaultTextFill = new SolidColorBrush(Colors.White);

        private readonly double DefaultRadius = 2;

        private readonly SolidColorBrush DefaultStroke = new SolidColorBrush(Colors.LightGray);
        private readonly double DefaultThickness = 0;

        private readonly SolidColorBrush[] colors = new SolidColorBrush[]
        {
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#379acc")),     //default Fill
            new SolidColorBrush(Colors.Red),
            new SolidColorBrush(Colors.Blue),
            new SolidColorBrush(Colors.Purple),
            new SolidColorBrush(Colors.Green),
            new SolidColorBrush(Colors.Black),
            new SolidColorBrush(Colors.Violet),
            new SolidColorBrush(Colors.Orange)
        };

        public bool visited;
        public int distance;
        public Node parentVertex;
        public int rank;
        public FibonacciHeapNode<Node, int> heapItem;

        private int color;
        #endregion

        #region Properties
        public int Color
        {
            get { return color; }
            set
            {
                this.color = value;
                this.Fill = colors[color];
            }
        }
        public bool Selected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                this.Fill = value ? this.SelectedFill : (_isPath ? this.PathFill : colors[color]);
            }
        }
        public bool isPath
        {
            get { return _isPath; }
            set
            {
                _isPath = value;
                this.Fill = value ? this.PathFill : (_isSelected ? this.SelectedFill : colors[color]);
            }
        }
        public static double NodeWidth
        {
            get { return 40; }
        }

        public static double NodeHeight
        {
            get { return 40; }
        }

        public double LocationX
        {
            get { return _locationX; }
            set
            {
                if (value == _locationX)
                    return;

                _locationX = value;
                base.X = value;
                RaisePropertyChanged("LocationX");
            }
        }

        public double LocationY
        {
            get { return _locationY; }
            set
            {
                if (value == _locationY)
                    return;

                _locationY = value;
                base.Y = value;
                RaisePropertyChanged("LocationY");
            }
        }
        #endregion

        #region Constructor
        public Node(string name, double locationX, double locationY)
        {
            this.name = name;
            base._Name = name;

            this.edges = new List<Edge>();

            this.LocationX = locationX;
            this.LocationY = locationY;
            base.X = locationX;
            base.Y = locationY;

            color = 0;
            this.Fill = colors[0];
            this.TextFill = this.DefaultTextFill;
            this.Radius = this.DefaultRadius;

            this.Stroke = this.DefaultStroke;
            this.StrokeThickness = this.DefaultThickness;

            this.parentVertex = null;

        }
        #endregion

        #region ObjectObserver

        #region RaisePropertyChanged

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion // RaisePropertyChanged

        #region Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This 
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // If you raise PropertyChanged and do not specify a property name,
            // all properties on the object are considered to be changed by the binding system.
            if (String.IsNullOrEmpty(propertyName))
                return;

            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new ArgumentException(msg);
                else
                    Debug.Fail(msg);
            }
        }




        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might 
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion // Debugging Aides

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion // INotifyPropertyChanged Members

        #endregion

        #region Base Class Implementations
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", this.name, typeof(string));

            info.AddValue("edges", edges, typeof(List<Edge>));

            info.AddValue("X", LocationX, typeof(double));
            info.AddValue("Y", LocationY, typeof(double));
        }

        public Node(SerializationInfo info, StreamingContext context)
        {
            this.name = (string)info.GetValue("name", typeof(string));
            base._Name = this.name;

            this.edges = (List<Edge>)info.GetValue("edges", typeof(List<Edge>));

            this.LocationX = (double)info.GetValue("X", typeof(double));
            this.LocationY = (double)info.GetValue("Y", typeof(double));
            base.X = this.LocationX;
            base.Y = this.LocationY;

            color = 0;
            this.Fill = colors[0];
            this.TextFill = this.DefaultTextFill;
            this.Radius = this.DefaultRadius;

            this.Stroke = this.DefaultStroke;
            this.StrokeThickness = this.DefaultThickness;

            this.parentVertex = null;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            Node otherNode = obj as Node;
            if (otherNode == null) throw new ArgumentException("Object is not a Node");
            return this.distance.CompareTo(otherNode.distance);
        }
        #endregion
    }

}
