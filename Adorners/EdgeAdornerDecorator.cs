using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace GraphPlotter
{
    public class EdgeAdornerDecorator : AdornerDecorator
    {
        #region Constructor
        public EdgeAdornerDecorator()
        {
            base.Loaded += this.OnLoaded;
        }
        #endregion

        #region OnLoaded
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            var layer = AdornerLayer.GetAdornerLayer(this);
            if (layer == null)
                return;
            _adorner = new EdgeAdorner(this);
            layer.Add(_adorner);
            this.GiveGraphToAdorner();
            this.Focusable = true;
            Keyboard.Focus(this);
        }
        #endregion

        #region Graph (DP)

        public Graph Graph
        {
            get { return (Graph)GetValue(GraphProperty); }
            set { SetValue(GraphProperty, value); }
        }

        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register(
            "Graph",
            typeof(Graph),
            typeof(EdgeAdornerDecorator),
            new UIPropertyMetadata(null, OnGraphChanged));

        static void OnGraphChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as EdgeAdornerDecorator).GiveGraphToAdorner();
        }

        #endregion // Graph (DP)

        #region Fields

        EdgeAdorner _adorner;

        #endregion 

        #region Private Helpers

        void GiveGraphToAdorner()
        {
            if (_adorner != null && this.Graph != null)
            {
                _adorner.Graph = this.Graph;
            }
        }

        #endregion

        #region Mouse Events
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            _adorner.EditMode = false;
            _adorner.selectedItem = null;
            _adorner.EdgeDrawMode = false;
            _adorner.EdgeDrawHelper = null;
            _adorner.AddNode(e.GetPosition(this));
        }
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
            _adorner.EditMode = false;
            _adorner.selectedItem = null;
            _adorner.EdgeDrawMode = false;
            _adorner.EdgeDrawHelper = null;
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            _adorner.EditMode = false;
            _adorner.selectedItem = null;
            _adorner.EdgeDrawMode = false;
            _adorner.EdgeDrawHelper = null;
            Keyboard.Focus(this);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_adorner.EdgeDrawMode) _adorner.MoveMouse(e);
        }
        #endregion

        #region KeyEvents
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.D && SystemKey.ControlPressed && SystemKey.ShiftPressed) _adorner.DuplicateAll();
        }
        #endregion

        #region Properties
        public Mode Mode
        {
            get { return _adorner.mode; }
            set { _adorner.mode = value; }
        }
        public bool UnHalt
        {
            get { return _adorner.UnHalt; }
            set { _adorner.UnHalt = value; }
        }
        #endregion
    }
}
