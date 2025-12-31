using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EmployeeWpfClient.ViewModels
{
    public class ExploreTabViewModel : TabViewModelBase
    {
        private readonly Action<string> _openTabCallback;

        public RelayCommand OpenTileCommand { get; }

        public ExploreTabViewModel(Action<string> openTabCallback)
        {
            Header = "Explore";
            _openTabCallback = openTabCallback ?? throw new ArgumentNullException(nameof(openTabCallback));
            OpenTileCommand = new RelayCommand(p => OnOpenTile(p as string), _ => true);
        }

        private void OnOpenTile(string key) { if (!string.IsNullOrWhiteSpace(key)) _openTabCallback(key); }

        public override void Load() { }

        public override bool IsClosable => false;
    }
}