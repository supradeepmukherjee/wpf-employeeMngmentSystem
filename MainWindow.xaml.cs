using EmployeeWpfClient.ViewModels;
using System.Windows;

namespace EmployeeWpfClient
{
    public partial class MainWindow:Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
