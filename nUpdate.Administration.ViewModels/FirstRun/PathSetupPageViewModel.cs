// Author: Dominic Beger (Trade/ProgTrade) 2017

using System;
using System.IO;
using System.Windows.Input;
using nUpdate.Administration.Common;

namespace nUpdate.Administration.ViewModels.FirstRun
{
    public class PathSetupPageViewModel : WizardPageViewModel
    {
        private readonly FirstRunViewModel _firstRunViewModel;

        private ICommand _applicationDataDirectorySelectCommand;

        private string _applicationDataLocation =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "nUpdate Administration");

        private string _defaultProjectDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "nUpdate Administration",
                "Projects");

        private ICommand _defaultProjectDirectorySelectCommand;

        public PathSetupPageViewModel(FirstRunViewModel firstRunViewModel,
            IFirstRunProvider firstRunProvider)
        {
            _firstRunViewModel = firstRunViewModel;
            CanGoBack = true;

            _applicationDataDirectorySelectCommand = new RelayCommand(() =>
                firstRunProvider.GetApplicationDataDirectoryCommandAction(ref _applicationDataLocation));
            _defaultProjectDirectorySelectCommand = new RelayCommand(() =>
                firstRunProvider.GetDefaultProjectDirectoryCommandAction(ref _defaultProjectDirectory));
            OnPropertyChanged(nameof(ApplicationDataLocation));
            OnPropertyChanged(nameof(DefaultProjectDirectory));

            // Refresh the navigation directly to enable the buttons and set the setup data.
            RefreshNavigation();
        }

        public string ApplicationDataLocation
        {
            get => _applicationDataLocation;
            set
            {
                SetProperty(value, ref _applicationDataLocation, nameof(ApplicationDataLocation));
                RefreshNavigation();
            }
        }

        public ICommand ApplicationDataSelectCommand
        {
            get => _applicationDataDirectorySelectCommand;
            set => SetProperty(value, ref _applicationDataDirectorySelectCommand, nameof(ApplicationDataSelectCommand));
        }

        public string DefaultProjectDirectory
        {
            get => _defaultProjectDirectory;
            set
            {
                SetProperty(value, ref _defaultProjectDirectory, nameof(DefaultProjectDirectory));
                RefreshNavigation();
            }
        }

        public ICommand DefaultProjectDirectorySelectCommand
        {
            get => _defaultProjectDirectorySelectCommand;
            set => SetProperty(value, ref _defaultProjectDirectorySelectCommand,
                nameof(DefaultProjectDirectorySelectCommand));
        }

        private void RefreshFirstRunData()
        {
            _firstRunViewModel.FirstSetupData.ApplicationDataLocation = ApplicationDataLocation;
            _firstRunViewModel.FirstSetupData.DefaultProjectDirectory = DefaultProjectDirectory;
        }

        private void RefreshNavigation()
        {
            CanGoForward = !string.IsNullOrEmpty(ApplicationDataLocation) &&
                           !string.IsNullOrEmpty(DefaultProjectDirectory);

            if (CanGoForward)
                RefreshFirstRunData();
        }
    }
}