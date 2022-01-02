// DownloadUpdateViewModel.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using nUpdate.Exceptions;
using nUpdate.Localization;
using nUpdate.UI.WPF.ServiceInterfaces;
using nUpdate.UI.WPF.ViewModel.Interfaces;
using nUpdate.UpdateEventArgs;
using nUpdate.Updating;

// ReSharper disable once CheckNamespace
namespace nUpdate.UI.WPF.ViewModel
{
    public class DownloadUpdateViewModel : UpdateUiBaseViewModel, IDialogViewModel
    {
        private string _infoText;
        private float _progressPercentage;


        private bool _refreshProgress;


        public DownloadUpdateViewModel(UpdateManager manager)
        {
            UpdateManager = manager;

            Abort = new RelayCommand(o => Abort_Execute(), o => true);

            LocProperties = LocalizationHelper.GetLocalizationProperties(UpdateManager.LanguageCulture,
                UpdateManager.CultureFilePaths);

            WindowIcon = GetIcon(IconHelper.ExtractAssociatedIcon(Application.ExecutablePath));
            WindowTitle = LocProperties.UpdateDownloadDialogLoadingHeader;

            InfoText = string.Format(
                LocProperties.UpdateDownloadDialogLoadingInfo, "0");
        }

        public LocalizationProperties LocProperties { get; }

        public string InfoText
        {
            get => _infoText;
            set
            {
                _infoText = value;
                RaisePropertyChanged();
            }
        }

        public float ProgressPercentage
        {
            get => _progressPercentage;
            set
            {
                if (_refreshProgress)
                {
                    _progressPercentage = (int) value;
                    InfoText = string.Format(UpdateManager.LanguageCulture,
                        LocProperties.UpdateDownloadDialogLoadingInfo, Math.Round(value, 1));
                    RaisePropertyChanged();
                }

                _refreshProgress = !_refreshProgress;
            }
        }


        public ICommand Abort { get; }


        public bool DialogResult { get; set; }
        public string WindowTitle { get; set; }
        public Dispatcher CurrentDispatcher { get; set; }


        public async void DialogLoaded()
        {
            var progress = new Progress<UpdateDownloadProgressChangedEventArgs>();
            progress.ProgressChanged += (o, value) => ProgressPercentage = value.Percentage;

            try
            {
                await UpdateManager.DownloadPackagesAsync(progress);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (StatisticsException ex)
            {
                var msgService = ServiceContainer.Instance.GetService<IMessageboxService>();
                msgService.Show($"Error while adding a new statistics entry.  {ex.Message}", "Error",
                    EnuMessageBoxButton.Ok,
                    EnuMessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                var msgService = ServiceContainer.Instance.GetService<IMessageboxService>();
                msgService.Show($"Error while downloading the update package. {ex.Message}", "Error",
                    EnuMessageBoxButton.Ok,
                    EnuMessageBoxImage.Error);
                DialogResult = false;
                return;
            }

            DialogResult = true;
            var dialogService = ServiceContainer.Instance
                .GetService<IDialogWindowService>();
            dialogService.CloseDialog();
        }

        public void DialogClosing()
        {
            if (!DialogResult) UpdateManager.CancelDownloadAsync();
        }

        private void Abort_Execute()
        {
            DialogResult = false;
            var dialogService = ServiceContainer.Instance
                .GetService<IDialogWindowService>();
            dialogService.CloseDialog();
        }
    }
}