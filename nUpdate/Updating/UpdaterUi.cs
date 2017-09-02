// Copyright © Dominic Beger 2017

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft;
using nUpdate.Core.Localization;
using nUpdate.Exceptions;
using nUpdate.UI.Dialogs;
using nUpdate.UI.Popups;
using nUpdate.UpdateEventArgs;

namespace nUpdate.Updating
{
    public class UpdaterUI
    {
        private readonly LocalizationProperties _lp;
        private readonly ManualResetEvent _searchResetEvent = new ManualResetEvent(false);
        private SynchronizationContext _context;
        private bool _isTaskRunning;
        private bool _updatesAvailable;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdaterUI" />-class.
        /// </summary>
        /// <param name="updateManager">The instance of the <see cref="UpdateManager" /> to handle over.</param>
        /// <param name="context">The synchronization context to use.</param>
        public UpdaterUI(UpdateManager updateManager, SynchronizationContext context)
            : this(updateManager, context, false)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdaterUI" /> class.
        /// </summary>
        /// <param name="updateManager">The update manager.</param>
        /// <param name="context">The context.</param>
        /// <param name="useHiddenSearch">
        ///     If set to <c>true</c> a hidden search will be provided in order to search in the
        ///     background without informing the user.
        /// </param>
        public UpdaterUI(UpdateManager updateManager, SynchronizationContext context, bool useHiddenSearch)
        {
            UpdateManagerInstance = updateManager;
            Context = context;
            UseHiddenSearch = useHiddenSearch;
            _lp = LocalizationHelper.GetLocalizationProperties(UpdateManagerInstance.LanguageCulture,
                UpdateManagerInstance.CultureFilePaths);
        }

        /// <summary>
        ///     Gets or sets the synchronization context to use.
        /// </summary>
        internal SynchronizationContext Context
        {
            get => _context;
            set
            {
                _context = value;
                UpdateManagerInstance.Context = value;
            }
        }

        /// <summary>
        ///     Gets or sets the given instance of the <see cref="UpdateManager" />-class.
        /// </summary>
        internal UpdateManager UpdateManagerInstance { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether a hidden search should be provided in order to search in the background
        ///     without informing the user, or not.
        /// </summary>
        public bool UseHiddenSearch { get; set; }

        private void SearchFinished(object sender, UpdateSearchFinishedEventArgs e)
        {
            _updatesAvailable = e.UpdatesAvailable;
            _searchResetEvent.Set();
        }

        /// <summary>
        ///     Shows the built-in UI while the updates are managed.
        /// </summary>
        public void ShowUserInterface()
        {
            if (_isTaskRunning)
                return;

            _isTaskRunning = true;
            var searchDialog = new UpdateSearchDialog {Updater = UpdateManagerInstance};
            searchDialog.CancelButtonClicked += UpdateSearchDialogCancelButtonClick;

            var newUpdateDialog = new NewUpdateDialog {Updater = UpdateManagerInstance};
            var noUpdateDialog = new NoUpdateFoundDialog {Updater = UpdateManagerInstance};

            var progressIndicator = new Progress<UpdateDownloadProgressChangedEventArgs>();
            var downloadDialog = new UpdateDownloadDialog {Updater = UpdateManagerInstance};
            downloadDialog.CancelButtonClicked += UpdateDownloadDialogCancelButtonClick;

#if PROVIDE_TAP

            try
            {
                // TAP
                TaskEx.Run(async delegate
                {
                    if (!UseHiddenSearch)
                        _context.Post(searchDialog.ShowModalDialog, null);

                    try
                    {
                        _updatesAvailable = await UpdateManagerInstance.SearchForUpdatesAsync();
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    catch (Exception ex)
                    {
                        if (UseHiddenSearch)
                            _context.Send(
                                o =>
                                    Popup.ShowPopup(SystemIcons.Error, _lp.UpdateSearchErrorCaption, ex,
                                        PopupButtons.Ok), null);
                        else
                        {
                            searchDialog.Fail(ex);
                            _context.Post(searchDialog.CloseDialog, null);
                        }
                        return;
                    }

                    if (!UseHiddenSearch)
                    {
                        _context.Post(searchDialog.CloseDialog, null);
                        await TaskEx.Delay(100);
                            // Prevents race conditions that cause that the UpdateSearchDialog can't be closed before further actions are done
                    }

                    if (_updatesAvailable)
                    {
                        var newUpdateDialogReference = new DialogResultReference();
                        _context.Send(newUpdateDialog.ShowModalDialog, newUpdateDialogReference);
                        if (newUpdateDialogReference.DialogResult == DialogResult.Cancel)
                            return;
                    }
                    else if (!_updatesAvailable && UseHiddenSearch)
                        return;
                    else if (!_updatesAvailable && !UseHiddenSearch)
                    {
                        var noUpdateDialogResultReference = new DialogResultReference();
                        if (!UseHiddenSearch)
                            _context.Send(noUpdateDialog.ShowModalDialog, noUpdateDialogResultReference);
                        return;
                    }
            
                    _context.Post(downloadDialog.ShowModalDialog, null);

                    try
                    {
                        progressIndicator.ProgressChanged += (sender, args) =>
                            downloadDialog.ProgressPercentage = (int) args.Percentage;

                        await UpdateManagerInstance.DownloadPackagesAsync(progressIndicator);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    catch (StatisticsException ex)
                    {
                        downloadDialog.StatisticsEntryFail(ex);
                    }
                    catch (Exception ex)
                    {
                        downloadDialog.Fail(ex);
                        _context.Send(downloadDialog.CloseDialog, null);
                        return;
                    }
                    _context.Send(downloadDialog.CloseDialog, null);

                    bool isValid = false;
                    try
                    {
                        isValid = UpdateManagerInstance.ValidatePackages();
                    }
                    catch (FileNotFoundException)
                    {
                        _context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                            _lp.PackageNotFoundErrorText,
                            PopupButtons.Ok), null);
                    }
                    catch (ArgumentException)
                    {
                        _context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                            _lp.InvalidSignatureErrorText, PopupButtons.Ok), null);
                    }
                    catch (Exception ex)
                    {
                        _context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                            ex, PopupButtons.Ok), null);
                    }

                    if (!isValid)
                        _context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.InvalidSignatureErrorCaption,
                            _lp.SignatureNotMatchingErrorText,
                            PopupButtons.Ok), null);
                    else
                    {
                        try
                        {
                            UpdateManagerInstance.InstallPackage();
                        }
                        catch (Win32Exception ex)
                        {
                            // TODO: Localize
                            _context.Send(o => Popup.ShowPopup(SystemIcons.Error, "Error while starting the installer.",
                                ex,
                                PopupButtons.Ok), null);
                        }
                        catch (Exception ex)
                        {
                            _context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.InstallerInitializingErrorCaption,
                                ex,
                                PopupButtons.Ok), null);
                        }
                    }
                });
            }
            finally
            {
                _isTaskRunning = false;
            }

#else
            try
            {
                //EAP
                UpdateManagerInstance.UpdateSearchFinished += SearchFinished;
                UpdateManagerInstance.UpdateSearchFinished += searchDialog.Finished;
                UpdateManagerInstance.UpdateSearchFailed += searchDialog.Failed;
                UpdateManagerInstance.PackagesDownloadProgressChanged += downloadDialog.ProgressChanged;
                UpdateManagerInstance.PackagesDownloadFinished += downloadDialog.Finished;
                UpdateManagerInstance.PackagesDownloadFailed += downloadDialog.Failed;
                UpdateManagerInstance.StatisticsEntryFailed += downloadDialog.StatisticsEntryFailed;

                Task.Factory.StartNew(() =>
                {
                    UpdateManagerInstance.SearchForUpdatesAsync();
                    if (!UseHiddenSearch)
                    {
                        var searchDialogResultReference = new DialogResultReference();
                        _context.Send(searchDialog.ShowModalDialog, searchDialogResultReference);
                        _context.Send(searchDialog.CloseDialog, null);
                        if (searchDialogResultReference.DialogResult == DialogResult.Cancel)
                            return;
                    }
                    else
                    {
                        _searchResetEvent.WaitOne();
                    }

                    if (_updatesAvailable)
                    {
                        var newUpdateDialogResultReference = new DialogResultReference();
                        _context.Send(newUpdateDialog.ShowModalDialog, newUpdateDialogResultReference);
                        if (newUpdateDialogResultReference.DialogResult == DialogResult.Cancel)
                            return;
                    }
                    else if (!_updatesAvailable && UseHiddenSearch)
                    {
                        return;
                    }
                    else if (!_updatesAvailable && !UseHiddenSearch)
                    {
                        _context.Send(noUpdateDialog.ShowModalDialog, null);
                        _context.Send(noUpdateDialog.CloseDialog, null);
                        return;
                    }

                    UpdateManagerInstance.DownloadPackagesAsync();

                    var downloadDialogResultReference = new DialogResultReference();
                    _context.Send(downloadDialog.ShowModalDialog, downloadDialogResultReference);
                    _context.Send(downloadDialog.CloseDialog, null);
                    if (downloadDialogResultReference.DialogResult == DialogResult.Cancel)
                        return;

                    var isValid = false;
                    try
                    {
                        isValid = UpdateManagerInstance.ValidatePackages();
                    }
                    catch (FileNotFoundException)
                    {
                        _context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                            _lp.PackageNotFoundErrorText,
                            PopupButtons.Ok), null);
                    }
                    catch (ArgumentException)
                    {
                        _context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                            _lp.InvalidSignatureErrorText, PopupButtons.Ok), null);
                    }
                    catch (Exception ex)
                    {
                        _context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                            ex, PopupButtons.Ok), null);
                    }

                    if (!isValid)
                        _context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.InvalidSignatureErrorCaption,
                            _lp.SignatureNotMatchingErrorText,
                            PopupButtons.Ok), null);
                    else
                    {
                        try
                        {
                            UpdateManagerInstance.InstallPackage();
                        }
                        catch (Win32Exception ex)
                        {
                            // TODO: Localize
                            _context.Send(o => Popup.ShowPopup(SystemIcons.Error, "Error while starting the installer.",
                                ex,
                                PopupButtons.Ok), null);
                        }
                        catch (Exception ex)
                        {
                            _context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.InstallerInitializingErrorCaption,
                                ex,
                                PopupButtons.Ok), null);
                        }
                    }
                });
            }
            finally
            {
                _isTaskRunning = false;
            }
#endif
        }

        private void UpdateDownloadDialogCancelButtonClick(object sender, EventArgs e)
        {
            UpdateManagerInstance.CancelDownload();
        }

        private void UpdateSearchDialogCancelButtonClick(object sender, EventArgs e)
        {
            UpdateManagerInstance.CancelSearch();
        }
    }
}