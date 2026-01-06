using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeWpfClient.ViewModels
{
    public class CreateEmpViewModel : ViewModelBase
    {
        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set
            {
                Set(ref _firstName, value);
                CreateCommand.RaiseCanExecuteChanged();
            }
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set
            {
                Set(ref _lastName, value);
                CreateCommand.RaiseCanExecuteChanged();
            }
        }
        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                Set(ref _email, value);
                CreateCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand CreateCommand { get; }
        public RelayCommand CancelCommand { get; }

        public CreateEmpViewModel()
        {
            CreateCommand = new RelayCommand(_ => DialogResult = true,
                _ => !string.IsNullOrWhiteSpace(FirstName)
                  && !string.IsNullOrWhiteSpace(LastName));

            CancelCommand = new RelayCommand(_ => DialogResult = false,_=>true);
        }

        public bool? DialogResult { get; private set; }
    }
}
