// Author: Dominic Beger (Trade/ProgTrade) 2017

using System;
using System.IO;
using System.Windows.Input;
using nUpdate.Administration.Infrastructure;
using Ookii.Dialogs.Wpf;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class GeneralDataPageViewModel : PageViewModel
    {
        private readonly NewProjectViewModel _newProjectViewModel;
        private string _location;
        private ICommand _locationSelectCommand;
        private string _name;
        private string _updateDirectory;
        private ICommand _updateDirectorySelectCommand;

        public GeneralDataPageViewModel(NewProjectViewModel viewModel)
        {
            _newProjectViewModel = viewModel;
            CanGoBack = true;

            _location = PathProvider.DefaultProjectDirectory;
            _locationSelectCommand = new RelayCommand(() =>
            {
                var browseDialog = new VistaFolderBrowserDialog
                {
                    ShowNewFolderButton = true,
                    Description = "Select the project location...",
                    UseDescriptionForTitle = true
                };

                var result = browseDialog.ShowDialog();
                if (result.HasValue && result.Value)
                    Location = browseDialog.SelectedPath;
            });

            _updateDirectorySelectCommand = new RelayCommand(() =>
            {
                var browseDialog = new VistaFolderBrowserDialog
                {
                    ShowNewFolderButton = true,
                    Description = "Select the local update directory...",
                    UseDescriptionForTitle = true
                };

                var result = browseDialog.ShowDialog();
                if (result.HasValue && result.Value)
                    UpdateDirectory = browseDialog.SelectedPath;
            });
        }

        public string Location
        {
            get => _location;
            set
            {
                SetProperty(value, ref _location, nameof(Location));
                RefreshNavigation();
            }
        }

        public ICommand LocationSelectCommand
        {
            get => _locationSelectCommand;
            set => SetProperty(value, ref _locationSelectCommand, nameof(LocationSelectCommand));
        }

        public string Name
        {
            get => _name;
            set
            {
                SetProperty(value, ref _name, nameof(Name));
                RefreshNavigation();
            }
        }

        public string UpdateDirectory
        {
            get => _updateDirectory;
            set
            {
                SetProperty(value, ref _updateDirectory, nameof(UpdateDirectory));
                RefreshNavigation();
            }
        }

        public ICommand UpdateDirectorySelectCommand
        {
            get => _updateDirectorySelectCommand;
            set => SetProperty(value, ref _updateDirectorySelectCommand, nameof(UpdateDirectorySelectCommand));
        }

        public override void OnNavigated(PageViewModel fromPage, PagedWindowViewModel window)
        {
            if (!Directory.Exists(PathProvider.DefaultProjectDirectory))
                Directory.CreateDirectory(PathProvider.DefaultProjectDirectory);

            var nameAvailable = false;
            var targetDirectory = default(DirectoryInfo);
            while (!nameAvailable)
            {
                var i = 0;
                targetDirectory = new DirectoryInfo(Path.Combine(PathProvider.DefaultProjectDirectory,
                    $"NewProject{(i > 0 ? i.ToString() : string.Empty)}"));
                if (!(nameAvailable = !targetDirectory.Exists))
                    ++i;
            }

            Name = targetDirectory.Name;
        }

        private void RefreshNavigation()
        {
            CanGoForward = !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Location) &&
                           Uri.TryCreate(UpdateDirectory, UriKind.Absolute, out Uri _);

            // If the data is okay, we will set it directly.
            if (CanGoForward)
                RefreshProjectData();
        }

        private void RefreshProjectData()
        {
            _newProjectViewModel.ProjectCreationData.Project.Name = Name;
            _newProjectViewModel.ProjectCreationData.Project.UpdateDirectory = new Uri(UpdateDirectory);
            _newProjectViewModel.ProjectCreationData.Location = Location;
        }
    }
}