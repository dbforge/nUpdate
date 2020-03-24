// DefaultInteractionUpdater.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using nUpdate.Properties;
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
        private readonly CancellationTokenSource _updateCheckCancellationTokenSource = new CancellationTokenSource();
        private readonly UpdateProvider _updateProvider;
        private bool _updateProcessActive;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultInteractionUpdater" /> class.
        /// </summary>
        /// <param name="updateProvider">
        ///     The <see cref="UpdateProvider" /> instance that has been initialized and will be used for
        ///     the updating process.
        /// </param>
        /// <param name="context">
        ///     The <see cref="SynchronizationContext" /> that should be used to invoke the methods that show the
        ///     dialogs.
        /// </param>
        /// <param name="useHiddenSearch">
        ///     If set to <c>true</c>, nUpdate will search for updates in the background without showing
        ///     a search dialog.
        /// </param>
        public DefaultInteractionUpdater(UpdateProvider updateProvider, SynchronizationContext context,
            bool useHiddenSearch = false)
        {
            Context = context;
            UseBackgroundSearch = useHiddenSearch;
            _updateProvider = updateProvider;
        }

        /// <summary>
        ///     Gets or sets the <see cref="SynchronizationContext" /> that should be used to invoke the methods that show the
        ///     dialogs.
        /// </summary>
        public SynchronizationContext Context { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="DefaultInteractionUpdater" /> should search for updates in
        ///     the background without showing a search dialog, or not.
        /// </summary>
        public bool UseBackgroundSearch { get; set; }

        /// <summary>
        ///     Executes the integrated update process.
        /// </summary>
        public async void Execute()
        {
            if (_updateProcessActive)
                return;
            _updateProcessActive = true;

            try
            {
                UpdateCheckResult updateCheckResult;
                if (!UseBackgroundSearch)
                {
                    var searchDialog = new UpdateSearchDialog {UpdateProvider = _updateProvider};
                    if (searchDialog.ShowDialog() != DialogResult.OK)
                        return;

                    if (!(updateCheckResult = searchDialog.UpdateCheckResult).UpdatesFound)
                    {
                        var noUpdateDialog = new NoUpdateFoundDialog {UpdateProvider = _updateProvider};
                        noUpdateDialog.ShowDialog();
                        return;
                    }
                }
                else
                {
                    try
                    {
                        if (!(updateCheckResult =
                                await _updateProvider.CheckForUpdates(_updateCheckCancellationTokenSource.Token))
                            .UpdatesFound)
                            return;
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    catch (Exception ex)
                    {
                        Context.Send(c => Popup.ShowPopup(SystemIcons.Error, strings.UpdateSearchErrorCaption, ex,
                            PopupButtons.Ok), null);
                        return;
                    }
                }

                var newUpdateDialog = new NewUpdateDialog
                    {UpdateProvider = _updateProvider, UpdateCheckResult = updateCheckResult};
                if (newUpdateDialog.ShowDialog() != DialogResult.OK)
                    return;

                var downloadDialog = new UpdateDownloadDialog
                    {UpdateProvider = _updateProvider, UpdateCheckResult = updateCheckResult};
                if (downloadDialog.ShowDialog() != DialogResult.OK)
                    return;

                bool valid;
                try
                {
                    valid = (await _updateProvider.VerifyUpdates(updateCheckResult)).AreValid;
                }
                catch (FileNotFoundException)
                {
                    Context.Send(c => Popup.ShowPopup(SystemIcons.Error, strings.PackageValidityCheckErrorCaption,
                        strings.PackageNotFoundErrorText,
                        PopupButtons.Ok), null);
                    return;
                }
                catch (ArgumentException)
                {
                    Context.Send(c => Popup.ShowPopup(SystemIcons.Error, strings.PackageValidityCheckErrorCaption,
                        strings.InvalidSignatureErrorText, PopupButtons.Ok), null);
                    return;
                }
                catch (Exception ex)
                {
                    Context.Send(c => Popup.ShowPopup(SystemIcons.Error, strings.PackageValidityCheckErrorCaption,
                        ex, PopupButtons.Ok), null);
                    return;
                }

                if (!valid)
                    Context.Send(c => Popup.ShowPopup(SystemIcons.Error, strings.InvalidSignatureErrorCaption,
                        strings.SignatureNotMatchingErrorText,
                        PopupButtons.Ok), null);
                else
                    try
                    {
                        await _updateProvider.InstallUpdates(updateCheckResult);
                    }
                    catch (Exception ex)
                    {
                        Context.Send(c => Popup.ShowPopup(SystemIcons.Error, strings.InstallerInitializingErrorCaption,
                            ex,
                            PopupButtons.Ok), null);
                    }
            }
            finally
            {
                _updateProcessActive = false;
            }
        }
    }
}