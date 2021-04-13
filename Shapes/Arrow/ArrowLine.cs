//------------------------------------------
// ArrowLine.cs (c) 2007 by Charles Petzold
//------------------------------------------
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace GraphPlotter
{
    /// <summary>
    ///     Draws a straight line between two points with 
    ///     optional arrows on the ends.
    /// </summary>
    public class ArrowLine : ArrowLineBase
    {
        public ArrowLine(double X1, double Y1, double X2, double Y2)
        {
            this.X1 = X1;
            this.Y1 = Y1;
            this.X2 = X2;
            this.Y2 = Y2;
            this.Opacity = 0.7;
            this.Stroke= new SolidColorBrush(Colors.Maroon);
            this.StrokeThickness = 2;
        }
        public ArrowLine() { }

        private static readonly FontFamily font = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Copse");
        FormattedText formattedText;
        /// <summary>
        ///     Identifies the X1 dependency property.
        /// </summary>
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1",
                typeof(double), typeof(ArrowLine),
                new FrameworkPropertyMetadata(0.0,
                        FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        ///     Gets or sets the x-coordinate of the ArrowLine start point.
        /// </summary>
        public double X1
        {
            set { SetValue(X1Property, value); }
            get { return (double)GetValue(X1Property); }
        }

        /// <summary>
        ///     Identifies the Y1 dependency property.
        /// </summary>
        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1",
                typeof(double), typeof(ArrowLine),
                new FrameworkPropertyMetadata(0.0,
                        FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        ///     Gets or sets the y-coordinate of the ArrowLine start point.
        /// </summary>
        public double Y1
        {
            set { SetValue(Y1Property, value); }
            get { return (double)GetValue(Y1Property); }
        }

        /// <summary>
        ///     Identifies the X2 dependency property.
        /// </summary>
        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2",
                typeof(double), typeof(ArrowLine),
                new FrameworkPropertyMetadata(0.0,
                        FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        ///     Gets or sets the x-coordinate of the ArrowLine end point.
        /// </summary>
        public double X2
        {
            set { SetValue(X2Property, value); }
            get { return (double)GetValue(X2Property); }
        }

        /// <summary>
        ///     Identifies the Y2 dependency property.
        /// </summary>
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2",
                typeof(double), typeof(ArrowLine),
                new FrameworkPropertyMetadata(0.0,
                        FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        ///     Gets or sets the y-coordinate of the ArrowLine end point.
        /// </summary>
        public double Y2
        {
            set { SetValue(Y2Property, value); }
            get { return (double)GetValue(Y2Property); }
        }

        public static readonly DependencyProperty isSelectedProperty =
            DependencyProperty.Register("isSelected",
                typeof(bool), typeof(ArrowLine),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public bool isSelected
        {
            get { return (bool)GetValue(isSelectedProperty); }
            set { SetValue(isSelectedProperty, value); }
        }

        public static readonly DependencyProperty ArrowTextProperty =
            DependencyProperty.Register("ArrowText",
                typeof(string), typeof(ArrowLineBase),
                new FrameworkPropertyMetadata("",
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public string Length
        {
            set
            {
                SetValue(ArrowTextProperty, value);
                SetText(value.ToString());
            }
            get { return (string)GetValue(ArrowTextProperty); }
        }

        /// <summary>
        ///     Identifies the TextFill dependency property.
        /// </summary>
        public static readonly DependencyProperty TextFillProperty =
            DependencyProperty.Register("TextFill",
                typeof(SolidColorBrush), typeof(ArrowLine),
                new FrameworkPropertyMetadata(new SolidColorBrush(),
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        ///     Gets or sets the Text Fill color on the ArrowLine
        /// </summary>
        public SolidColorBrush TextFill
        {
            set { SetValue(TextFillProperty, value); SetText(Length); }
            get { return (SolidColorBrush)GetValue(TextFillProperty); }
        }


        /// <summary>
        ///     Gets or sets Edit mode for this Edge
        /// </summary>
        public bool IsEdit
        {
            get { return (bool)GetValue(IsEditProperty); }
            set { SetValue(IsEditProperty, value); }
        }

        public static readonly DependencyProperty IsEditProperty =
            DependencyProperty.Register("IsEdit"
                , typeof(bool), typeof(ArrowLine),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.AffectsRender));




        private void SetText(string text)
        {
            formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(
                    font, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal), 16, TextFill);
            formattedText.TextAlignment = TextAlignment.Center;   //TODO => make a decision
        }

        /// <summary>
        ///     Gets a value that represents the Geometry of the ArrowLine.
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get
            {
                // Clear out the PathGeometry.
                pathgeo.Figures.Clear();

                // Define a single PathFigure with the points.
                pathfigLine.StartPoint = new Point(X1, Y1);
                polysegLine.Points.Clear();
                polysegLine.Points.Add(new Point(X2, Y2));
                //polysegLine.
                pathgeo.Figures.Add(pathfigLine);

                return base.DefiningGeometry;
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            Point start = new Point(X1, Y1);
            Point end = new Point(X2, Y2);

            Vector vector = end - start;

            Point mid = start + vector / 2;

            double angle = Math.Atan(vector.Y / vector.X) * (180 / Math.PI);
            if (start.X > end.X) angle += 180;

            Transform transform;

            if (start.X > end.X)
            {
                vector = new Vector(-vector.Y, vector.X);
                vector.Normalize();
                vector *= 10;
                Point TextMid = mid + vector;
                transform = new RotateTransform(180, TextMid.X, TextMid.Y);
                drawingContext.PushTransform(transform);
            }

            transform = new RotateTransform(angle, mid.X, mid.Y);
            drawingContext.PushTransform(transform);

            if (IsEdit)
            {
                Size size = new Size(formattedText.Width + 5, formattedText.Height - 6);
                Point loc = new Point(mid.X - size.Width / 2, mid.Y + 3);
                drawingContext.DrawRoundedRectangle(Brushes.Transparent, new Pen(TextFill, 1), new Rect(loc, size), 3, 3);
            }

            drawingContext.DrawText(formattedText, mid);
        }

    }
}
