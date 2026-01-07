using EmployeeWpfClient.EmployeeServiceRef;
using EmployeeWpfClient.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace EmployeeWpfClient.ViewModels
{
    public class EmployeesTabViewModel : TabViewModelBase
    {
        private readonly EmployeeServiceClient _client;
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
                UploadPhotoCommand.RaiseCanExecuteChanged();

                if(_employee != null) LoadEmployeePhoto(_employee.EmployeeId);
                else EmployeePhoto=null;
            }
        }

        private BitmapImage _employeePhoto;
        public BitmapImage EmployeePhoto
        {
            get => _employeePhoto;
            set => Set(ref _employeePhoto, value);
        }

        public string NewFirstName { get; set; }
        public string NewLastName { get; set; }
        public string NewEmail { get; set; }

        public RelayCommand AddCommand { get; }
        public RelayCommand UpdateCommand { get; }
        public RelayCommand DeleteCommand { get; }

        public RelayCommand UploadPhotoCommand { get; }

        public EmployeesTabViewModel(EmployeeServiceClient client)
        {
            Header = "Employees";
            AddCommand = new RelayCommand(_ => AddEmployee(), _ => true);
            UpdateCommand = new RelayCommand(_ => UpdateEmployee(), _ => Employee != null);
            DeleteCommand = new RelayCommand(_ => DeleteEmployee(), _ => Employee != null);

            UploadPhotoCommand = new RelayCommand(_ => UploadPhoto(), _ => Employee != null);
            _client = client;
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
                var vm = new CreateEmpViewModel();
                var win = new CreateEmpWindow
                {
                    DataContext = vm,
                    Owner = Application.Current.MainWindow
                };

                if (win.ShowDialog() == true)
                {
                    using (var client = new EmployeeServiceClient())
                    {
                        var created = client.CreateEmployee(new EmployeeDto
                        {
                            FirstName = vm.FirstName,
                            LastName = vm.LastName,
                            Email = vm.Email
                        });

                        Employees.Add(created);
                    }
                }
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
            if (Employee == null) return;

            var displayName = string.IsNullOrWhiteSpace($"{Employee.FirstName} {Employee.LastName}".Trim())
                              ? $"(Id: {Employee.EmployeeId})"
                              : $"{Employee.FirstName} {Employee.LastName}";

            var msg = $"Are you sure you want to delete employee '{displayName}'?";
            var answer = MessageBox.Show(msg, "Confirm delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (answer != MessageBoxResult.Yes) return;

            try
            {
                using (var client = new EmployeeServiceClient())
                {
                    var ok = client.DeleteEmployee(Employee.EmployeeId);
                    if (ok) Employees.Remove(Employee);
                    else MessageBox.Show("Delete failed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("DeleteEmployee failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UploadPhoto()
        {
            if (Employee == null) return;

            var dlg = new OpenFileDialog
            {
                Title = "Select profile photo",
                Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                CheckFileExists = true
            };

            if (dlg.ShowDialog() != true) return;

            var path = dlg.FileName;
            try
            {
                var bytes = File.ReadAllBytes(path);
                using (var client = new EmployeeServiceClient())
                {
                    var ok = client.UploadEmployeePhoto(Employee.EmployeeId, bytes, Path.GetFileName(path));
                    if (ok)LoadEmployeePhoto(Employee.EmployeeId);
                    else MessageBox.Show("Upload failed.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Upload error: " + ex.Message);
            }
        }

        public void LoadEmployeePhoto(int employeeId)
        {
            try
            {
                using (var client = new EmployeeServiceClient())
                {
                    var bytes = client.GetEmployeePhoto(employeeId);
                    if (bytes == null || bytes.Length == 0)
                    {
                        EmployeePhoto = null;
                        return;
                    }

                    var img = new BitmapImage();
                    using (var ms = new MemoryStream(bytes))
                    {
                        img.BeginInit();
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.StreamSource = ms;
                        img.EndInit();
                    }
                    EmployeePhoto = img;
                }
            }
            catch (Exception ex)
            {
                EmployeePhoto = null;
                MessageBox.Show("Could not load photo: " + ex.Message);
            }
        }
    }
}
