using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GraphPlotter.Forms
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        public int Value;
        public InputDialog(String Text, int Value)
        {
            InitializeComponent();
            Point point = Application.Current.MainWindow.PointToScreen(Mouse.GetPosition(Application.Current.MainWindow));
            this.Left = point.X - this.Width / 2;
            this.Top = point.Y - this.Height / 2;
            this.WindowStartupLocation = WindowStartupLocation.Manual;

            Label.Text = "Distance between Node " + Text + " is:";
            this.InputText.Text = Value.ToString();
            this.Value = Value;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(InputText.Text, out int result))
            {
                this.Value = result;
                this.DialogResult = true;
                this.Close();
            }
            InputText.BorderBrush = Brushes.Red;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) this.DragMove();
        }

        private void InputText_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            InputText.Text = "";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) Button_Click_1(sender, new RoutedEventArgs());
        }
    }
}
