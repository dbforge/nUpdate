// UpdateSearchViewModel.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using nUpdate.Localization;
using nUpdate.UI.WPF.ServiceInterfaces;
using nUpdate.UI.WPF.ViewModel.Interfaces;
using nUpdate.Updating;

// ReSharper disable once CheckNamespace
namespace nUpdate.UI.WPF.ViewModel
{
    public class UpdateSearchViewModel : UpdateUiBaseViewModel, IDialogViewModel
    {
        public UpdateSearchViewModel(UpdateManager manager)
        {
            UpdateManager = manager;
            LocProperties = LocalizationHelper.GetLocalizationProperties(UpdateManager.LanguageCulture,
                UpdateManager.CultureFilePaths);

            WindowIcon = GetIcon(IconHelper.ExtractAssociatedIcon(Application.ExecutablePath));
            WindowTitle = LocProperties.UpdateSearchDialogHeader;

            Abort = new RelayCommand(o => Abort_Execute(), o => true);
        }

        internal bool UpdatesFound { get; set; }


        internal bool HasError { get; set; }
        public LocalizationProperties LocProperties { get; }


        public ICommand Abort { get; }
        public bool DialogResult { get; set; }
        public string WindowTitle { get; set; }
        public Dispatcher CurrentDispatcher { get; set; }


        public async void DialogLoaded()
        {
            try
            {
                UpdatesFound = await UpdateManager.SearchForUpdatesAsync();
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                var msgService = ServiceContainer.Instance.GetService<IMessageboxService>();
                msgService.Show(ex.Message, LocProperties.UpdateSearchErrorCaption, EnuMessageBoxButton.Ok,
                    EnuMessageBoxImage.Error);

                UpdatesFound = false;
                HasError = true;
            }

            var dialogService = ServiceContainer.Instance.GetService<IDialogWindowService>();
            dialogService.CloseDialog();
        }

        public void DialogClosing()
        {
            DialogResult = UpdatesFound;
            if (!DialogResult) UpdateManager.CancelSearchAsync();
        }

        private void Abort_Execute()
        {
            UpdatesFound = false;
            var dialogService = ServiceContainer.Instance.GetService<IDialogWindowService>();
            dialogService.CloseDialog();
        }
    }
}