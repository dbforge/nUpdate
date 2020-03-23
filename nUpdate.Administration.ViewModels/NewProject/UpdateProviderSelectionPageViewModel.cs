using System;
using System.Collections.Generic;
using System.Linq;
using nUpdate.Administration.PluginBase.Models;
using nUpdate.Administration.PluginBase.ViewModels;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class UpdateProviderSelectionPageViewModel : WizardPageViewModelBase
    {
        private readonly ProjectCreationData _projectCreationData;
        private KeyValuePair<Guid, string> _selectedUpdateProviderIdentifier;

        public UpdateProviderSelectionPageViewModel(ProjectCreationData projectCreationData, Dictionary<Guid, string> updateProviderDictionary)
        {
            _projectCreationData = projectCreationData;
            UpdateProviderDictionary = updateProviderDictionary;
            CanGoBack = true;
            CanGoForward = true;

            PropertyChanged += (sender, args) => RefreshProjectData();
            _selectedUpdateProviderIdentifier = UpdateProviderDictionary.First();
        }

        private void RefreshProjectData()
        {
            _projectCreationData.Project.UpdateProviderIdentifier = _selectedUpdateProviderIdentifier.Key;
        }
        
        public Dictionary<Guid, string> UpdateProviderDictionary { get; set; }
        public KeyValuePair<Guid, string> SelectedUpdateProviderIdentifier
        {
            get => _selectedUpdateProviderIdentifier;
            set => SetProperty(value, ref _selectedUpdateProviderIdentifier);
        }
    }
}
