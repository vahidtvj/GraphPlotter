using System;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Globalization;

namespace GraphPlotter
{
    public class NodeShape : Shape
    {
        private static readonly FontFamily font = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Copse");
        private FormattedText formattedText;

        /// <summary>
        ///     Identifies the X dependency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X",
                typeof(double), typeof(NodeShape),
                new FrameworkPropertyMetadata(0.0,
                        FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        ///     Gets or sets the x-coordinate of the NodeShape start point.
        /// </summary>
        public double X
        {
            set { SetValue(XProperty, value); }
            get { return (double)GetValue(XProperty); }
        }

        /// <summary>
        ///     Identifies the Y1 dependency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y",
                typeof(double), typeof(NodeShape),
                new FrameworkPropertyMetadata(0.0,
                        FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        ///     Gets or sets the Y-coordinate of the NodeShape start point.
        /// </summary>
        public double Y
        {
            set { SetValue(YProperty, value); }
            get { return (double)GetValue(YProperty); }
        }

        /// <summary>
        ///     Identifies the _Name dependency property.
        /// </summary>
        public static readonly DependencyProperty _NameProperty =
            DependencyProperty.Register("Name",
                typeof(string), typeof(NodeShape),
                new FrameworkPropertyMetadata("",
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        ///     Gets or sets the text on the Nodeshape
        /// </summary>
        public string _Name
        {
            set
            {
                SetValue(_NameProperty, value);
                SetText(value);
            }
            get { return (string)GetValue(_NameProperty); }
        }

        /// <summary>
        ///     Identifies the TextFill dependency property.
        /// </summary>
        public static readonly DependencyProperty TextFillProperty =
            DependencyProperty.Register("TextFill",
                typeof(SolidColorBrush), typeof(NodeShape),
                new FrameworkPropertyMetadata(new SolidColorBrush(),
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        ///     Gets or sets the Text Fill color on the Nodeshape
        /// </summary>
        public SolidColorBrush TextFill
        {
            set { SetValue(TextFillProperty, value); }
            get { return (SolidColorBrush)GetValue(TextFillProperty); }
        }


        /// <summary>
        ///     Identifies the Radius dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius",
                typeof(double), typeof(NodeShape),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        ///     Gets or sets the Radius of the Nodeshape
        /// </summary>
        public double Radius
        {
            set
            {
                SetValue(RadiusProperty, value);
                SetText(_Name);
            }
            get { return (double)GetValue(RadiusProperty); }
        }


        private void SetText(string text)
        {
            formattedText = new FormattedText(_Name, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(
              font,
              FontStyles.Normal,
              FontWeights.Normal,
              new FontStretch()), 16, TextFill);
            formattedText.MaxTextWidth = 40;
            formattedText.MaxTextHeight = 40;
            formattedText.TextAlignment = TextAlignment.Center;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawText(formattedText, new Point(X, Y + 10));
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                RectangleGeometry rect = new RectangleGeometry(new Rect(new Point(X, Y), new Size(40, 40)));
                rect.RadiusX = Radius;
                rect.RadiusY = Radius;
                return rect;
            }
        }
    }
}
