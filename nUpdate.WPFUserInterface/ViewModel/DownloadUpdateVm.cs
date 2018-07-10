using System;
using System.Windows.Input;
using System.Windows.Threading;
using nUpdate.Exceptions;
using nUpdate.Internal.Core;
using nUpdate.Internal.Core.Localization;
using nUpdate.ServiceInterfaces;
using nUpdate.UpdateEventArgs;
using nUpdate.Updating;
using nUpdate.ViewModel;
using nUpdate.ViewModel.Interfaces;
using Application = System.Windows.Forms.Application;

// ReSharper disable once CheckNamespace
namespace nUpdate.WPFUserInterface.ViewModel
{
    class DownloadUpdateVm : UpdateUiBaseVm, IDialogViewModel
    {


        #region "Constructors"

        public DownloadUpdateVm()
        {
            if (!IsInDesignMode)
            {
                throw new Exception(
                    "This constructor is only for DesigntimeSupport!! Please use the second constructor!");
            }

            InfoText = "Please wait while the aviable updates are downloading...";
        }

        public DownloadUpdateVm(UpdateManager manager)
        {
            UpdateManager = manager;

            AbortCommand = new RelayCommand(o => AbortCommand_Execute(), o => true);

            LocProperties = LocalizationHelper.GetLocalizationProperties(UpdateManager.LanguageCulture,
                UpdateManager.CultureFilePaths);

            WindowIcon = GetIcon(IconHelper.ExtractAssociatedIcon(Application.ExecutablePath));
            WindowTitle = LocProperties.UpdateDownloadDialogLoadingHeader;

            InfoText = string.Format(
                LocProperties.UpdateDownloadDialogLoadingInfo, "0");
        }

        #endregion

        #region "Properties"

        public bool DialogResult { get; set; }
        public string WindowTitle { get; set; }
        public Dispatcher CurrentDispatcher { get; set; }
        public LocalizationProperties LocProperties { get; }

        private string _infoText;
        public string InfoText
        {
            get { return _infoText; }
            set
            {
                _infoText = value;
                RaisePropertyChanged();
            }
        }


        private bool _refreshProgress;
        private float _progressPercentage;
        public float ProgressPercentage
        {
            get => _progressPercentage;
            set
            {
                //Nur jedes zweite mal aktualisieren da der Downloader von nUpdate so oft einen event wirft. Da kommt keine View mit.
                //Allerdings ist es nicht schön mit int zu Arbeiten da die Progressbar dann so 'springt'.
                if (_refreshProgress)
                {
                    _progressPercentage = (int)value;
                    InfoText = string.Format(UpdateManager.LanguageCulture,
                        LocProperties.UpdateDownloadDialogLoadingInfo, Math.Round(value, 1));
                    RaisePropertyChanged();
                }
                _refreshProgress = !_refreshProgress;
            }
        }


        #endregion



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
                msgService.Show($"Error while adding a new statistics entry.  {ex.Message}", "Error", EnuMessageBoxButton.Ok,
                    EnuMessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                var msgService = ServiceContainer.Instance.GetService<IMessageboxService>();
                msgService.Show($"Error while downloading the update package. {ex.Message}", "Error", EnuMessageBoxButton.Ok,
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

        #region "Commands"


        public ICommand AbortCommand { get; }

        private void AbortCommand_Execute()
        {
            DialogResult = false;
            var dialogService = ServiceContainer.Instance
                .GetService<IDialogWindowService>();
            dialogService.CloseDialog();
        }

        #endregion

    }
}
