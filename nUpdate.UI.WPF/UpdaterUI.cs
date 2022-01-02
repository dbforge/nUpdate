// UpdaterUI.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.IO;
using System.Threading;
using nUpdate.Localization;
using nUpdate.UI.WPF.ServiceInterfaces;
using nUpdate.UI.WPF.Services;
using nUpdate.UI.WPF.ViewModel;
using nUpdate.Updating;

// ReSharper disable once CheckNamespace
namespace nUpdate.UI.WPF
{
    public sealed class UpdaterUI
    {
        private bool _active;

        public UpdaterUI(UpdateManager updateManager, SynchronizationContext context)
        {
            UpdateManager = updateManager;
            Context = context;
        }


        /// <summary>
        ///     Gets or sets the <see cref="SynchronizationContext" /> to use for mashalling the user interface specific calls to
        ///     the current UI thread.
        /// </summary>
        internal SynchronizationContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the given instance of the <see cref="UpdateManager" />-class.
        /// </summary>
        internal UpdateManager UpdateManager { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether a hidden search should be provided in order to search in the background
        ///     without informing the user, or not.
        /// </summary>
        public bool UseHiddenSearch { get; set; }

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

            var lp = LocalizationHelper.GetLocalizationProperties(UpdateManager.LanguageCulture,
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
                        messageboxService.Show(lp.NoUpdateDialogInfoText, lp.NoUpdateDialogHeader,
                            EnuMessageBoxButton.Ok,
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

                var vmChangelog = new ChangelogViewModel(UpdateManager);
                dialogService.ShowDialog("showChangelog", vmChangelog);

                if (!vmChangelog.DialogResult) return;

                var vmDownload = new DownloadUpdateViewModel(UpdateManager);
                dialogService.ShowDialog("downloadUpdates", vmDownload);

                if (!vmDownload.DialogResult) return;

                bool valid;
                try
                {
                    valid = UpdateManager.ValidatePackages();
                }
                catch (FileNotFoundException)
                {
                    messageboxService.Show(lp.PackageNotFoundErrorText, lp.PackageValidityCheckErrorCaption,
                        EnuMessageBoxButton.Ok,
                        EnuMessageBoxImage.Error);
                    return;
                }
                catch (ArgumentException)
                {
                    messageboxService.Show(lp.InvalidSignatureErrorText, lp.PackageValidityCheckErrorCaption,
                        EnuMessageBoxButton.Ok,
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
                    messageboxService.Show(lp.SignatureNotMatchingErrorText, lp.InvalidSignatureErrorCaption,
                        EnuMessageBoxButton.Ok,
                        EnuMessageBoxImage.Error);

                else
                    try
                    {
                        UpdateManager.InstallPackage();
                    }
                    catch (Exception ex)
                    {
                        messageboxService.Show($"{lp.SignatureNotMatchingErrorText} Error: {ex}",
                            lp.InvalidSignatureErrorCaption, EnuMessageBoxButton.Ok,
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