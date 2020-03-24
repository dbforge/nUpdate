// UpdateProviderWizardPageViewModelBase.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using nUpdate.Administration.PluginBase.Models;

namespace nUpdate.Administration.PluginBase.ViewModels
{
    public abstract class UpdateProviderWizardPageViewModelBase : WizardPageViewModelBase
    {
        protected UpdateProviderWizardPageViewModelBase(WizardViewModelBase wizardViewModelBase,
            ProjectCreationData projectCreationData)
        {
            WizardViewModelBase = wizardViewModelBase;
            ProjectCreationData = projectCreationData;
        }

        public ProjectCreationData ProjectCreationData { get; set; }
        public WizardViewModelBase WizardViewModelBase { get; set; }
    }
}