namespace nUpdate.Administration.ViewModels.Base
{
    /// <summary>
    ///     Represents a view model for a wizard page.
    /// </summary>
    public abstract class WizardPageViewModelBase : PropertyChangedBase
    {
        private bool _canGoForward;
        public bool CanGoForward
        {
            get { return _canGoForward; }
            set { SetProperty(value, ref _canGoForward); }
        }

        private bool _canGoBack;
        public bool CanGoBack
        {
            get { return _canGoBack; }
            set { SetProperty(value, ref _canGoBack); }
        }
    }
}
