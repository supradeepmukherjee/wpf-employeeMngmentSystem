using EmployeeWpfClient.EmployeeServiceRef;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EmployeeWpfClient.ViewModels
{
    public class EmployeesTabViewModel : TabViewModelBase
    {
        public ObservableCollection<EmployeeDto> Employees { get; } = new ObservableCollection<EmployeeDto>();

        private EmployeeDto _employee;
        public EmployeeDto Employee
        {
            get => _employee;
            set
            {
                Set(ref _employee, value);
                UpdateCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        public string NewFirstName { get; set; }
        public string NewLastName { get; set; }
        public string NewEmail { get; set; }

        public RelayCommand AddCommand { get; }
        public RelayCommand UpdateCommand { get; }
        public RelayCommand DeleteCommand { get; }

        public EmployeesTabViewModel()
        {
            Header = "Employees";
            AddCommand = new RelayCommand(_ => AddEmployee(), _ => !string.IsNullOrWhiteSpace(NewFirstName) && !string.IsNullOrWhiteSpace(NewLastName));
            UpdateCommand = new RelayCommand(_ => UpdateEmployee(), _ => Employee != null);
            DeleteCommand = new RelayCommand(_ => DeleteEmployee(), _ => Employee != null);
        }

        public override void Load()
        {
            Employees.Clear();
            try
            {
                using (var client = new EmployeeServiceClient())
                {
                    var list = client.GetAllEmployees();
                    foreach (var e in list) Employees.Add(e);
                }
            }
            catch (Exception ex) { MessageBox.Show("LoadEmployees failed: " + ex.Message); }
        }

        private void AddEmployee()
        {
            try
            {
                var dto = new EmployeeDto
                {
                    FirstName = NewFirstName,
                    LastName = NewLastName,
                    Email = NewEmail
                };
                using (var client = new EmployeeServiceClient())
                {
                    var created = client.CreateEmployee(dto);
                    Employees.Add(created);
                }
                NewFirstName = NewLastName = NewEmail = string.Empty;
                NotifyPropertyChanged(nameof(NewFirstName));
                NotifyPropertyChanged(nameof(NewLastName));
                NotifyPropertyChanged(nameof(NewEmail));
            }
            catch (Exception ex) { MessageBox.Show("AddEmployee failed: " + ex.Message); }
        }

        private void UpdateEmployee()
        {
            try
            {
                using (var client = new EmployeeServiceClient())
                {
                    var updated = client.UpdateEmployee(Employee);
                    Load();
                }
            }
            catch (Exception ex) { MessageBox.Show("UpdateEmployee failed: " + ex.Message); }
        }

        private void DeleteEmployee()
        {
            try
            {
                using (var client = new EmployeeServiceClient())
                {
                    var ok = client.DeleteEmployee(Employee.EmployeeId);
                    if (ok) Employees.Remove(Employee);
                }
            }
            catch (Exception ex) { MessageBox.Show("DeleteEmployee failed: " + ex.Message); }
        }
    }
}
