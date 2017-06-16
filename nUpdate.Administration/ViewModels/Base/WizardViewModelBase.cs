using System.Collections.ObjectModel;
using nUpdate.Administration.Infrastructure;

namespace nUpdate.Administration.ViewModels.Base
{
    public abstract class WizardViewModelBase : PropertyChangedBase
    {
        protected readonly ReadOnlyCollection<WizardPageViewModelBase> PageViewModels;
        
        private WizardPageViewModelBase _currentPageViewModel;
        private RelayCommand _goBackCommand;
        private RelayCommand _goForwardCommand;

        public bool CanGoForward => CurrentPageViewModel.CanGoForward;
        public bool CanGoBack => CurrentPageViewModel.CanGoBack;

        public WizardPageViewModelBase CurrentPageViewModel
        {
            get { return _currentPageViewModel; }
            set { SetProperty(value, ref _currentPageViewModel); }
        }

        public RelayCommand GoForwardCommand
        {
            get
            {
                return _goForwardCommand ?? (_goForwardCommand = new RelayCommand(() =>
                {
                    if (CanGoForward)
                        CurrentPageViewModel = PageViewModels[PageViewModels.IndexOf(CurrentPageViewModel) + 1];
                }));
            }
        }

        public RelayCommand GoBackCommand
        {
            get
            {
                return _goBackCommand ?? (_goBackCommand = new RelayCommand(() =>
                {
                    if (CanGoBack)
                        CurrentPageViewModel = PageViewModels[PageViewModels.IndexOf(CurrentPageViewModel) - 1];
                }));
            }
        }
    }
}
