// Copyright © Dominic Beger 2017

using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using nUpdate.UI.Dialogs;
using nUpdate.UI.Popups;

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
            
            try
            {
                if (!UseHiddenSearch)
                {
                    var searchDialog = new UpdateSearchDialog {UpdateManager = UpdateManager};
                    if (searchDialog.ShowDialog() != DialogResult.OK)
                        return;

                    if (!searchDialog.UpdatesFound)
                    {
                        var noUpdateDialog = new NoUpdateFoundDialog {UpdateManager = UpdateManager};
                        noUpdateDialog.ShowDialog();
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
                        Context.Send(c => Popup.ShowPopup(SystemIcons.Error, _lp.UpdateSearchErrorCaption, args.Exception,
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
                if (newUpdateDialog.ShowDialog() != DialogResult.OK)
                    return;

                var downloadDialog = new UpdateDownloadDialog {UpdateManager = UpdateManager};
                if (downloadDialog.ShowDialog() != DialogResult.OK)
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
                }
                catch (ArgumentException)
                {
                    Context.Send(c => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                        _lp.InvalidSignatureErrorText, PopupButtons.Ok), null);
                }
                catch (Exception ex)
                {
                    Context.Send(c => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                        ex, PopupButtons.Ok), null);
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
        }
    }
}