// Author: Dominic Beger (Trade/ProgTrade) 2017

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using nUpdate.Administration.Infrastructure;
using nUpdate.Administration.Views;

namespace nUpdate.Administration.ViewModels
{
    public abstract class PagedWindowViewModel : ViewModel
    {
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

        private PageViewModel _currentPageViewModel;
        private RelayCommand _goBackCommand;
        private RelayCommand _goForwardCommand;
        private ReadOnlyCollection<PageViewModel> _pageViewModels;

        /// <summary>
        ///     Gets a value indicating whether the user is allowed to go back.
        /// </summary>
        public bool CanGoBack => (bool) GetValue(CanGoBackProperty);

        /// <summary>
        ///     Gets a value indicating whether the current view model allows to go forward.
        /// </summary>
        public bool CanGoForward => (bool) GetValue(CanGoForwardProperty);

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
                                   CurrentPageViewModel =
                                       PageViewModels[PageViewModels.IndexOf(CurrentPageViewModel) - 1];
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
                               () =>
                               {
                                   CurrentPageViewModel =
                                       PageViewModels[PageViewModels.IndexOf(CurrentPageViewModel) + 1];
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
        ///     Initialize the pages associated with this window.
        /// </summary>
        protected void InitializePages(IList<PageViewModel> viewModels)
        {
            PageViewModels = new ReadOnlyCollection<PageViewModel>(viewModels);
            CurrentPageViewModel = PageViewModels[0];
        }

        private void PageNavigationPropertiesChanged()
        {
            var previousPageAvailable = PageViewModels.IndexOf(CurrentPageViewModel) > 0;
            SetValue(CanGoBackPropertyKey, previousPageAvailable && (bool) GetValue(PageCanGoBackProperty) &&
                                           PageViewModels[PageViewModels.IndexOf(CurrentPageViewModel) - 1]
                                               .CanBeShown);

            var nextPageAvailable = PageViewModels.IndexOf(CurrentPageViewModel) < PageViewModels.Count - 1;
            SetValue(CanGoForwardPropertyKey, nextPageAvailable && (bool) GetValue(PageCanGoForwardProperty) &&
                                              PageViewModels[PageViewModels.IndexOf(CurrentPageViewModel) + 1]
                                                  .CanBeShown);

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

        public void RequestClose()
        {
            var nextPageAvailable = PageViewModels.IndexOf(CurrentPageViewModel) < PageViewModels.Count - 1;
            if (!nextPageAvailable && (bool) GetValue(PageCanGoForwardProperty))
                WindowManager.GetCurrentWindow().Close();
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