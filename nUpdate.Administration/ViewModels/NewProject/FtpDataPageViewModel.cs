// Author: Dominic Beger (Trade/ProgTrade) 2017

using nUpdate.Administration.Ftp;
using nUpdate.Administration.Infrastructure;
using nUpdate.Administration.Views;
using nUpdate.Administration.Views.Dialogs;
using Starksoft.Aspen.Ftps;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class FtpDataPageViewModel : PageViewModel
    {
        private readonly NewProjectViewModel _newProjectViewModel;
        private readonly FtpData _transferData;
        private RelayCommand _directoryButtonCommand;
        private string _directory;
        private string _host;
        private string _password;
        private int _port = 21;
        private FtpsSecurityProtocol _protocol;
        private int _selectedModeIndex;
        private string _username;

        public FtpDataPageViewModel(NewProjectViewModel viewModel)
        {
            _newProjectViewModel = viewModel;
            _directoryButtonCommand = new RelayCommand(OnDirectoryButtonClick);
            _directory = "/";
            _transferData = new FtpData();

            PropertyChanged += (sender, args) => RefreshNavigation();
        }

        private void OnDirectoryButtonClick()
        {
            var ftpData = new FtpData
            {
                Host = Host,
                Port = Port,
                FtpSpecificProtocol = Protocol,
                UsePassiveMode = SelectedModeIndex == 0,
                Username = Username,
                Secret = Password
            };
            
            var ftpBrowseWindow = new FtpBrowseDialog(ftpData);
            var result = WindowManager.ShowDialog(ftpBrowseWindow);
            if (result.HasValue && result.Value)
                Directory = ftpBrowseWindow.Directory;
        }

        public string Directory
        {
            get => _directory;
            set => SetProperty(value, ref _directory, nameof(Directory));
        }

        public RelayCommand DirectoryButtonCommand
        {
            get => _directoryButtonCommand;
            set => SetProperty(value, ref _directoryButtonCommand, nameof(DirectoryButtonCommand));
        }

        public string Host
        {
            get => _host;
            set => SetProperty(value, ref _host, nameof(Host));
        }

        public string Password
        {
            get => _password;
            set => SetProperty(value, ref _password, nameof(Password));
        }

        public int Port
        {
            get => _port;
            set => SetProperty(value, ref _port, nameof(Port));
        }

        public FtpsSecurityProtocol Protocol
        {
            get => _protocol;
            set => SetProperty(value, ref _protocol, nameof(Protocol));
        }

        public int SelectedModeIndex
        {
            get => _selectedModeIndex;
            set => SetProperty(value, ref _selectedModeIndex, nameof(SelectedModeIndex));
        }

        public string Username
        {
            get => _username;
            set => SetProperty(value, ref _username, nameof(Username));
        }

        private void RefreshNavigation()
        {
            CanGoForward = !string.IsNullOrEmpty(Host) && !string.IsNullOrEmpty(Username) &&
                           !string.IsNullOrEmpty(Directory);

            // If the data is okay, we will set it directly.
            if (CanGoForward)
                RefreshProjectData();
        }

        private void RefreshProjectData()
        {
            _transferData.Host = Host;
            _transferData.Port = Port;
            _transferData.Username = Username;
            _transferData.UsePassiveMode = SelectedModeIndex == 0;
            _transferData.FtpSpecificProtocol = Protocol;
            _transferData.Directory = Directory;

            _newProjectViewModel.ProjectCreationData.Project.TransferData = _transferData;
        }
    }
}