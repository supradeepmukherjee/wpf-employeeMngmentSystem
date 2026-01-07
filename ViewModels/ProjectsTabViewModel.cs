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
    public class ProjectsTabViewModel : TabViewModelBase
    {
        private readonly EmployeeServiceClient _client;
        public ObservableCollection<ProjectDto> Projects { get; } = new ObservableCollection<ProjectDto>();
        private ProjectDto _project;
        public ProjectDto Project
        {
            get => _project;
            set
            {
                Set(ref _project, value);
                UpdateCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        public string NewProjectTitle { get; set; }
        public string NewProjectDescription { get; set; }

        public RelayCommand AddCommand { get; }
        public RelayCommand UpdateCommand { get; }
        public RelayCommand DeleteCommand { get; }

        public ProjectsTabViewModel(EmployeeServiceClient client)
        {
            Header = "Projects";
            AddCommand = new RelayCommand(_ => Add(), _ => true);
            UpdateCommand = new RelayCommand(_ => Update(), _ => Project != null);
            DeleteCommand = new RelayCommand(_ => Delete(), _ => Project != null);
            _client = client;
        }

        public override void Load()
        {
            Projects.Clear();
            try
            {
                var list = _client.GetAllProjects();
                foreach (var p in list) Projects.Add(p);

            }
            catch (Exception ex) { MessageBox.Show("LoadProjects failed: " + ex.Message); }
        }

        private void Add()
        {
            try
            {
                var vm = new CreateProjectViewModel();
                var win = new CreateProjectWindow
                {
                    DataContext = vm,
                    Owner = Application.Current.MainWindow
                };

                if (win.ShowDialog() == true)
                {
                    var created = _client.CreateProject(new ProjectDto
                    {
                        Title = vm.Title,
                        Description = vm.Description
                    });

                    Projects.Add(created);

                }
            }
            catch (Exception ex) { MessageBox.Show("AddProject failed: " + ex.Message); }
        }

        private void Update()
        {
            try
            {
                var updated = _client.UpdateProject(Project);
                Load();

            }
            catch (Exception ex) { MessageBox.Show("UpdateProject failed: " + ex.Message); }
        }

        private void Delete()
        {
            if (Project == null) return;

            var title = string.IsNullOrWhiteSpace(Project.Title) ? $"(Id: {Project.ProjectId})" : Project.Title;
            var msg = $"Are you sure you want to delete project '{title}'?";
            var answer = MessageBox.Show(msg, "Confirm delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (answer != MessageBoxResult.Yes) return;

            try
            {
                var deleted = _client.DeleteProject(Project.ProjectId);
                if (deleted) Projects.Remove(Project);
                else MessageBox.Show("Delete failed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            catch (Exception ex)
            {
                MessageBox.Show("DeleteProject failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}