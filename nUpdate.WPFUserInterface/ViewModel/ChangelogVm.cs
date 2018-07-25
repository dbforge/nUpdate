using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using nUpdate.Internal.Core;
using nUpdate.Internal.Core.Localization;
using nUpdate.Internal.Core.Operations;
using nUpdate.ServiceInterfaces;
using nUpdate.Updating;
using nUpdate.ViewModel;
using nUpdate.ViewModel.Interfaces;

// ReSharper disable once CheckNamespace
namespace nUpdate.WPFUserInterface.ViewModel
{
    class ChangelogVm : UpdateUiBaseVm, IDialogViewModel
    {
       private readonly Icon _appIcon = IconHelper.ExtractAssociatedIcon(Application.ExecutablePath);


        #region "Constructors"

        public ChangelogVm()
        {
            if (!IsInDesignMode)
            {
                throw new Exception(
                    "This constructor is only for DesigntimeSupport!! Please use the second constructor!");
            }

            Header = "0 new updates aviable.";
            InfoText = "New updates can be downloaded for App";
            AviableVersionText = "Aviable version: 1.2.0.0";
            CurrentVersionText = "Current version: 1.1.0.0";
            UpdateSizeText = "Total package size: 12 MB";
            AccessesText = "Accesses:";

            WindowIcon = GetIcon(IconHelper.ExtractAssociatedIcon(Application.ExecutablePath));
        }

        public ChangelogVm(UpdateManager manager)
        {
            
            UpdateManager = manager;

            DoInstallCommand = new RelayCommand(o => DoInstallCommand_Execute(), o => true);
            AbortCommand = new RelayCommand(o => AbortCommand_Execute(), o => true);
            
            LocProperties = LocalizationHelper.GetLocalizationProperties(UpdateManager.LanguageCulture,
                UpdateManager.CultureFilePaths);


            Header = string.Format(
                UpdateManager.PackageConfigurations.Count() > 1
                    ? LocProperties.NewUpdateDialogMultipleUpdatesHeader
                    : LocProperties.NewUpdateDialogSingleUpdateHeader, UpdateManager.PackageConfigurations.Count());

            WindowIcon = GetIcon(_appIcon);
            WindowTitle = string.Format(LocProperties.NewUpdateDialogSingleUpdateHeader, UpdateManager.PackageConfigurations.Count());

            InfoText = string.Format(LocProperties.NewUpdateDialogInfoText, Application.ProductName);
            var availableVersions =
                UpdateManager.PackageConfigurations.Select(item => new UpdateVersion(item.LiteralVersion)).ToArray();
            AviableVersionText = string.Format(LocProperties.NewUpdateDialogAvailableVersionsText,
                UpdateManager.PackageConfigurations.Count() <= 2
                    ? string.Join(", ", availableVersions.Select(item => item.FullText))
                    : $"{UpdateVersion.GetLowestUpdateVersion(availableVersions).FullText} - {UpdateVersion.GetHighestUpdateVersion(availableVersions).FullText}");
            CurrentVersionText = string.Format(LocProperties.NewUpdateDialogCurrentVersionText,
                UpdateManager.CurrentVersion.FullText);
            
            var size = SizeHelper.ConvertSize((long) UpdateManager.TotalSize);
            UpdateSizeText = $"{string.Format(LocProperties.NewUpdateDialogSizeText, size)}";
            
            foreach (var updateConfiguration in UpdateManager.PackageConfigurations)
            {
                var versionText = new UpdateVersion(updateConfiguration.LiteralVersion).FullText;
                var changelogText = updateConfiguration.Changelog.ContainsKey(UpdateManager.LanguageCulture)
                    ? updateConfiguration.Changelog.First(item => Equals(item.Key, UpdateManager.LanguageCulture)).Value
                    : updateConfiguration.Changelog.First(item => item.Key.Name == "en").Value;

                ChangelogText +=
                    string.Format(string.IsNullOrEmpty(ChangelogText) ? "{0}:\n{1}" : "\n\n{0}:\n{1}",
                        versionText, changelogText);
            }
            if (OperationAreas == null || OperationAreas.Count == 0)
            {
                AccessesText = $"{LocProperties.NewUpdateDialogAccessText} -";
                return;
            }

            AccessesText =
                $"{LocProperties.NewUpdateDialogAccessText} {string.Join(", ", LocalizationHelper.GetLocalizedEnumerationValues(LocProperties, OperationAreas.Cast<object>().GroupBy(item => item).Select(item => item.First()).ToArray()))}";
            }

        #endregion


        #region "Properties"
        public bool DialogResult { get; set; }
        public string WindowTitle { get; set; }
        public Dispatcher CurrentDispatcher { get; set; }
        public void DialogLoaded(){}
        public void DialogClosing(){}

        public LocalizationProperties LocProperties { get; }
        public List<OperationArea> OperationAreas { get; set; }
        public string Header { get; }
        public string InfoText { get; }
        public string AviableVersionText { get; }
        public string CurrentVersionText { get; }
        public string AccessesText { get; }
        public string ChangelogText { get; }
        public string UpdateSizeText { get; }

        #endregion



        #region "Commands"

        public ICommand DoInstallCommand { get; }
    
        private void DoInstallCommand_Execute()
        {
            if (!SizeHelper.HasEnoughSpace(UpdateManager.TotalSize, out var necessarySpaceToFree))
            {
                var packageSizeString = SizeHelper.ConvertSize((long) UpdateManager.TotalSize);
                var spaceToFreeString = SizeHelper.ConvertSize((long) necessarySpaceToFree);
                
                var msgService = ServiceContainer.Instance.GetService<IMessageboxService>();
                msgService.Show($"You don't have enough disk space left on your drive and nUpdate is not able to download and install the available updates ({packageSizeString}). Please free a minimum of {spaceToFreeString} to make sure the updates can be downloaded and installed without any problems."
                    , "Not enough disk space.", EnuMessageBoxButton.Ok,
                    EnuMessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
            var dialogService = ServiceContainer.Instance
                .GetService<IDialogWindowService>();
            dialogService.CloseDialog();
        }


    

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
