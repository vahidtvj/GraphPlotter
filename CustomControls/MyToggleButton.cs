using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace GraphPlotter
{
    public class MyToggleButton : ToggleButton
    {


        public bool IsMouseOverEx
        {
            get { return (bool)GetValue(IsMouseOverExProperty); }
            set { SetValue(IsMouseOverExProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMouseOverEx.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMouseOverExProperty =
            DependencyProperty.Register("IsMouseOverEx", typeof(bool), typeof(MyToggleButton), new FrameworkPropertyMetadata(false));

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            //IsMouseOverEx = (Visibility == Visibility.Visible) ? false : true;
        }
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.Visibility = Visibility.Hidden;
            IsMouseOverEx = true;
        }
    }
}
