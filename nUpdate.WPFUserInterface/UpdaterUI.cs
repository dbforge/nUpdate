// Copyright © Sascha Patschka 2018

using System;
using System.IO;
using nUpdate.Internal.Core.Localization;
using nUpdate.WPFUserInterface.ServiceInterfaces;
using nUpdate.WPFUserInterface.Services;
using nUpdate.WPFUserInterface.ViewModel;

// ReSharper disable once CheckNamespace
namespace nUpdate.Updating
{

    public sealed partial class UpdaterUI
    {
        /// <summary>
        ///     Starts the complete update process and uses the integrated user interface for user interaction.
        /// </summary>
        public async void ShowUserInterface()
        {
            if (_active)
                return;
            _active = true;

            ServiceInjector.InjectServices();

            var dialogService = ServiceContainer.Instance.GetService<IDialogWindowService>();
            var messageboxService = ServiceContainer.Instance.GetService<IMessageboxService>();

            LocalizationProperties lp = LocalizationHelper.GetLocalizationProperties(UpdateManager.LanguageCulture,
                UpdateManager.CultureFilePaths);


            try
            {
                if (!UseHiddenSearch)
                {
                    var vmUpdateSearch = new UpdateSearchViewModel(UpdateManager);
                    dialogService.ShowDialog("searchforupdates", vmUpdateSearch);

                    if (vmUpdateSearch.HasError) return;

                    if (!vmUpdateSearch.UpdatesFound)
                    {
                        messageboxService.Show(lp.NoUpdateDialogInfoText, lp.NoUpdateDialogHeader, EnuMessageBoxButton.Ok,
                            EnuMessageBoxImage.Information);
                        return;
                    }
                }
                else
                {
                    try
                    {
                        if (!await UpdateManager.SearchForUpdatesAsync()) return;
                    }
                    catch (Exception ex)
                    {
                        messageboxService.Show(ex.Message, lp.UpdateSearchErrorCaption, EnuMessageBoxButton.Ok,
                            EnuMessageBoxImage.Error);
                        return;
                    }
                }

                var vmChangelog = new WPFUserInterface.ViewModel.ChangelogViewModel(UpdateManager);
                dialogService.ShowDialog("showChangelog", vmChangelog);

                if (!vmChangelog.DialogResult) return;

                var vmDownload = new WPFUserInterface.ViewModel.DownloadUpdateViewModel(UpdateManager);
                dialogService.ShowDialog("downloadUpdates", vmDownload);

                if (!vmDownload.DialogResult) return;

                bool valid;
                try
                {
                    valid = UpdateManager.ValidatePackages();
                }
                catch (FileNotFoundException)
                {
                    messageboxService.Show(lp.PackageNotFoundErrorText, lp.PackageValidityCheckErrorCaption, EnuMessageBoxButton.Ok,
                        EnuMessageBoxImage.Error);
                    return;
                }
                catch (ArgumentException)
                {
                    messageboxService.Show(lp.InvalidSignatureErrorText, lp.PackageValidityCheckErrorCaption, EnuMessageBoxButton.Ok,
                        EnuMessageBoxImage.Error);
                    return;
                }
                catch (Exception ex)
                {
                    messageboxService.Show(ex.Message, lp.PackageValidityCheckErrorCaption, EnuMessageBoxButton.Ok,
                        EnuMessageBoxImage.Error);
                    return;
                }

                if (!valid)
                {
                    messageboxService.Show(lp.SignatureNotMatchingErrorText, lp.InvalidSignatureErrorCaption, EnuMessageBoxButton.Ok,
                        EnuMessageBoxImage.Error);
                }

                else
                    try
                    {
                        UpdateManager.InstallPackage();
                    }
                    catch (Exception ex)
                    {
                        messageboxService.Show($"{lp.SignatureNotMatchingErrorText} Error: {ex}", lp.InvalidSignatureErrorCaption, EnuMessageBoxButton.Ok,
                            EnuMessageBoxImage.Error);
                    }
            }
            finally
            {
                _active = false;
            }
        }
    }
}