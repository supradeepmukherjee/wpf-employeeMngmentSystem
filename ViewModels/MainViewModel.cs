using EmployeeWpfClient.EmployeeServiceRef;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EmployeeWpfClient.ViewModels
{
    public class MainViewModel:ViewModelBase
    {
        public ObservableCollection<DepartmentDto> Departments { get; } = new ObservableCollection<DepartmentDto>();
        public ObservableCollection<EmployeeDto> EmployeesOfSelectedDept { get; } =new ObservableCollection<EmployeeDto>();

        private DepartmentDto _department;
        public DepartmentDto Department
        {
            get => _department;
            set
            {
                Set(ref _department, value);
                LoadEmployeesForSelectedDepartment();
                AddEmployeeCommand.RaiseCanExecuteChanged();
                AddDepartmentCommand.RaiseCanExecuteChanged();
            }
        }

        private string _newDepartment;
        public string NewDepartment
        {
            get => _newDepartment;
            set
            {
                Set(ref _newDepartment, value);
                AddDepartmentCommand.RaiseCanExecuteChanged();
            }
        }

        private EmployeeDto _employee;
        public EmployeeDto Employee
        {
            get => _employee;
            set
            {
                Set(ref _employee, value);
                DeleteEmployeeCommand.RaiseCanExecuteChanged();
                CreateProjectAndAssignCommand.RaiseCanExecuteChanged();
                RemoveAssignmentCommand.RaiseCanExecuteChanged();
            }
        }

        private string _employeeFirstName;
        public string EmployeeFirstName
        {
            get => _employeeFirstName;
            set
            {
                Set(ref _employeeFirstName, value);
                AddEmployeeCommand.RaiseCanExecuteChanged();
            }
        }

        private string _employeeLastName;
        public string EmployeeLastName
        {
            get => _employeeLastName;
            set
            {
                Set(ref _employeeLastName, value);
                AddEmployeeCommand.RaiseCanExecuteChanged();
            }
        }

        private string _employeeEmail;
        public string EmployeeEmail
        {
            get => _employeeEmail;
            set
            {
                Set(ref _employeeEmail, value);
                AddEmployeeCommand.RaiseCanExecuteChanged();
            }
        }

        private string _newProjectTitle;
        public string NewProjectTitle
        {
            get => _newProjectTitle;
            set
            {
                Set(ref _newProjectTitle, value);
                CreateProjectAndAssignCommand.RaiseCanExecuteChanged();
            }
        }

        private string _newProjectDescription;
        public string NewProjectDescription
        {
            get => _newProjectDescription;
            set
            {
                Set(ref _newProjectDescription, value);
                CreateProjectAndAssignCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand LoadCommand { get; }
        public RelayCommand AddDepartmentCommand { get; }
        public RelayCommand AddEmployeeCommand { get; }
        public RelayCommand DeleteEmployeeCommand { get; }
        public RelayCommand CreateProjectAndAssignCommand { get; }
        public RelayCommand RemoveAssignmentCommand { get; }

        public MainViewModel()
        {
            LoadCommand = new RelayCommand(async _ => await LoadAsync(), _ => true);
            AddDepartmentCommand = new RelayCommand(async _ => await AddDepartmentAsync(), _ => !string.IsNullOrWhiteSpace(NewDepartment));
            AddEmployeeCommand = new RelayCommand(async _ => await AddEmployeeAsync(), _ => Department != null && !string.IsNullOrWhiteSpace(EmployeeFirstName) && !string.IsNullOrWhiteSpace(EmployeeLastName) && !string.IsNullOrWhiteSpace(EmployeeEmail));
            DeleteEmployeeCommand = new RelayCommand(async _ => await DeleteEmployeeAsync(), _ => Employee != null);
            CreateProjectAndAssignCommand = new RelayCommand(async _ => await CreateProjectAndAssignAsync(), _ => Employee != null && !string.IsNullOrWhiteSpace(NewProjectTitle));
            RemoveAssignmentCommand = new RelayCommand(async _ => await RemoveAssignmentAsync(), _ => Employee != null);

            _ = LoadAsync();
        }

        public async Task LoadAsync()
        {
            Console.WriteLine("Loading...");
            try
            {
                Departments.Clear();
                using (var client = new EmployeeServiceClient())
                {
                    var depts = await client.GetAllDepartmentsAsync();
                    foreach (var d in depts)
                        Departments.Add(d);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
        }

        private void LoadEmployeesForSelectedDepartment()
        {
            EmployeesOfSelectedDept.Clear();
            if (Department == null) return;
            using (var client=new EmployeeServiceClient())
            {
                var dept = client.GetDepartmentWithEmployees(Department.DepartmentId);

                if (dept?.Employees == null)
                    return;

                foreach (var emp in dept.Employees)
                {
                    EmployeesOfSelectedDept.Add(emp);
                }
            }
        }

        public async Task AddDepartmentAsync()
        {
            try
            {
                var dto = new DepartmentDto { Name = NewDepartment };
                using (var client = new EmployeeServiceClient())
                {
                    var created = await client.CreateDepartmentAsync(dto);
                    Departments.Add(created);
                }
                NewDepartment= "";
                await LoadAsync();
            }
            catch (Exception ex) { MessageBox.Show($"Add dept failed: {ex.Message}"); }
        }

        public async Task AddEmployeeAsync()
        {
            if (Department == null) return;
            try
            {
                var emp = new EmployeeDto
                {
                    FirstName = EmployeeFirstName,
                    LastName = EmployeeLastName,
                    Email = EmployeeEmail,
                    DepartmentId = Department.DepartmentId
                };
                using (var client = new EmployeeServiceClient())
                {
                    var created = await client.CreateEmployeeAsync(emp);
                    EmployeesOfSelectedDept.Add(created);
                }
                EmployeeFirstName = "";
                EmployeeLastName = "";
                EmployeeEmail = "";

                LoadEmployeesForSelectedDepartment();
            }
            catch (Exception ex) { MessageBox.Show($"Add employee failed: {ex.Message}"); }
        }

        public async Task DeleteEmployeeAsync()
        {
            if (Employee == null) return;
            try
            {
                var id = Employee.EmployeeId;
                using (var client = new EmployeeServiceClient())
                {
                    var ok = await client.DeleteEmployeeAsync(id);
                    if (ok)
                    {
                        EmployeesOfSelectedDept.Remove(Employee);
                        Employee = null;
                    }
                    else MessageBox.Show("Delete failed");
                }
            }
            catch (Exception ex) { MessageBox.Show($"Delete employee failed: {ex.Message}"); }
        }

        public async Task CreateProjectAndAssignAsync()
        {
            if (Employee == null) return;
            try
            {
                using (var client = new EmployeeServiceClient())
                {
                    var created = await client.CreateProjectAsync(new ProjectDto { Title = NewProjectTitle, Description = NewProjectDescription });
                    var assigned = await client.AssignEmployeeToProjectAsync(Employee.EmployeeId, created.ProjectId);
                    MessageBox.Show($"Project created: {created.ProjectId}, assigned: {assigned}");
                    var emp = await client.GetEmployeeByIdAsync(Employee.EmployeeId);
                    Employee = emp;
                }
                NewProjectTitle = "";
                NewProjectDescription = "";

                LoadEmployeesForSelectedDepartment();
            }
            catch (Exception ex) { MessageBox.Show($"Create/Assign failed: {ex.Message}"); }
        }

        public async Task RemoveAssignmentAsync()
        {
            if (Employee == null) return;
            try
            {
                // For demo, remove first project if exists
                if (Employee.Projects == null || Employee.Projects.Length == 0)
                {
                    MessageBox.Show("No projects to remove");
                    return;
                }
                var projectId = Employee.Projects[0].ProjectId;
                using (var client = new EmployeeServiceClient())
                {
                    var ok = await client.RemoveEmployeeFromProjectAsync(Employee.EmployeeId, projectId);
                    MessageBox.Show("Removed? " + ok);
                    if (ok)
                    {
                        var emp = await client.GetEmployeeByIdAsync(Employee.EmployeeId);
                        Employee = emp;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"Remove assignment failed: {ex.Message}"); }
        }
    }
}
