// Author: Dominic Beger (Trade/ProgTrade) 2017

namespace nUpdate.Administration.ViewModels
{
    /// <summary>
    ///     Represents a view model for a wizard page.
    /// </summary>
    public abstract class PageViewModel : ViewModel
    {
        private bool _canBeShown = true;
        private bool _canGoBack;
        private bool _canGoForward;
        private bool _needsUserInteraction = true;

        public bool CanBeShown
        {
            get { return _canBeShown; }
            set { SetProperty(value, ref _canBeShown); }
        }

        public bool CanGoBack
        {
            get { return _canGoBack; }
            set { SetProperty(value, ref _canGoBack); }
        }

        public bool CanGoForward
        {
            get { return _canGoForward; }
            set { SetProperty(value, ref _canGoForward); }
        }

        public bool NeedsUserInteraction
        {
            get { return _needsUserInteraction; }
            set { SetProperty(value, ref _needsUserInteraction); }
        }
    }
}