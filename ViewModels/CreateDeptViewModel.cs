using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeWpfClient.ViewModels
{
    public class CreateDepartmentViewModel : ViewModelBase
    {
        private string _departmentName;
        public string DepartmentName
        {
            get => _departmentName;
            set
            {
                Set(ref _departmentName, value);
                CreateCommand.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand CreateCommand { get; }
        public RelayCommand CancelCommand { get; }

        public bool? DialogResult { get; private set; }

        public CreateDepartmentViewModel()
        {
            CreateCommand = new RelayCommand(_ => DialogResult = true,
                                             _ => !string.IsNullOrWhiteSpace(DepartmentName));

            CancelCommand = new RelayCommand(_ => DialogResult = false,_=>true);
        }
    }
}
