using PolygonClient.ViewModels;
using System.Windows;

namespace PolygonClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}
