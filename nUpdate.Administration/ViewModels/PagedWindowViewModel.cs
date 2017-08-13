// Author: Dominic Beger (Trade/ProgTrade) 2017

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using nUpdate.Administration.Infrastructure;
using nUpdate.Administration.Views;

namespace nUpdate.Administration.ViewModels
{
    public abstract class PagedWindowViewModel : ViewModel
    {
        #region NavigationDependencyProperties

        private static readonly DependencyProperty PageCanGoBackProperty =
            DependencyProperty.Register("PageCanGoBack", typeof(bool), typeof(PagedWindowViewModel),
                new PropertyMetadata((obj, e) => { ((PagedWindowViewModel) obj).PageNavigationPropertiesChanged(); }));

        private static readonly DependencyProperty PageCanGoForwardProperty =
            DependencyProperty.Register("PageCanGoForward", typeof(bool), typeof(PagedWindowViewModel),
                new PropertyMetadata((obj, e) => { ((PagedWindowViewModel) obj).PageNavigationPropertiesChanged(); }));

        private static readonly DependencyPropertyKey CanGoBackPropertyKey =
            DependencyProperty.RegisterReadOnly("CanGoBack", typeof(bool), typeof(PagedWindowViewModel),
                new PropertyMetadata(false));

        /// <summary>
        ///     Identifies the <see cref="CanGoBack" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanGoBackProperty = CanGoBackPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey CanGoForwardPropertyKey =
            DependencyProperty.RegisterReadOnly("CanGoForward", typeof(bool), typeof(PagedWindowViewModel),
                new PropertyMetadata(false));

        /// <summary>
        ///     Identifies the <see cref="CanGoForward" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanGoForwardProperty = CanGoForwardPropertyKey.DependencyProperty;

        #endregion

        #region CloseDependencyProperties

        private static readonly DependencyProperty PageAllowsClosingProperty = DependencyProperty.Register("PageAllowsClosing", typeof(bool), typeof(PagedWindowViewModel),
            new PropertyMetadata((obj, e) => { ((PagedWindowViewModel)obj).PageAllowClosingPropertiesChanged(); }));

        private static readonly DependencyPropertyKey AllowClosingPropertyKey =
            DependencyProperty.RegisterReadOnly("AllowClosing", typeof(bool), typeof(PagedWindowViewModel),
                new PropertyMetadata(false));

        /// <summary>
        ///     Identifies the <see cref="AllowClosing" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowClosingProperty = AllowClosingPropertyKey.DependencyProperty;


        #endregion

        private PageViewModel _currentPageViewModel;
        private RelayCommand _goBackCommand;
        private RelayCommand _goForwardCommand;
        private ReadOnlyCollection<PageViewModel> _pageViewModels;
        private bool _allowClosing = true;

        /// <summary>
        ///     Gets a value indicating whether the user is allowed to go back, or not.
        /// </summary>
        public bool CanGoBack => (bool) GetValue(CanGoBackProperty);

        /// <summary>
        ///     Gets a value indicating whether the user is allowed to go forward, or not.
        /// </summary>
        public bool CanGoForward => (bool) GetValue(CanGoForwardProperty);

        /// <summary>
        ///     Gets a value indicating whether the user is allowed to close the window, or not.
        /// </summary>
        public bool AllowClosing => (bool) GetValue(AllowClosingProperty);

        /// <summary>
        ///     Gets or sets the view model of the current page to be shown.
        /// </summary>
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
                SetPageBindings();
            }
        }

        /// <summary>
        ///     Gets the command for going back to the previous page.
        /// </summary>
        public RelayCommand GoBackCommand
        {
            get
            {
                EnsureObjectState();
                return _goBackCommand ?? (_goBackCommand =
                           new RelayCommand(
                               () =>
                               {
                                   var oldPageViewModel = CurrentPageViewModel;
                                   oldPageViewModel.OnNavigateBack(this);
                                   CurrentPageViewModel =
                                       PageViewModels[PageViewModels.IndexOf(CurrentPageViewModel) - 1];
                                   CurrentPageViewModel.OnNavigated(oldPageViewModel, this);
                               }, () => CanGoBack));
            }
        }

        /// <summary>
        ///     Gets the command for going forward to the next page.
        /// </summary>
        public RelayCommand GoForwardCommand
        {
            get
            {
                EnsureObjectState();
                return _goForwardCommand ?? (_goForwardCommand =
                           new RelayCommand(
                               async () =>
                               {
                                   var oldPageViewModel = CurrentPageViewModel;
                                   oldPageViewModel.OnNavigateForward(this);

                                   // There is no further page which means we are finished with the wizard
                                   if (PageViewModels.IndexOf(CurrentPageViewModel) == PageViewModels.Count - 1)
                                   {
                                       _allowClosing = false;
                                       var finished = await Finish();
                                       _allowClosing = true;

                                       // If no errors occured and everything worked, we can now close the window if (result)
                                       if (finished)
                                           WindowManager.GetCurrentWindow().Close();
                                       return;
                                   }

                                   CurrentPageViewModel =
                                       PageViewModels[PageViewModels.IndexOf(CurrentPageViewModel) + 1];
                                   CurrentPageViewModel.OnNavigated(oldPageViewModel, this);
                               }, () => CanGoForward));
            }
        }


        /// <summary>
        ///     Gets the view models of the pages to be shown.
        /// </summary>
        protected ReadOnlyCollection<PageViewModel> PageViewModels
        {
            get
            {
                EnsureObjectState();
                return _pageViewModels;
            }
            private set => _pageViewModels = value;
        }

        private void EnsureObjectState()
        {
            if (_pageViewModels == null)
                throw new InvalidOperationException("Call InitializePages before working with this object.");

            if (!_pageViewModels.Any())
                throw new InvalidOperationException("There are no page view models available.");
        }

        /// <summary>
        ///     Performs the final steps with the data collected among the different pages to finish the wizard.
        /// </summary>
        protected abstract Task<bool> Finish();

        /// <summary>
        ///     Initialize the pages associated with this window.
        /// </summary>
        protected void InitializePages(IList<PageViewModel> viewModels)
        {
            PageViewModels = new ReadOnlyCollection<PageViewModel>(viewModels);
            CurrentPageViewModel = PageViewModels[0];
            CurrentPageViewModel.OnNavigated(null, this);
        }

        private void PageAllowClosingPropertiesChanged()
        {
            SetValue(AllowClosingPropertyKey, _allowClosing && (bool) GetValue(PageAllowsClosingProperty));
        }

        private void PageNavigationPropertiesChanged()
        {
            var previousPageAvailable = PageViewModels.IndexOf(CurrentPageViewModel) > 0;
            SetValue(CanGoBackPropertyKey, previousPageAvailable && (bool) GetValue(PageCanGoBackProperty) &&
                                           PageViewModels[PageViewModels.IndexOf(CurrentPageViewModel) - 1]
                                               .CanBeShown);

            var nextPageAvailable = PageViewModels.IndexOf(CurrentPageViewModel) < PageViewModels.Count - 1;
            SetValue(CanGoForwardPropertyKey, (bool) GetValue(PageCanGoForwardProperty) &&
                                              (!nextPageAvailable ||
                                               PageViewModels[PageViewModels.IndexOf(CurrentPageViewModel) + 1]
                                                   .CanBeShown));

            GoBackCommand.OnCanExecuteChanged();
            GoForwardCommand.OnCanExecuteChanged();
        }

        public void RequestGoBack()
        {
            GoBackCommand.Execute();
        }

        public void RequestGoForward()
        {
            GoForwardCommand.Execute();
        }

        private void SetPageBindings()
        {
            var canGoBackwardBinding = new Binding
            {
                Source = CurrentPageViewModel,
                Path = new PropertyPath("CanGoBack"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.ClearBinding(this, PageCanGoBackProperty);
            BindingOperations.SetBinding(this, PageCanGoBackProperty, canGoBackwardBinding);

            var canGoForwardBinding = new Binding
            {
                Source = CurrentPageViewModel,
                Path = new PropertyPath("CanGoForward"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.ClearBinding(this, PageCanGoForwardProperty);
            BindingOperations.SetBinding(this, PageCanGoForwardProperty, canGoForwardBinding);
        }
    }
}