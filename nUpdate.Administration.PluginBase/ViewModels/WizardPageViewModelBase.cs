// WizardPageViewModelBase.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using nUpdate.Administration.Infrastructure;

namespace nUpdate.Administration.PluginBase.ViewModels
{
    /// <summary>
    ///     Represents a view model for a wizard page.
    /// </summary>
    public abstract class WizardPageViewModelBase : NotifyPropertyChangedBase
    {
        private bool _canBeShown = true;
        private bool _canGoBack;
        private bool _canGoForward;
        private bool _needsUserInteraction = true;

        public bool CanBeShown
        {
            get => _canBeShown;
            set => SetProperty(value, ref _canBeShown);
        }

        public bool CanGoBack
        {
            get => _canGoBack;
            set => SetProperty(value, ref _canGoBack);
        }

        public bool CanGoForward
        {
            get => _canGoForward;
            set => SetProperty(value, ref _canGoForward);
        }

        public ushort DisplayOrderId { get; }

        public bool NeedsUserInteraction
        {
            get => _needsUserInteraction;
            set => SetProperty(value, ref _needsUserInteraction);
        }

        public virtual void OnNavigateBack(WizardViewModelBase window)
        {
        }

        public virtual void OnNavigated(WizardPageViewModelBase fromPage, WizardViewModelBase window)
        {
        }

        public virtual void OnNavigateForward(WizardViewModelBase window)
        {
        }
    }
}