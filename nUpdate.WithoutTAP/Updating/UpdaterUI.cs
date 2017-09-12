// Copyright © Dominic Beger 2017

using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using nUpdate.Internal.UI.Popups;
using nUpdate.UI.Dialogs;

namespace nUpdate.Updating
{
    // WITHOUT TAP
    public sealed partial class UpdaterUI
    {
        private readonly ManualResetEvent _searchResetEvent = new ManualResetEvent(false);

        /// <summary>
        ///     Starts the complete update process and uses the integrated user interface for user interaction.
        /// </summary>
        public void ShowUserInterface()
        {
            if (_active)
                return;
            _active = true;
            _searchResetEvent.Reset();

            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (!UseHiddenSearch)
                    {
                        var searchDialog = new UpdateSearchDialog {UpdateManager = UpdateManager};
                        var searchDialogResult = new DialogResultWrapper();
                        Context.Send(d => searchDialog.ShowDialog(searchDialogResult), null);
                        if (searchDialogResult.DialogResult != DialogResult.OK)
                            return;

                        if (!searchDialog.UpdatesFound)
                        {
                            var noUpdateDialog = new NoUpdateFoundDialog {UpdateManager = UpdateManager};
                            var noUpdateDialogResult = new DialogResultWrapper();
                            Context.Send(d => noUpdateDialog.ShowDialog(noUpdateDialogResult), null);
                            if (noUpdateDialogResult.DialogResult != DialogResult.OK)
                                return;
                        }
                    }
                    else
                    {
                        var failed = false;
                        var updatesFound = false;

                        UpdateManager.UpdateSearchFailed += (sender, args) =>
                        {
                            failed = true;

                            // Important: The UI thread that we want to access using the synchronization context is blocked until we set the manual reset event.
                            // This call needs to be done first, otherwise we'll experience a deadlock as SynchronizationContext.Send is sending a synchronous message.
                            _searchResetEvent.Set();
                            Context.Send(c => Popup.ShowPopup(SystemIcons.Error, _lp.UpdateSearchErrorCaption,
                                args.Exception,
                                PopupButtons.Ok), null);
                        };

                        UpdateManager.UpdateSearchFinished += (sender, args) =>
                        {
                            updatesFound = args.UpdatesAvailable;
                            _searchResetEvent.Set();
                        };

                        UpdateManager.SearchForUpdatesAsync();
                        _searchResetEvent.WaitOne();

                        if (failed || !updatesFound)
                            return;
                    }

                    var newUpdateDialog = new NewUpdateDialog {UpdateManager = UpdateManager};
                    var newUpdateDialogResult = new DialogResultWrapper();
                    Context.Send(d => newUpdateDialog.ShowDialog(newUpdateDialogResult), null);
                    if (newUpdateDialogResult.DialogResult != DialogResult.OK)
                        return;

                    var downloadDialog = new UpdateDownloadDialog {UpdateManager = UpdateManager};
                    var downloadDialogResult = new DialogResultWrapper();
                    Context.Send(d => downloadDialog.ShowDialog(downloadDialogResult), null);
                    if (downloadDialogResult.DialogResult != DialogResult.OK)
                        return;

                    var valid = false;
                    try
                    {
                        valid = UpdateManager.ValidatePackages();
                    }
                    catch (FileNotFoundException)
                    {
                        Context.Send(c => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                            _lp.PackageNotFoundErrorText,
                            PopupButtons.Ok), null);
                        return;
                    }
                    catch (ArgumentException)
                    {
                        Context.Send(c => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                            _lp.InvalidSignatureErrorText, PopupButtons.Ok), null);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Context.Send(c => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                            ex, PopupButtons.Ok), null);
                        return;
                    }

                    if (!valid)
                        Context.Send(c => Popup.ShowPopup(SystemIcons.Error, _lp.InvalidSignatureErrorCaption,
                            _lp.SignatureNotMatchingErrorText,
                            PopupButtons.Ok), null);
                    else
                        try
                        {
                            UpdateManager.InstallPackage();
                        }
                        catch (Exception ex)
                        {
                            Context.Send(c => Popup.ShowPopup(SystemIcons.Error, _lp.InstallerInitializingErrorCaption,
                                ex,
                                PopupButtons.Ok), null);
                        }
                }
                finally
                {
                    _active = false;
                }
            });
        }
    }
}