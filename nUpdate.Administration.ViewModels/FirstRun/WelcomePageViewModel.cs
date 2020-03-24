// WelcomePageViewModel.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using nUpdate.Administration.PluginBase.ViewModels;

namespace nUpdate.Administration.ViewModels.FirstRun
{
    public class WelcomePageViewModel : WizardPageViewModelBase
    {
        public WelcomePageViewModel()
        {
            CanGoForward = true;
        }
    }
}