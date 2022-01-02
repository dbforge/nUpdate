// HttpUpdateProviderPlugin.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using nUpdate.Administration.BusinessLogic.Http;
using nUpdate.Administration.PluginBase;
using nUpdate.Administration.PluginBase.BusinessLogic;
using nUpdate.Administration.PluginBase.Models;
using nUpdate.Administration.PluginBase.ViewModels;
using nUpdate.Administration.ViewModels.NewProject;
using nUpdate.Administration.Views.NewProject;

namespace nUpdate.Administration.Plugins
{
    [Export(typeof(IUpdateProviderPlugin))]
    public class HttpUpdateProviderPlugin : IUpdateProviderPlugin
    {
        private HttpBackendSelectionPageViewModel _httpBackendSelectionPageViewModel;
        private HttpDataPageViewModel _httpDataPageViewModel;
        public string Author => "Dominic Beger";
        public string Description => "HTTP(S)";

        public Guid Identifier => Guid.Parse("a8085977-15cd-447f-bf55-48b3c403c620");
        public string Name => "HTTP(S) update provider";
        public (Version, Version) SupportedVersionRange => (new Version(4, 0), null);
        public string Url => string.Empty;
        public Version Version => new Version(1, 0);

        public UpdateProviderWizardPageViewModelBase GetNextPageViewModel(WizardViewModelBase wizardViewModelBase,
            WizardPageViewModelBase current,
            ProjectCreationData projectCreationData)
        {
            if (!(current is UpdateProviderWizardPageViewModelBase))
                return _httpBackendSelectionPageViewModel ?? (_httpBackendSelectionPageViewModel =
                           new HttpBackendSelectionPageViewModel(wizardViewModelBase, projectCreationData));
            return current is HttpBackendSelectionPageViewModel
                ? _httpDataPageViewModel ?? (_httpDataPageViewModel =
                      new HttpDataPageViewModel(wizardViewModelBase, projectCreationData))
                : null;
        }

        public UpdateProviderWizardPageViewModelBase GetPreviousPageViewModel(WizardViewModelBase wizardViewModelBase,
            WizardPageViewModelBase current,
            ProjectCreationData projectCreationData)
        {
            return current is HttpDataPageViewModel ? _httpBackendSelectionPageViewModel : null;
        }

        public IUpdateProvider UpdateProvider => new HttpUpdateProvider();

        public Dictionary<Type, Type> WizardViewModelViewAssociations => new Dictionary<Type, Type>
        {
            {typeof(HttpBackendSelectionPageViewModel), typeof(HttpBackendSelectionPage)},
            {typeof(HttpDataPageViewModel), typeof(HttpDataPage)}
        };
    }
}