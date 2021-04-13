using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GraphPlotter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private int _BusyCost;
        private int _BusyDiff;

        public event PropertyChangedEventHandler PropertyChanged;

        public int BusyCost
        {
            get { return _BusyCost; }
            set
            {
                BusyDiff = _BusyCost - value;
                _BusyCost = value;
                NotifyPropertyChanged();
            }
        }
        public int BusyDiff
        {
            get { return _BusyDiff; }
            set { _BusyDiff = value; NotifyPropertyChanged(); }
        }
        public MainWindow()
        {
            InitializeComponent();
            this.BusyCost = 10;
            this.DataContext = InitScreen.GetGraph();
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as Graph).Busy = BusyControl;
            (this.DataContext as Graph).window = this;
        }

        private void OnSizeChanged(object sender, RoutedEventArgs e)
        {
            EdgeAdorner.size = this.RenderSize;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void vahid(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MinimizeClick(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void ClearColorsClick(object sender, RoutedEventArgs e)
        {
            Graph graph = this.DataContext as Graph;
            if (graph != null) graph.ClearColors();
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            Graph graph = this.DataContext as Graph;
            if (graph == null) return;
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Binary Files(*.bin)|*.bin"
            };
            if (dialog.ShowDialog() == false) return;
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(dialog.FileName, FileMode.Create);
            formatter.Serialize(stream, graph);
        }

        private void LoadButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Binary Files(*.bin)|*.bin",
                Multiselect = false
            };
            if (dialog.ShowDialog() == false) return;
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(dialog.FileName, FileMode.OpenOrCreate);
            Graph graph = (Graph)formatter.Deserialize(stream);
            this.DataContext = graph;
            graph.Busy = BusyControl;
            graph.window = this;
        }

        private void NewButtonClick(object sender, RoutedEventArgs e)
        {
            this.DataContext = new Graph();
            (this.DataContext as Graph).Busy = BusyControl;
            (this.DataContext as Graph).window = this;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            mygraphview.Mode = Mode.MSTMode;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            mygraphview.Mode = Mode.SalesmanMode;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            mygraphview.Mode = Mode.PathMode;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            mygraphview.Mode = Mode.ColoringMode;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            mygraphview.Mode = Mode.AntColonyMode;
        }

        private void SpinnerStop(object sender, RoutedEventArgs e)
        {
            mygraphview.UnHalt = true;
        }
    }
}
