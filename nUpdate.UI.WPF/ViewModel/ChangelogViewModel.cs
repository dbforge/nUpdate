// ChangelogViewModel.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System.Drawing;
using System.Linq;
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
    public class ChangelogViewModel : UpdateUiBaseViewModel, IDialogViewModel
    {
        private readonly Icon _appIcon = IconHelper.ExtractAssociatedIcon(Application.ExecutablePath);


        public ChangelogViewModel(UpdateManager manager)
        {
            UpdateManager = manager;

            DoInstall = new RelayCommand(o => DoInstall_Execute(), o => true);
            Abort = new RelayCommand(o => Abort_Execute(), o => true);

            LocProperties = LocalizationHelper.GetLocalizationProperties(UpdateManager.LanguageCulture,
                UpdateManager.CultureFilePaths);


            Header = string.Format(
                UpdateManager.PackageConfigurations.Count() > 1
                    ? LocProperties.NewUpdateDialogMultipleUpdatesHeader
                    : LocProperties.NewUpdateDialogSingleUpdateHeader, UpdateManager.PackageConfigurations.Count());

            WindowIcon = GetIcon(_appIcon);
            WindowTitle = string.Format(LocProperties.NewUpdateDialogSingleUpdateHeader,
                UpdateManager.PackageConfigurations.Count());

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

            /*if (OperationAreas == null || OperationAreas.Count == 0)
            {
                AccessesText = $"{LocProperties.NewUpdateDialogAccessText} -";
                return;
            }

            AccessesText =
                $"{LocProperties.NewUpdateDialogAccessText} {string.Join(", ", LocalizationHelper.GetLocalizedEnumerationValues(LocProperties, OperationAreas.Cast<object>().GroupBy(item => item).Select(item => item.First()).ToArray()))}";*/
        }

        public LocalizationProperties LocProperties { get; }
        public string Header { get; }
        public string InfoText { get; }
        public string AviableVersionText { get; }
        public string CurrentVersionText { get; }
        public string AccessesText { get; }
        public string ChangelogText { get; }
        public string UpdateSizeText { get; }


        public ICommand DoInstall { get; }


        public ICommand Abort { get; }


        public bool DialogResult { get; set; }
        public string WindowTitle { get; set; }
        public Dispatcher CurrentDispatcher { get; set; }

        public void DialogLoaded()
        {
        }

        public void DialogClosing()
        {
        }

        private void DoInstall_Execute()
        {
            if (!SizeHelper.HasEnoughSpace(UpdateManager.TotalSize, out var necessarySpaceToFree))
            {
                var packageSizeString = SizeHelper.ConvertSize((long) UpdateManager.TotalSize);
                var spaceToFreeString = SizeHelper.ConvertSize((long) necessarySpaceToFree);

                var msgService = ServiceContainer.Instance.GetService<IMessageboxService>();
                msgService.Show(
                    $"You don't have enough disk space left on your drive and nUpdate is not able to download and install the available updates ({packageSizeString}). Please free a minimum of {spaceToFreeString} to make sure the updates can be downloaded and installed without any problems."
                    , "Not enough disk space.", EnuMessageBoxButton.Ok,
                    EnuMessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            var dialogService = ServiceContainer.Instance
                .GetService<IDialogWindowService>();
            dialogService.CloseDialog();
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