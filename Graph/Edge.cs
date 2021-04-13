using System;
using System.Windows;
using System.Windows.Media.Animation;
using MvvmFoundation.Wpf;
using Thriple.Easing;
using System.Windows.Media;
using System.Runtime.Serialization;

namespace GraphPlotter
{
    [Serializable]
    public class Edge : ArrowLine, ISerializable, IComparable
    {
        #region Fields
        private int _distance;
        public double Pheromone;
        public readonly Node startNode, endNode;

        readonly PropertyObserver<Node> startObserver, endObserver;
        static readonly Duration _Duration = new Duration(TimeSpan.FromMilliseconds(2000));

        private readonly SolidColorBrush DefaultStroke = new SolidColorBrush(Colors.Maroon);
        private readonly SolidColorBrush SelectedStroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#e59432"));
        private readonly SolidColorBrush PathStroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#629947"));
        public readonly double DefaultThickness = 2;

        private readonly SolidColorBrush[] colors = new SolidColorBrush[]
        {
            new SolidColorBrush(Colors.Maroon),     //default stroke
            new SolidColorBrush(Colors.Red),
            new SolidColorBrush(Colors.Blue),
            new SolidColorBrush(Colors.Purple),
            new SolidColorBrush(Colors.Green),
            new SolidColorBrush(Colors.Black),
            new SolidColorBrush(Colors.Violet),
            new SolidColorBrush(Colors.Orange)
        };
        private int color;

        private readonly double dualOffset = 3;

        private bool _selected;
        private bool _isPath;
        private bool _isDual;
        #endregion

        #region Helpers
        static Point ComputeLocation(Node node1, Node node2)
        {
            // Initially set the location to the center of the first node.
            Point loc = new Point
            {
                X = node1.LocationX + (Node.NodeWidth / 2),
                Y = node1.LocationY + (Node.NodeHeight / 2)
            };

            bool overlapY = Math.Abs(node1.LocationY - node2.LocationY) < Node.NodeHeight;
            if (!overlapY)
            {
                bool above = node1.LocationY < node2.LocationY;
                if (above)
                    loc.Y += Node.NodeHeight / 2;
                else
                    loc.Y -= Node.NodeHeight / 2;
            }

            bool overlapX = Math.Abs(node1.LocationX - node2.LocationX) < Node.NodeWidth;
            if (!overlapX)
            {
                bool left = node1.LocationX < node2.LocationX;
                if (left)
                    loc.X += Node.NodeWidth / 2;
                else
                    loc.X -= Node.NodeWidth / 2;
            }

            return loc;
        }
        void SetToolTip()
        {
            string toolTipText = String.Format("Distance is : {0}", this.Distance);

            //if (endNode.NodeDependencies.Contains(startNode))
            //    toolTipText += String.Format("\n{0} depends on {1}", endNode.ID, startNode.ID);

            base.ToolTip = toolTipText;
        }

        void UpdateLocations(bool animate)
        {
            var start = ComputeLocation(startNode, endNode);
            var end = ComputeLocation(endNode, startNode);

            if (_isDual)
            {
                Vector vector = end - start;
                Vector RighAngle = new Vector(vector.Y, -vector.X);
                RighAngle.Normalize();
                RighAngle *= dualOffset;
                start -= RighAngle;
                end -= RighAngle;
            }

            if (animate)
            {
                base.BeginAnimation(ArrowLine.X1Property, CreateAnimation(base.X1, start.X));
                base.BeginAnimation(ArrowLine.Y1Property, CreateAnimation(base.Y1, start.Y));
                base.BeginAnimation(ArrowLine.X2Property, CreateAnimation(base.X2, end.X));
                base.BeginAnimation(ArrowLine.Y2Property, CreateAnimation(base.Y2, end.Y));
            }
            else
            {
                base.X1 = start.X;
                base.Y1 = start.Y;
                base.X2 = end.X;
                base.Y2 = end.Y;
            }
        }

        public static AnimationTimeline CreateAnimation(double from, double to)
        {
            return new EasingDoubleAnimation
            {
                Duration = _Duration,
                Equation = EasingEquation.ElasticEaseOut,
                From = from,
                To = to
            };
        }


        #endregion

        #region Constructor
        public Edge(Node startNode, Node endNode, int distance)
        {
            this.startNode = startNode;
            this.endNode = endNode;
            this.Distance = distance;
            color = 0;
            this.Stroke = colors[0];
            this.StrokeThickness = this.DefaultThickness;
            base.Length = this._distance.ToString();
            base.TextFill = this.DefaultStroke;


            this.SetToolTip();
            this.UpdateLocations(false);

            startObserver = new PropertyObserver<Node>(this.startNode)
                .RegisterHandler(n => n.LocationX, n => this.UpdateLocations(true))
                .RegisterHandler(n => n.LocationY, n => this.UpdateLocations(true));

            endObserver = new PropertyObserver<Node>(this.endNode)
                .RegisterHandler(n => n.LocationX, n => this.UpdateLocations(true))
                .RegisterHandler(n => n.LocationY, n => this.UpdateLocations(true));
        }
        #endregion

        #region Properties
        public int Color
        {
            get { return color; }
            set
            {
                this.color = value;
                this.Stroke = colors[color];
                this.TextFill = colors[color];
            }
        }
        public bool isDual
        {
            get { return this._isDual; }
            set
            {
                this._isDual = value;
                this.UpdateLocations(true);
            }
        }
        public bool isPath
        {
            get { return this._isPath; }
            set
            {
                this._isPath = value;
                this.Stroke = value ? this.PathStroke : (_selected ? this.SelectedStroke : colors[color]);
                this.TextFill = value ? this.PathStroke : (_selected ? this.SelectedStroke : colors[color]);
            }
        }

        public bool Selected
        {
            get { return this._selected; }
            set
            {
                this._selected = value;
                this.Stroke = value ? this.SelectedStroke : (_isPath ? this.PathStroke : colors[color]);
                this.TextFill = value ? this.SelectedStroke : (_isPath ? this.PathStroke : colors[color]);
            }
        }

        public int Distance
        {
            get { return this._distance; }
            set
            {
                this._distance = value;
                base.Length = value.ToString();
                SetToolTip();
            }
        }
        #endregion

        #region Serializer
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("distance", _distance, typeof(int));
            info.AddValue("startNode", startNode, typeof(Node));
            info.AddValue("endNode", endNode, typeof(Node));
            info.AddValue("isDual", _isDual, typeof(bool));
        }
        public Edge(SerializationInfo info, StreamingContext context)
        {
            this.Distance = (int)info.GetValue("distance", typeof(int));
            this.startNode = (Node)info.GetValue("startNode", typeof(Node));
            this.endNode = (Node)info.GetValue("endNode", typeof(Node));
            this._isDual = (bool)info.GetValue("isDual", typeof(bool));
            color = 0;
            this.Stroke = colors[0];
            this.StrokeThickness = this.DefaultThickness;
            base.Length = this._distance.ToString();
            base.TextFill = this.DefaultStroke;

            this.SetToolTip();
            this.UpdateLocations(false);

            startObserver = new PropertyObserver<Node>(this.startNode)
                .RegisterHandler(n => n.LocationX, n => this.UpdateLocations(true))
                .RegisterHandler(n => n.LocationY, n => this.UpdateLocations(true));

            endObserver = new PropertyObserver<Node>(this.endNode)
                .RegisterHandler(n => n.LocationX, n => this.UpdateLocations(true))
                .RegisterHandler(n => n.LocationY, n => this.UpdateLocations(true));
        }
        #endregion

        #region Comparer
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            Edge otherEdge = obj as Edge;
            if (otherEdge == null) throw new ArgumentException("Object is not an Edge");
            return this._distance.CompareTo(otherEdge._distance);
        }
        #endregion
    }

}
