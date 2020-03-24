// HttpBackendSelectionPageViewModel.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using nUpdate.Administration.Models.Http;
using nUpdate.Administration.PluginBase.Models;
using nUpdate.Administration.PluginBase.ViewModels;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class HttpBackendSelectionPageViewModel : UpdateProviderWizardPageViewModelBase, IFirstUpdateProviderBase
    {
        private HttpBackendType _httpBackendType;

        public HttpBackendSelectionPageViewModel(WizardViewModelBase wizardViewModelBase,
            ProjectCreationData projectCreationData)
            : base(wizardViewModelBase, projectCreationData)
        {
            PropertyChanged += (sender, args) => RefreshProjectData();
            CanGoBack = true;
            CanGoForward = true;
        }

        public HttpBackendType HttpBackendType
        {
            get => _httpBackendType;
            set => SetProperty(value, ref _httpBackendType);
        }

        private void RefreshProjectData()
        {
            HttpBackendType = _httpBackendType;
        }
    }
}