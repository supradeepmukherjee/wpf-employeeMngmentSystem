using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EmployeeWpfClient.Views
{
    public partial class CreateDeptWindow : Window
    {
        public CreateDeptWindow()
        {
            InitializeComponent();
        }
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
