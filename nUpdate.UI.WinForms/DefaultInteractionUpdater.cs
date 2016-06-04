// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using nUpdate.Localization;
using nUpdate.UI.WinForms.Dialogs;
using nUpdate.UI.WinForms.Popups;

// ReSharper disable MethodSupportsCancellation

namespace nUpdate.UI.WinForms
{
    /// <summary>
    ///     The integrated updater that interacts with the user using dialogs.
    /// </summary>
    public class DefaultInteractionUpdater
    {
        private readonly LocalizationProperties _lp = new LocalizationProperties();
        private bool _updatesAvailable;
        private bool _isTaskRunning;
        private readonly Updater _updaterInstance;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultInteractionUpdater"/> class.
        /// </summary>
        /// <param name="updater">The <see cref="Updater"/> instance that has been initialized and will be used for the updating process.</param>
        /// <param name="context">The <see cref="SynchronizationContext"/> that should be used to invoke the methods that show the dialogs.</param>
        /// <param name="useHiddenSearch">If set to <c>true</c>, nUpdate will search for updates in the background without showing a search dialog.</param>
        public DefaultInteractionUpdater(Updater updater, SynchronizationContext context, bool useHiddenSearch = false)
        {
            Context = context;
            UseBackgroundSearch = useHiddenSearch;
            _updaterInstance = updater;
        }

        /// <summary>
        ///     Gets or sets the <see cref="SynchronizationContext"/> that should be used to invoke the methods that show the dialogs.
        /// </summary>
        public SynchronizationContext Context { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="DefaultInteractionUpdater"/> should search for updates in the background without showing a search dialog, or not.
        /// </summary>
        public bool UseBackgroundSearch { get; set; }

        /// <summary>
        ///     Begins the automated update process.
        /// </summary>
        public void Begin()
        {
            if (_isTaskRunning)
                return;

            _isTaskRunning = true;
            var searchCancellationToken = new CancellationToken();
            var downloadCancellationToken = new CancellationToken();

            var searchDialog = new UpdateSearchDialog { InteractionUpdater = _updaterInstance };

            var newUpdateDialog = new NewUpdateDialog {InteractionUpdater = _updaterInstance };
            var noUpdateDialog = new NoUpdateFoundDialog { InteractionUpdater = _updaterInstance };

            // ReSharper disable once UnusedVariable
            var progressIndicator = new Microsoft.Progress<UpdateProgressData>();
            var downloadDialog = new UpdateDownloadDialog { InteractionUpdater = _updaterInstance };

            try
            {
                // ReSharper disable once MethodSupportsCancellation
                TaskEx.Run(async delegate
                {
                    if (!UseBackgroundSearch)
                        Context.Post(searchDialog.ShowModalDialog, null);

                    try
                    {
                        _updatesAvailable = await _updaterInstance.SearchForUpdatesTask(searchCancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    catch (Exception ex)
                    {
                        if (UseBackgroundSearch)
                            Context.Send(
                                o =>
                                    Popup.ShowPopup(SystemIcons.Error, _lp.UpdateSearchErrorCaption, ex,
                                        PopupButtons.Ok), null);
                        else
                        {
                            searchDialog.Fail(ex);
                            Context.Post(searchDialog.CloseDialog, null);
                        }
                        return;
                    }

                    if (!UseBackgroundSearch)
                    {
                        Context.Post(searchDialog.CloseDialog, null);
                        await TaskEx.Delay(100); // Prevents race conditions that cause that the UpdateSearchDialog can't be closed before further actions are done
                    }

                    if (_updatesAvailable)
                    {
                        var newUpdateDialogReference = new DialogResultReference();
                        Context.Send(newUpdateDialog.ShowModalDialog, newUpdateDialogReference);
                        if (newUpdateDialogReference.DialogResult == DialogResult.Cancel)
                            return;
                    }
                    else if (!_updatesAvailable && UseBackgroundSearch)
                        return;
                    else if (!_updatesAvailable && !UseBackgroundSearch)
                    {
                        var noUpdateDialogResultReference = new DialogResultReference();
                        if (!UseBackgroundSearch)
                            Context.Send(noUpdateDialog.ShowModalDialog, noUpdateDialogResultReference);
                        return;
                    }
                    
                    Context.Post(downloadDialog.ShowModalDialog, null);

                    try
                    {
                        progressIndicator.ProgressChanged += (sender, args) =>
                            downloadDialog.ProgressPercentage = (int) args.Percentage;
                        
                        await _updaterInstance.DownloadUpdatesTask(downloadCancellationToken, progressIndicator);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    catch (Exception ex)
                    {
                        downloadDialog.Fail(ex);
                        Context.Send(downloadDialog.CloseDialog, null);
                        return;
                    }
                    Context.Send(downloadDialog.CloseDialog, null);

                    bool isValid = false;
                    try
                    {
                        isValid = _updaterInstance.ValidateSignatures();
                    }
                    catch (FileNotFoundException)
                    {
                        Context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                            _lp.PackageNotFoundErrorText,
                            PopupButtons.Ok), null);
                    }
                    catch (ArgumentException)
                    {
                        Context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                            _lp.InvalidSignatureErrorText, PopupButtons.Ok), null);
                    }
                    catch (Exception ex)
                    {
                        Context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                            ex, PopupButtons.Ok), null);
                    }

                    if (!isValid)
                        Context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.InvalidSignatureErrorCaption,
                            _lp.SignatureNotMatchingErrorText,
                            PopupButtons.Ok), null);
                    else
                        _updaterInstance.InstallUpdates();
                });
            }
            finally
            {
                _isTaskRunning = false;
            }
        }
    }
}