using System;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using nUpdate.Internal.Core;
using nUpdate.Internal.Core.Localization;
using nUpdate.ServiceInterfaces;
using nUpdate.Updating;
using nUpdate.ViewModel;
using nUpdate.ViewModel.Interfaces;

// ReSharper disable once CheckNamespace
namespace nUpdate.WPFUserInterface.ViewModel
{
    class UpdateSearchVm : UpdateUiBaseVm, IDialogViewModel
    {
        internal bool UpdatesFound { get; set; }


        #region "Constructors"

        public UpdateSearchVm()
        {
            if (!IsInDesignMode)
            {
                throw new Exception(
                    "This constructor is only for DesigntimeSupport!! Please use the second constructor!");
            }
            }

        public UpdateSearchVm(UpdateManager manager)
        {
             UpdateManager = manager;
            LocProperties = LocalizationHelper.GetLocalizationProperties(UpdateManager.LanguageCulture,
                UpdateManager.CultureFilePaths);
            
            WindowIcon = GetIcon(IconHelper.ExtractAssociatedIcon(Application.ExecutablePath));
            WindowTitle = LocProperties.UpdateSearchDialogHeader;
           
            AbortCommand = new RelayCommand(o => AbortCommand_Execute(), o => true);
        }

        #endregion


        #region "Properties"

        internal bool HasError { get; set; }
        public LocalizationProperties LocProperties { get; }
        public bool DialogResult { get; set; }
        public string WindowTitle { get; set; }
        public Dispatcher CurrentDispatcher { get; set; }

        #endregion



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

        

        #region "Commands"

        public ICommand AbortCommand { get; }
    
        private void AbortCommand_Execute()
        {
            UpdatesFound = false;
            var dialogService = ServiceContainer.Instance.GetService<IDialogWindowService>();
            dialogService.CloseDialog();

        }

        #endregion


    }
}
