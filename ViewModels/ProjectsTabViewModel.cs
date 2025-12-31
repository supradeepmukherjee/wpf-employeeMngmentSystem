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
    public class ProjectsTabViewModel : TabViewModelBase
    {
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

        public ProjectsTabViewModel()
        {
            Header = "Projects";
            AddCommand = new RelayCommand(_ => Add(), _ => !string.IsNullOrWhiteSpace(NewProjectTitle));
            UpdateCommand = new RelayCommand(_ => Update(), _ => Project != null);
            DeleteCommand = new RelayCommand(_ => Delete(), _ => Project != null);
        }

        public override void Load()
        {
            Projects.Clear();
            try
            {
                using (var client = new EmployeeServiceClient())
                {
                    var list = client.GetAllProjects();
                    foreach (var p in list) Projects.Add(p);
                }
            }
            catch (Exception ex) { MessageBox.Show("LoadProjects failed: " + ex.Message); }
        }

        private void Add()
        {
            try
            {
                using (var client = new EmployeeServiceClient())
                {
                    var created = client.CreateProject(new ProjectDto { Title = NewProjectTitle, Description = NewProjectDescription });
                    Projects.Add(created);
                }
                NewProjectTitle = NewProjectDescription = string.Empty;
                NotifyPropertyChanged(nameof(NewProjectTitle));
                NotifyPropertyChanged(nameof(NewProjectDescription));
            }
            catch (Exception ex) { MessageBox.Show("AddProject failed: " + ex.Message); }
        }

        private void Update()
        {
            try
            {
                using (var client = new EmployeeServiceClient())
                {
                    var updated = client.UpdateProject(Project);
                    Load();
                }
            }
            catch (Exception ex) { MessageBox.Show("UpdateProject failed: " + ex.Message); }
        }

        private void Delete()
        {
            try
            {
                using (var client = new EmployeeServiceClient())
                {
                    var deleted = client.DeleteProject(Project.ProjectId);
                    if (deleted) Projects.Remove(Project);
                }
            }
            catch (Exception ex) { MessageBox.Show("DeleteProject failed: " + ex.Message); }
        }
    }
}
