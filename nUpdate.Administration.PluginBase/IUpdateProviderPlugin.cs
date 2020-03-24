// IUpdateProviderPlugin.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Collections.Generic;
using nUpdate.Administration.PluginBase.BusinessLogic;
using nUpdate.Administration.PluginBase.Models;
using nUpdate.Administration.PluginBase.ViewModels;

namespace nUpdate.Administration.PluginBase
{
    public interface IUpdateProviderPlugin : IPluginBase
    {
        IUpdateProvider UpdateProvider { get; }
        Dictionary<Type, Type> WizardViewModelViewAssociations { get; }

        UpdateProviderWizardPageViewModelBase GetNextPageViewModel(WizardViewModelBase wizardViewModelBase,
            WizardPageViewModelBase current, ProjectCreationData projectCreationData);

        UpdateProviderWizardPageViewModelBase GetPreviousPageViewModel(WizardViewModelBase wizardViewModelBase,
            WizardPageViewModelBase current, ProjectCreationData projectCreationData);
    }
}