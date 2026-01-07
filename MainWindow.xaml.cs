using EmployeeWpfClient.ViewModels;
using System;
using System.Windows;

namespace EmployeeWpfClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (DataContext is IDisposable d) d.Dispose();
        }
    }
}
