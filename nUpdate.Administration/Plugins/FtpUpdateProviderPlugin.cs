// FtpUpdateProviderPlugin.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using nUpdate.Administration.BusinessLogic.Ftp;
using nUpdate.Administration.PluginBase;
using nUpdate.Administration.PluginBase.BusinessLogic;
using nUpdate.Administration.PluginBase.Models;
using nUpdate.Administration.PluginBase.ViewModels;
using nUpdate.Administration.ViewModels.NewProject;
using nUpdate.Administration.Views.NewProject;

namespace nUpdate.Administration.Plugins
{
    [Export(typeof(IUpdateProviderPlugin))]
    public class FtpUpdateProviderPlugin : IUpdateProviderPlugin
    {
        public string Author => "Dominic Beger";
        public string Description => "FTP(S)";
        public Guid Identifier => Guid.Parse("497491ed-950a-43c2-bb92-f303cdacc4e2");
        public string Name => "FTP(S) update provider";
        public (Version, Version) SupportedVersionRange => (new Version(4, 0), null);
        public string Url => string.Empty;
        public Version Version => new Version(1, 0);

        public UpdateProviderWizardPageViewModelBase GetNextPageViewModel(WizardViewModelBase wizardViewModelBase,
            WizardPageViewModelBase current, ProjectCreationData projectCreationData)
        {
            return !(current is UpdateProviderWizardPageViewModelBase)
                ? new FtpDataPageViewModel(wizardViewModelBase, projectCreationData)
                : null;
        }

        public UpdateProviderWizardPageViewModelBase GetPreviousPageViewModel(WizardViewModelBase wizardViewModelBase,
            WizardPageViewModelBase current, ProjectCreationData projectCreationData)
        {
            return null;
        }

        public IUpdateProvider UpdateProvider => new FtpServerUpdateProvider();

        public Dictionary<Type, Type> WizardViewModelViewAssociations => new Dictionary<Type, Type>
            {{typeof(FtpDataPageViewModel), typeof(FtpDataPage)}};
    }
}