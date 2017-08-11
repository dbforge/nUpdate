// Author: Dominic Beger (Trade/ProgTrade) 2017

using System;
using System.IO;

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

        public PathSetupPageViewModel(FirstRunViewModel firstRunViewModel)
        {
            _firstRunViewModel = firstRunViewModel;
            CanGoBack = true;

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

        public string DefaultProjectDirectory
        {
            get => _defaultProjectDirectory;
            set
            {
                SetProperty(value, ref _defaultProjectDirectory, nameof(DefaultProjectDirectory));
                RefreshNavigation();
            }
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