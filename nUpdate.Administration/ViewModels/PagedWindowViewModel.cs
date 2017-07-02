using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using nUpdate.Administration.Infrastructure;

namespace nUpdate.Administration.ViewModels
{
    public abstract class PagedWindowViewModel : ViewModel
    {
        private PageViewModel _currentPageViewModel;
        private RelayCommand _goBackCommand;
        private RelayCommand _goForwardCommand;
        private ReadOnlyCollection<PageViewModel> _pageViewModels;
        
        /// <summary>
        ///     Loads the pages associated with this window.
        /// </summary>
        protected void LoadPages(IList<PageViewModel> viewModels)
        {
            PageViewModels = new ReadOnlyCollection<PageViewModel>(viewModels);
            CurrentPageViewModel = PageViewModels[0];
        }

        private bool EnsureObjectState()
        {
            if (_pageViewModels == null)
                throw new InvalidOperationException("Call LoadPages before working with this object.");

            if (!_pageViewModels.Any())
                throw new InvalidOperationException("There are no page view models available.");

            return true;
        }

        public bool CanGoBack => EnsureObjectState() && PageViewModels.IndexOf(CurrentPageViewModel) != 0 && CurrentPageViewModel.CanGoBack;

        public bool CanGoForward => EnsureObjectState() && PageViewModels.IndexOf(CurrentPageViewModel) != PageViewModels.Count - 1 && CurrentPageViewModel.CanGoForward;

        public PageViewModel CurrentPageViewModel
        {
            get
            {
                EnsureObjectState();
                return _currentPageViewModel;
            }

            set
            {
                EnsureObjectState();
                SetProperty(value, ref _currentPageViewModel);
            }
        }

        public RelayCommand GoBackCommand
        {
            get
            {
                EnsureObjectState();
                return _goBackCommand ?? (_goBackCommand = new RelayCommand(() =>
                {
                    if (CanGoBack)
                        CurrentPageViewModel = PageViewModels[PageViewModels.IndexOf(CurrentPageViewModel) - 1];
                }));
            }
        }

        public RelayCommand GoForwardCommand
        {
            get
            {
                EnsureObjectState();
                return _goForwardCommand ?? (_goForwardCommand = new RelayCommand(() =>
                {
                    if (CanGoForward)
                        CurrentPageViewModel = PageViewModels[PageViewModels.IndexOf(CurrentPageViewModel) + 1];
                }));
            }
        }


        protected ReadOnlyCollection<PageViewModel> PageViewModels
        {
            get
            {
                EnsureObjectState();
                return _pageViewModels;
            }
            private set { _pageViewModels = value; }
        }
    }
}