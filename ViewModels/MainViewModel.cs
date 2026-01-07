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
    public class MainViewModel:ViewModelBase,IDisposable
    {
        private readonly EmployeeServiceClient _client;

        public ObservableCollection<TabViewModelBase> OpenTabs { get; } = new ObservableCollection<TabViewModelBase>();

        private TabViewModelBase _selectedTab;
        public TabViewModelBase SelectedTab
        {
            get => _selectedTab;
            set { Set(ref _selectedTab, value); }
        }

        public RelayCommand TileClickCommand { get; }
        public RelayCommand CloseTabCommand { get; }

        public MainViewModel()
        {
            _client = new EmployeeServiceClient();

            CloseTabCommand = new RelayCommand(param => CloseTab(param as TabViewModelBase), param => CanCloseTab(param as TabViewModelBase));
            var explore = new ExploreTabViewModel(OnTileRequested);
            OpenTabs.Add(explore);
            SelectedTab = explore;
        }

        private void OnTileRequested(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return;

            TabViewModelBase tab = null;

            if (key == "Employees")
            {
                tab = OpenTabs.OfType<EmployeesTabViewModel>().FirstOrDefault();
                if (tab == null)
                {
                    var t = new EmployeesTabViewModel(_client);
                    OpenTabs.Add(t);
                    t.Load();
                    tab = t;
                }
            }
            else if (key == "Departments")
            {
                tab = OpenTabs.OfType<DepartmentsTabViewModel>().FirstOrDefault();
                if (tab == null)
                {
                    var t = new DepartmentsTabViewModel(_client);
                    OpenTabs.Add(t);
                    t.Load();
                    tab = t;
                }
            }
            else if (key == "Projects")
            {
                tab = OpenTabs.OfType<ProjectsTabViewModel>().FirstOrDefault();
                if (tab == null)
                {
                    var t = new ProjectsTabViewModel(_client);
                    OpenTabs.Add(t);
                    t.Load();
                    tab = t;
                }
            }

            if (tab != null)SelectedTab = tab;
        }

        private bool CanCloseTab(TabViewModelBase tab)=>tab != null && tab.IsClosable;

        private void CloseTab(TabViewModelBase tab)
        {
            if (tab == null) return;

            if (!tab.IsClosable) return;

            int index = OpenTabs.IndexOf(tab);
            if (index < 0) return;

            OpenTabs.Remove(tab);

            if (OpenTabs.Count == 0)
            {
                SelectedTab = null;
                return;
            }

            int newIndex = Math.Min(index, OpenTabs.Count - 1);
            SelectedTab = OpenTabs[newIndex];
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
