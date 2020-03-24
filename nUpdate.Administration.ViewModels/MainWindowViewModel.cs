using System.Diagnostics;
using System.Windows.Input;
using nUpdate.Administration.Infrastructure;

// ReSharper disable MemberCanBeMadeStatic.Local

namespace nUpdate.Administration.ViewModels
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private ICommand _loadCommand;
        private ICommand _newProjectCommand;
        private bool _canEditMasterPassword;

        public MainWindowViewModel(IMainViewActionProvider mainWindowActionProvider)
        {
            LoadCommand = new RelayCommand(o => mainWindowActionProvider.Load());
            NewProjectCommand = new RelayCommand(o => mainWindowActionProvider.CreateNewProject());
            CanEditMasterPassword = mainWindowActionProvider.CanEditMasterPassword();
        }

        public ICommand LoadCommand
        {
            get => _loadCommand;
            set => SetProperty(value, ref _loadCommand);
        }

        public ICommand NewProjectCommand
        {
            get => _newProjectCommand;
            set => SetProperty(value, ref _newProjectCommand);
        }

        public bool CanEditMasterPassword
        {
            get => _canEditMasterPassword;
            set => SetProperty(value, ref _canEditMasterPassword);
        }
    }
}