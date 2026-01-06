using EmployeeWpfClient.EmployeeServiceRef;
using EmployeeWpfClient.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EmployeeWpfClient.ViewModels
{
    public class DepartmentsTabViewModel : TabViewModelBase
    {
        public ObservableCollection<DepartmentDto> Departments { get; } = new ObservableCollection<DepartmentDto>();
        private DepartmentDto _department;
        public DepartmentDto Department
        {
            get => _department;
            set
            {
                Set(ref _department,value);
                UpdateCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        public string NewDepartmentName { get; set; }

        public RelayCommand AddCommand { get; }
        public RelayCommand UpdateCommand { get; }
        public RelayCommand DeleteCommand { get; }

        public DepartmentsTabViewModel()
        {
            Header = "Departments";
            AddCommand = new RelayCommand(_ => Add(), _ => true);
            UpdateCommand = new RelayCommand(_ => Update(), _ => Department != null);
            DeleteCommand = new RelayCommand(_ => Delete(), _ => Department != null);
        }

        public override void Load()
        {
            Departments.Clear();
            try
            {
                using (var client = new EmployeeServiceClient())
                {
                    var list = client.GetAllDepartments();
                    foreach (var d in list) Departments.Add(d);
                }
            }
            catch (Exception ex) { MessageBox.Show("LoadDepartments failed: " + ex.Message); }
        }

        private void Add()
        {
            try
            {
                var vm = new CreateDepartmentViewModel();
                var win = new CreateDeptWindow
                {
                    DataContext = vm,
                    Owner = Application.Current.MainWindow
                };

                if (win.ShowDialog() == true)
                {
                        using (var client = new EmployeeServiceClient())
                        {
                            var created = client.CreateDepartment(
                                new DepartmentDto { Name = vm.DepartmentName });

                            Departments.Add(created);
                            win.Close();
                        }
                }
            }
            catch (Exception ex) { MessageBox.Show("AddDepartment failed: " + ex.Message); }
        }

        private void Update()
        {
            try
            {
                using (var client = new EmployeeServiceClient())
                {
                    var updated = client.UpdateDepartment(Department);
                    Load();
                }
            }
            catch (Exception ex) { MessageBox.Show("UpdateDepartment failed: " + ex.Message); }
        }

        private void Delete()
        {
            if (Department == null) return;

            var msg = $"Are you sure you want to delete department '{Department.Name}'?";
            var answer = MessageBox.Show(msg, "Confirm delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (answer != MessageBoxResult.Yes) return;

            try
            {
                using (var client = new EmployeeServiceClient())
                {
                    var deleted = client.DeleteDepartment(Department.DepartmentId);
                    if (deleted) Departments.Remove(Department);
                    else MessageBox.Show("Delete failed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("DeleteDepartment failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
