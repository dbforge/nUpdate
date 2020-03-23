// Copyright © Dominic Beger 2018

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using nUpdate.Administration.Infrastructure;

/*using System.Windows.Input;
using System.Windows;
using System.Windows.Data;
using nUpdate.Administration.Views;*/

namespace nUpdate.Administration.PluginBase.ViewModels
{
    public abstract class WizardViewModelBase : NotifyPropertyChangedBase
    {
        /*private static readonly DependencyProperty PageCanGoBackProperty =
            DependencyProperty.Register("PageCanGoBack", typeof(bool), typeof(PagedWindowViewModel),
                new PropertyMetadata((obj, e) => { ((PagedWindowViewModel) obj).DependencyPropertiesChanged(); }));

        private static readonly DependencyProperty PageCanGoForwardProperty =
            DependencyProperty.Register("PageCanGoForward", typeof(bool), typeof(PagedWindowViewModel),
                new PropertyMetadata((obj, e) => { ((PagedWindowViewModel) obj).DependencyPropertiesChanged(); }));

        private static readonly DependencyPropertyKey CanGoBackPropertyKey =
            DependencyProperty.RegisterReadOnly("CanGoBack", typeof(bool), typeof(PagedWindowViewModel),
                new PropertyMetadata(false));

        /// <summary>
        ///     Identifies the <see cref="CanGoBack" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanGoBackProperty = CanGoBackPropertyKey.DependencyProperty;

        // CanGoForward
        private static readonly DependencyPropertyKey CanGoForwardPropertyKey =
            DependencyProperty.RegisterReadOnly("CanGoForward", typeof(bool), typeof(PagedWindowViewModel),
                new PropertyMetadata(false));

        /// <summary>
        ///     Identifies the <see cref="CanGoForward" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanGoForwardProperty = CanGoForwardPropertyKey.DependencyProperty;*/

        private WizardPageViewModelBase _currentPageViewModel;
        private RelayCommand _goBackCommand;
        private RelayCommand _goForwardCommand;
        private ReadOnlyCollection<WizardPageViewModelBase> _pageViewModels;
        
        /// <summary>
        ///     Gets a value indicating whether the user is allowed to go back, or not.
        /// </summary>
        public bool CanGoBack => _currentPageViewModel.CanGoBack; /*(bool) GetValue(CanGoBackProperty);*/

        /// <summary>
        ///     Gets a value indicating whether the user is allowed to go forward, or not.
        /// </summary>
        public bool CanGoForward => _currentPageViewModel.CanGoForward; /*(bool) GetValue(CanGoForwardProperty);*/

        /// <summary>
        ///     Gets or sets the view model of the current page to be shown.
        /// </summary>
        public WizardPageViewModelBase CurrentPageViewModel
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
                _currentPageViewModel.PropertyChanged += (sender, args) =>
                {
                    OnPropertyChanged(args.PropertyName);
                    GoBackCommand.OnCanExecuteChanged();
                    GoForwardCommand.OnCanExecuteChanged();
                };

                GoBackCommand.OnCanExecuteChanged();
                GoForwardCommand.OnCanExecuteChanged();
                //SetPageBindings();
            }
        }

        /// <summary>
        ///     Gets or sets the action that is invoked as soon as the wizard has finished.
        /// </summary>
        public Action FinishingAction { get; set; } = null;

        /// <summary>
        ///     Gets the command for going back to the previous page.
        /// </summary>
        public RelayCommand GoBackCommand
        {
            get
            {
                EnsureObjectState();
                return _goBackCommand ?? (_goBackCommand =
                           new RelayCommand(GoBack, () => CanGoBack));
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
                           new RelayCommand(GoForward, () => CanGoForward));
            }
        }

        /// <summary>
        ///     Gets the view models of the pages to be shown.
        /// </summary>
        protected ReadOnlyCollection<WizardPageViewModelBase> PageViewModels
        {
            get
            {
                EnsureObjectState();
                return _pageViewModels;
            }
            private set => _pageViewModels = value;
        }

        /*private void DependencyPropertiesChanged()
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
        }*/

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

        protected virtual void GoBack()
        {
            var oldPageViewModel = CurrentPageViewModel;
            oldPageViewModel.OnNavigateBack(this);
            CurrentPageViewModel =
                PageViewModels[PageViewModels.IndexOf(CurrentPageViewModel) - 1];
            CurrentPageViewModel.OnNavigated(oldPageViewModel, this);
        }

        protected virtual async void GoForward()
        {
            var oldPageViewModel = CurrentPageViewModel;
            oldPageViewModel.OnNavigateForward(this);

            // There is no further page which means we are finished with the wizard
            if (PageViewModels.IndexOf(CurrentPageViewModel) == PageViewModels.Count - 1)
            {
                // If no errors occured and everything worked, we can now call the finishing action (such as closing the window)
                if (await Finish())
                    FinishingAction?.Invoke();
                //WindowManager.GetCurrentWindow().RequestClose();
                return;
            }

            CurrentPageViewModel =
                PageViewModels[PageViewModels.IndexOf(CurrentPageViewModel) + 1];
            CurrentPageViewModel.OnNavigated(oldPageViewModel, this);
        }

        /// <summary>
        ///     Initialize the pages associated with this window.
        /// </summary>
        protected void InitializePages(IList<WizardPageViewModelBase> viewModels)
        {
            PageViewModels = new ReadOnlyCollection<WizardPageViewModelBase>(viewModels);
            CurrentPageViewModel = PageViewModels[0];
            CurrentPageViewModel.OnNavigated(null, this);
        }

        public void RequestGoBack()
        {
            GoBack();
        }

        public void RequestGoForward()
        {
            GoForward();
        }

        /*private void SetPageBindings()
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
        }*/
    }
}