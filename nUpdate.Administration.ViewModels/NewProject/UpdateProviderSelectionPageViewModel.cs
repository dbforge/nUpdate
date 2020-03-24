// UpdateProviderSelectionPageViewModel.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using nUpdate.Administration.Infrastructure;
using nUpdate.Administration.PluginBase.Models;
using nUpdate.Administration.PluginBase.ViewModels;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class UpdateProviderSelectionPageViewModel : WizardPageViewModelBase
    {
        private readonly ProjectCreationData _projectCreationData;
        private TrulyObservableCollection<UpdateProviderViewModel> _availableUpdateProviderViewModels;
        private ICommand _updateProviderSelectionCommand;

        public UpdateProviderSelectionPageViewModel(ProjectCreationData projectCreationData,
            IEnumerable<UpdateProviderViewModel> updateProviderViewModels)
        {
            _projectCreationData = projectCreationData;
            _availableUpdateProviderViewModels =
                new TrulyObservableCollection<UpdateProviderViewModel>(updateProviderViewModels);
            _availableUpdateProviderViewModels.First().IsSelected = true;
            _updateProviderSelectionCommand = new RelayCommand(SelectUpdateProvider);
            RefreshProjectData();

            CanGoBack = true;
            CanGoForward = true;

            PropertyChanged += (sender, args) => RefreshProjectData();
        }

        public TrulyObservableCollection<UpdateProviderViewModel> AvailableUpdateProviderViewModels
        {
            get => _availableUpdateProviderViewModels;
            set => SetProperty(value, ref _availableUpdateProviderViewModels);
        }

        public UpdateProviderViewModel SelectedViewModel => AvailableUpdateProviderViewModels.First(x => x.IsSelected);

        public ICommand UpdateProviderSelectionCommand
        {
            get => _updateProviderSelectionCommand;
            set => SetProperty(value, ref _updateProviderSelectionCommand);
        }

        private void RefreshProjectData()
        {
            _projectCreationData.Project.UpdateProviderIdentifier = SelectedViewModel.Identifier;
        }

        private void SelectUpdateProvider(object o)
        {
            if (o == null)
                throw new ArgumentNullException(nameof(o));

            var updateProvider = (UpdateProviderViewModel) o;
            foreach (var upv in AvailableUpdateProviderViewModels)
                upv.IsSelected = false;
            updateProvider.IsSelected = true;
            OnPropertyChanged();
        }
    }
}