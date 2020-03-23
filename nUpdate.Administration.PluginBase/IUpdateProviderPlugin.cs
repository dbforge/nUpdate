using System;
using System.Collections.Generic;
using nUpdate.Administration.PluginBase.BusinessLogic;
using nUpdate.Administration.PluginBase.Models;
using nUpdate.Administration.PluginBase.ViewModels;

namespace nUpdate.Administration.PluginBase
{
    public interface IUpdateProviderPlugin : IPluginBase
    {
        UpdateProviderWizardPageViewModelBase GetNextPageViewModel(WizardViewModelBase wizardViewModelBase, WizardPageViewModelBase current, ProjectCreationData projectCreationData);
        UpdateProviderWizardPageViewModelBase GetPreviousPageViewModel(WizardViewModelBase wizardViewModelBase, WizardPageViewModelBase current, ProjectCreationData projectCreationData);
        Dictionary<Type, Type> WizardViewModelViewAssociations { get; }
        IUpdateProvider UpdateProvider { get; }
    }
}
