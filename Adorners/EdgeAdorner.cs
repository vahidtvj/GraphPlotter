using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ToastNotifications;
using ToastNotifications.Messages;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace GraphPlotter
{
    public enum Mode
    {
        Normal,
        PathMode,
        MSTMode,
        SalesmanMode,
        ColoringMode,
        AntColonyMode
    }

    public static class SystemKey
    {
        public static bool ControlPressed
        {
            get { return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl); }
        }
        public static bool ShiftPressed
        {
            get { return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift); }
        }

    }

    public class EdgeAdorner : Adorner
    {

        #region Fields
        Graph graph;
        List<object> objs;
        private ArrowLine edgeDrawHelper = null;

        private object _selectedItem;
        public bool EdgeDrawMode;
        public bool NodeDragMode;
        public bool PathFindMode;
        private bool _editMode;
        Point dragOffset;
        List<int> removedNodes;

        Mode _mode;
        private bool _UnHalt = false;

        public static Size size;
        public static int padding = 4;

        private readonly Notifier notifier;

        MessageOptions opts = new MessageOptions
        {
            Tag = $"[This is Tag Value]",
            FreezeOnMouseEnter = true,
            UnfreezeOnMouseLeave = true,
            ShowCloseButton = true
        };
        #endregion

        #region Constructor
        public EdgeAdorner(UIElement adornedElement) : base(adornedElement)
        {
            objs = new List<object>();
            removedNodes = new List<int>();
            _mode = new Mode();

            notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.TopRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });

            Unloaded += (object sender, RoutedEventArgs e) => notifier.Dispose();
        }
        #endregion

        #region Properties
        public ArrowLine EdgeDrawHelper
        {
            get { return edgeDrawHelper; }
            set
            {
                if (edgeDrawHelper != null)
                {
                    objs.Remove(edgeDrawHelper);
                    base.RemoveLogicalChild(edgeDrawHelper);
                    base.RemoveVisualChild(edgeDrawHelper);
                }
                edgeDrawHelper = value;
                if (value != null)
                {
                    objs.Add(value);
                    base.AddLogicalChild(value);
                    base.AddVisualChild(value);
                }
                base.InvalidateMeasure();
            }
        }
        public Mode mode
        {
            get { return this._mode; }
            set
            {
                this._mode = value;
                switch (value)
                {
                    case Mode.Normal:
                        break;
                    case Mode.PathMode:
                        PathFindPressed();
                        break;
                    case Mode.MSTMode:
                        if (selectedItem != null && selectedItem.GetType() == typeof(Node)) PrimsKeyPressed();
                        else KruskalsKeyPressed();
                        break;
                    case Mode.SalesmanMode:
                        SalesmanPath();
                        break;
                    case Mode.ColoringMode:
                        ColoringPressed();
                        break;
                    case Mode.AntColonyMode:
                        AntColonyPressed();
                        break;
                    default:
                        break;
                }
            }
        }
        public bool UnHalt
        {
            get { return _UnHalt; }
            set
            {
                graph.FindSalesManPathAntColony(out int Cost);
                if (Cost == int.MaxValue) notifier.ShowWarning("No tour found with the given graph", opts);
                else notifier.ShowSuccess("Found a route with cost of " + Cost, opts);
            }
        }
        public Graph Graph
        {
            get { return graph; }
            set
            {
                if (value == graph)
                    return;
                selectedItem = null;
                EdgeDrawMode = false;
                EdgeDrawHelper = null;
                NodeDragMode = false;
                EditMode = false;
                graph = value;
                removedNodes = new List<int>();

                if (graph != null)
                    this.ProcessGraph();
            }
        }
        public object selectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (PathFindMode) PathFindSecondSelected(value);
                PathFindMode = false;

                if (_selectedItem != null && _selectedItem != value)
                {
                    if (_selectedItem.GetType() == typeof(Node))
                        ((Node)_selectedItem).Selected = false;
                    else
                        ((Edge)_selectedItem).Selected = false;
                }

                _selectedItem = value;

            }
        }
        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                if (_editMode == true)
                {
                    Edge edge = (Edge)selectedItem;
                    edge.IsEdit = false;
                    Keyboard.ClearFocus();
                    selectedItem = null;
                }
                _editMode = value;
            }
        }
        #endregion

        #region Helpers
        void ProcessGraph()
        {
            List<Edge> temp = new List<Edge>();
            this.ClearEdges();
            foreach (Node node in graph.nodes)
            {
                objs.Add(node);
                base.AddVisualChild(node);
                base.AddLogicalChild(node);
                temp.AddRange(node.edges);
            }
            objs.AddRange(temp);

            foreach (Edge item in temp)
            {
                base.AddVisualChild(item);
                base.AddLogicalChild(item);
            }

            base.InvalidateVisual();
        }

        void ClearEdges()   //No physical remove
        {
            foreach (object item in objs)
            {
                if (item.GetType() == typeof(Edge))
                {
                    base.RemoveVisualChild((Edge)item);
                    base.RemoveLogicalChild((Edge)item);
                }
                else
                {
                    base.RemoveVisualChild((Node)item);
                    base.RemoveLogicalChild((Node)item);
                }

            }
            objs.Clear();
        }

        void RemoveEdge(Edge edge)
        {
            if (edge.isDual) edge.endNode.edges.Find(x => x.endNode == edge.startNode).isDual = false;
            edge.startNode.edges.Remove(edge);
            this.ProcessGraph();
        }

        void RemoveNode(Node node)
        {
            if (int.TryParse(node.name, out int result)) removedNodes.Add(result);
            foreach (Node item in graph.nodes)
            {
                List<Edge> rem = item.edges.FindAll(x => x.endNode == node);
                foreach (Edge edge in rem)
                    item.edges.Remove(edge);
            }
            graph.nodes.Remove(node);
            this.ProcessGraph();
        }
        #endregion

        #region Methods

        bool AddEdge(Node startNode, Node endNode, int distance)
        {
            if (startNode.edges.Exists(x => x.endNode == endNode)) return false;

            Edge edge = new Edge(startNode, endNode, distance);

            Edge temp = endNode.edges.Find(x => x.endNode == startNode);
            if (temp != null)
            {
                edge.isDual = true;
                temp.isDual = true;
            }


            objs.Add(edge);
            startNode.edges.Add(edge);

            base.AddVisualChild(edge);
            base.AddLogicalChild(edge);
            base.InvalidateMeasure();

            EditMode = true;
            edge.IsEdit = true;
            selectedItem = edge;
            edge.Selected = true;
            edge.Focusable = true;
            Keyboard.Focus(edge);
            //InputDialog dialog = new InputDialog(string.Format("'{0}' and '{1}'", edge.startNode.name, edge.endNode.name), edge.Distance);
            //if (dialog.ShowDialog() == true) edge.Distance = dialog.Value;
            return true;
        }

        public void AddNode(Point point)
        {
            point.X -= 20;
            point.Y -= 20;

            double width = size.Width - Node.NodeWidth - padding * 2 - 2;
            double height = size.Height - Node.NodeHeight - padding * 2 - 35;

            if (point.X < 0) point.X = 0;
            else if (point.X > width) point.X = width;
            if (point.Y < 0) point.Y = 0;
            else if (point.Y > height) point.Y = height;



            string name = (graph.nodes.Count + 1).ToString();
            if (removedNodes.Any())
            {
                int min = removedNodes.Min();
                removedNodes.Remove(min);
                name = min.ToString();
            }
            Node node = new Node(name, point.X, point.Y);
            objs.Add(node);
            graph.AddNode(node);
            base.AddVisualChild(node);
            base.AddLogicalChild(node);
            base.InvalidateMeasure();
        }
        public void Duplicate(Edge edge)
        {
            if (edge.isDual == true) return;
            Edge edge2 = new Edge(edge.endNode, edge.startNode, edge.Distance);
            edge.endNode.edges.Add(edge2);
            objs.Add(edge2);
            edge.isDual = true;
            edge2.isDual = true;

            base.AddVisualChild(edge2);
            base.AddLogicalChild(edge2);
            base.InvalidateMeasure();
        }
        public void DuplicateAll()
        {
            List<Edge> edges = new List<Edge>();
            foreach (Node node in graph.nodes)
                foreach (Edge edge in node.edges)
                    if (edge.isDual == false) edges.Add(edge);
            foreach (Edge edge in edges) Duplicate(edge);
        }
        #endregion

        #region Key Occurrences
        private void dupPressed()
        {
            if (selectedItem != null && selectedItem.GetType() == typeof(Edge)) Duplicate((Edge)selectedItem);
        }
        private void dupAllPressed()
        {
            DuplicateAll();
        }
        private void PathFindPressed()
        {
            ClearPath();
            mode = Mode.Normal;
            if (selectedItem != null && selectedItem.GetType() == typeof(Node))
            {
                PathFindMode = true;
                notifier.ShowInformation("Select another Node as End Point", opts);
            }
            else
                notifier.ShowWarning("No Node is Selected as start point", opts);
        }

        private bool PathFindSecondSelected(object obj)
        {
            if (obj != null && obj.GetType() == typeof(Node))
            {
                PathFindMode = false;
                Node start = (Node)selectedItem;
                Node end = (Node)obj;
                if (start != end)
                {
                    if (graph.GetPath(start, end) != null)
                    {
                        notifier.ShowSuccess("Found a path with distance of " + end.heapItem.Key, opts);
                        return true;
                    }
                    else
                        notifier.ShowError("No Path found between given Nodes", opts);
                }
                else
                    notifier.ShowError("Start and End Nodes can't be the same", opts);
            }
            else
                notifier.ShowError("End point should be a Node");
            return false;
        }

        private void ClearPath()
        {
            foreach (Node node in graph.nodes)
            {
                node.Color = 0;
                node.isPath = false;
                foreach (Edge edge in node.edges)
                {
                    edge.Color = 0;
                    edge.isPath = false;
                }
            }
        }
        private void EditModeKeyPress(KeyEventArgs e)
        {
            int x = -1;
            if (e.Key.CompareTo(Key.NumPad0) >= 0 && e.Key.CompareTo(Key.NumPad9) <= 0)
                x = (int)e.Key - 74;
            else if (e.Key.CompareTo(Key.D0) >= 0 && e.Key.CompareTo(Key.D9) <= 0)
                x = (int)e.Key - 34;

            if (x != -1)
            {
                Edge edge = (Edge)selectedItem;
                x += edge.Distance * 10;
                edge.Distance = x;
            }
            else if (e.Key == Key.Back)
            {
                Edge edge = (Edge)selectedItem;
                edge.Distance /= 10;
            }
            else if (e.Key == Key.Enter)
            {
                EditMode = false;
            }
        }
        private void EditKeyPressed()
        {
            if (selectedItem != null && selectedItem.GetType() == typeof(Edge))
            {
                Edge edge = (Edge)selectedItem;
                EditMode = true;
                edge.IsEdit = true;
                edge.Focusable = true;
                Keyboard.Focus(edge);
                //InputDialog dialog = new InputDialog(string.Format("'{0}' and '{1}'", edge.startNode.name, edge.endNode.name), edge.Distance);
                //if (dialog.ShowDialog() == true) edge.Distance = dialog.Value;
            }
        }
        private void DeleteKeyPressed()
        {
            if (selectedItem == null) return;
            if (selectedItem.GetType() == typeof(Edge))
            {
                RemoveEdge((Edge)selectedItem);
            }
            else if (selectedItem.GetType() == typeof(Node))
            {
                RemoveNode((Node)selectedItem);
            }
            selectedItem = null;
            EdgeDrawMode = false;
            EdgeDrawHelper = null;
        }
        private void PrimsKeyPressed()
        {
            ClearPath();
            if (selectedItem != null && selectedItem.GetType() == typeof(Node))
            {
                Node start = (Node)selectedItem;
                graph.FindMSTPrim(start);
            }
            selectedItem = null;
        }
        private void KruskalsKeyPressed()
        {
            ClearPath();
            _mode = Mode.Normal;
            graph.FindMSTKruskal();
            selectedItem = null;
        }
        private void SalesmanPath()
        {
            ClearPath();
            _mode = Mode.Normal;
            if (selectedItem != null && selectedItem.GetType() == typeof(Node))
            {
                Node start = (Node)selectedItem;
                if (graph.FindSalesManPath(start, out int distance) == null)
                    notifier.ShowWarning("No Path found from selected node", opts);
                else notifier.ShowSuccess("Found a Path with distance of " + distance, opts);

            }
            else
            {
                notifier.ShowWarning("No Node is selected", opts);
            }
            selectedItem = null;
        }
        private void ColoringPressed()
        {
            ClearPath();
            _mode = Mode.Normal;
            graph.ColorGraph(out int count);
            notifier.ShowSuccess("Colored graph using " + count + " Colors");
        }

        private void AntColonyPressed()
        {
            ClearPath();
            _mode = Mode.Normal;
            graph.StartAntColony();

        }
        #endregion

        #region Mouse Events
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            EditMode = false;
            EdgeDrawMode = false;
            EdgeDrawHelper = null;

            if (e.ClickCount >= 2)      //edit item
            {
                if (e.OriginalSource.GetType() == typeof(Edge))
                {
                    Edge edge = (Edge)e.OriginalSource;
                    EditMode = true;
                    edge.IsEdit = true;
                    selectedItem = edge;
                    edge.Focusable = true;
                    Keyboard.Focus(edge);
                }
            }
            else if (e.OriginalSource.GetType() == typeof(Edge))    //select edge
            {
                Edge edge = (Edge)e.OriginalSource;
                edge.Selected = true;
                edge.Focusable = true;
                selectedItem = edge;
                Keyboard.Focus(edge);
            }
            else if (e.OriginalSource.GetType() == typeof(Node))    //select node
            {
                Node node = (Node)e.OriginalSource;

                if (PathFindMode)
                {
                    PathFindSecondSelected(node);
                    selectedItem = null;
                    e.Handled = true;
                    return;
                }

                selectedItem = node;        //Node drag start
                node.Selected = true;
                node.Focusable = true;
                Keyboard.Focus(node);

                dragOffset = e.GetPosition(node);
                dragOffset.X -= node.LocationX;
                dragOffset.Y -= node.LocationY;

                node.CaptureMouse();
                NodeDragMode = true;
            }
            e.Handled = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (NodeDragMode && selectedItem != null && selectedItem.GetType() == typeof(Node)) ((Node)selectedItem).ReleaseMouseCapture();
            EdgeDrawMode = false;
            EdgeDrawHelper = null;
            NodeDragMode = false;
            e.Handled = true;
        }


        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            EditMode = false;
            selectedItem = null;
            if (e.OriginalSource.GetType() == typeof(Node)) //Edge Draw start
            {
                Node start = (Node)e.OriginalSource;
                start.Selected = true;
                selectedItem = start;
                EdgeDrawMode = true;
                Point point = e.GetPosition(this);
                EdgeDrawHelper = new ArrowLine(start.X + Node.NodeWidth / 2, start.Y + Node.NodeHeight / 2, point.X, point.Y);
            }
            else
            {
                selectedItem = null;
                EdgeDrawMode = false;
                EdgeDrawHelper = null;
            }
            e.Handled = true;
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            EditMode = false;
            if (EdgeDrawMode) //Edge Draw end
            {
                Node start = (Node)selectedItem;
                if (e.OriginalSource.GetType() == typeof(Node))
                {
                    Node end = (Node)e.OriginalSource;
                    if (start != end)
                        if (AddEdge(start, end, 0))
                        {
                            if (selectedItem != null && selectedItem.GetType() == typeof(Edge))
                            {
                                Edge edge = (Edge)selectedItem;
                                if (edge.isDual != true)
                                {
                                    edge.BeginAnimation(ArrowLine.X1Property, Edge.CreateAnimation(edgeDrawHelper.X1, edge.X1));
                                    edge.BeginAnimation(ArrowLine.Y1Property, Edge.CreateAnimation(edgeDrawHelper.Y1, edge.Y1));
                                    edge.BeginAnimation(ArrowLine.X2Property, Edge.CreateAnimation(edgeDrawHelper.X2, edge.X2));
                                    edge.BeginAnimation(ArrowLine.Y2Property, Edge.CreateAnimation(edgeDrawHelper.Y2, edge.Y2));
                                }
                            }
                            EdgeDrawMode = false;
                            EdgeDrawHelper = null;
                            e.Handled = true;
                            return;
                        }
                }
                selectedItem = null;
            }
            EdgeDrawMode = false;
            EdgeDrawHelper = null;
            e.Handled = true;
        }
        public void MoveMouse(MouseEventArgs e)
        {
            this.OnMouseMove(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Pressed) return;
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (EdgeDrawMode)    //Edge Draw
                {
                    Point point = e.GetPosition(this);
                    Vector vector = new Point(edgeDrawHelper.X1, edgeDrawHelper.Y1) - point;
                    vector.Normalize();
                    vector *= 5;
                    point += vector;
                    edgeDrawHelper.X2 = point.X;
                    edgeDrawHelper.Y2 = point.Y;
                }
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (NodeDragMode)     //Node Drag
                {
                    Node node = (Node)selectedItem;
                    Point point = e.GetPosition(node);
                    node.LocationX = point.X - dragOffset.X;
                    node.LocationY = point.Y - dragOffset.Y;

                    double width = size.Width - Node.NodeWidth - padding * 2 - 2;
                    double height = size.Height - Node.NodeHeight - padding * 2 - 35;

                    if (node.LocationX < 0) node.LocationX = 0;
                    else if (node.LocationX > width) node.LocationX = width;
                    if (node.LocationY < 0) node.LocationY = 0;
                    else if (node.LocationY > height) node.LocationY = height;
                }
            }
            e.Handled = true;
        }
        #endregion

        #region KeyBoard Events
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (EditMode) EditModeKeyPress(e);
            else if (SystemKey.ControlPressed)
            {
                switch (e.Key)
                {
                    case Key.E:
                        EditKeyPressed();
                        break;
                    case Key.D:
                        if (SystemKey.ShiftPressed)
                            dupAllPressed();
                        else
                            dupPressed();
                        break;
                    case Key.P:
                        PrimsKeyPressed();
                        break;
                    case Key.K:
                        KruskalsKeyPressed();
                        break;
                    case Key.S:
                        SalesmanPath();
                        break;
                    case Key.C:
                        ClearPath();
                        break;
                }
            }
            else if (e.Key == Key.Delete)
                DeleteKeyPressed();

            e.Handled = true;
        }

        #endregion

        #region Base Class Overrides
        protected override IEnumerator LogicalChildren
        {
            get { return objs.GetEnumerator(); }
        }

        protected override int VisualChildrenCount
        {
            get { return objs.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (objs[index].GetType() == typeof(Edge))
                return (Edge)objs[index];
            else if (objs[index].GetType() == typeof(Node))
                return (Node)objs[index];
            else
                return (ArrowLine)objs[index];
        }

        protected override Size MeasureOverride(Size constraint)
        {
            foreach (object item in objs)
            {
                if (item.GetType() == typeof(Edge))
                    ((Edge)item).Measure(constraint);
                else if (item.GetType() == typeof(Node))
                    ((Node)item).Measure(constraint);
                else
                    ((ArrowLine)(item)).Measure(constraint);
            }

            if (Double.IsInfinity(constraint.Width) || Double.IsInfinity(constraint.Height))
                return new Size(10000, 10000);

            return constraint;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var bounds = VisualTreeHelper.GetDescendantBounds(base.AdornedElement);

            if (!bounds.IsEmpty)
                foreach (object item in objs)
                {
                    if (item.GetType() == typeof(Edge))
                        ((Edge)item).Arrange(bounds);
                    else if (item.GetType() == typeof(Node))
                        ((Node)item).Arrange(bounds);
                    else
                        ((ArrowLine)(item)).Arrange(bounds);
                }

            return finalSize;
        }

        #endregion
    }
}
