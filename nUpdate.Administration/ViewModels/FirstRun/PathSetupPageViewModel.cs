// Author: Dominic Beger (Trade/ProgTrade) 2017

using System;
using System.IO;
using System.Windows.Input;
using nUpdate.Administration.Infrastructure;
using WPFFolderBrowser;

namespace nUpdate.Administration.ViewModels.FirstRun
{
    public class PathSetupPageViewModel : PageViewModel
    {
        private readonly FirstRunViewModel _firstRunViewModel;

        private string _applicationDataLocation =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "nUpdate Administration");

        private string _defaultProjectDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "nUpdate Administration",
                "Projects");

        private ICommand _applicationDataSelectCommand;
        private ICommand _defaultProjectDirectorySelectCommand;

        public PathSetupPageViewModel(FirstRunViewModel firstRunViewModel)
        {
            _firstRunViewModel = firstRunViewModel;
            CanGoBack = true;

            _applicationDataSelectCommand = new RelayCommand(() =>
            {
                var browseDialog = new WPFFolderBrowserDialog
                {
                    Title = "Select the location for the application data..."
                };

                var result = browseDialog.ShowDialog();
                if (result.HasValue && result.Value)
                    ApplicationDataLocation = browseDialog.FileName;
            });

            _defaultProjectDirectorySelectCommand = new RelayCommand(() =>
            {
                var browseDialog = new WPFFolderBrowserDialog
                {
                    Title = "Select the default location for your projects..."
                };

                var result = browseDialog.ShowDialog();
                if (result.HasValue && result.Value)
                    DefaultProjectDirectory = browseDialog.FileName;
            });

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
            get => _applicationDataSelectCommand;
            set => SetProperty(value, ref _applicationDataSelectCommand, nameof(ApplicationDataSelectCommand));
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
            set => SetProperty(value, ref _defaultProjectDirectorySelectCommand, nameof(DefaultProjectDirectorySelectCommand));
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