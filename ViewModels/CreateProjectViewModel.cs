using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeWpfClient.ViewModels
{
    public class CreateProjectViewModel : ViewModelBase
    {
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                NotifyPropertyChanged();
                CreateCommand.RaiseCanExecuteChanged();
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                Set(ref _description, value);
                CreateCommand.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand CreateCommand { get; }
        public RelayCommand CancelCommand { get; }

        public CreateProjectViewModel()
        {
            CreateCommand = new RelayCommand(_ => DialogResult = true,
                _ => !string.IsNullOrWhiteSpace(Title));

            CancelCommand = new RelayCommand(_ => DialogResult = false, _ => true);
        }

        public bool? DialogResult { get; private set; }
    }
}