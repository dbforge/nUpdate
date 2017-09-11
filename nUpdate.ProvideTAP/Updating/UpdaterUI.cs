// Copyright © Dominic Beger 2017

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using nUpdate.Internal.UI.Popups;
using nUpdate.UI.Dialogs;

namespace nUpdate.Updating
{
    // PROVIDE TAP
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
                    try
                    {
                        if (!await UpdateManager.SearchForUpdatesAsync())
                            return;
                    }
                    catch (Exception ex)
                    {
                        Context.Send(c => Popup.ShowPopup(SystemIcons.Error, _lp.UpdateSearchErrorCaption, ex,
                            PopupButtons.Ok), null);
                        return;
                    }
                }

                var newUpdateDialog = new NewUpdateDialog {UpdateManager = UpdateManager};
                if (newUpdateDialog.ShowDialog() != DialogResult.OK)
                    return;

                var downloadDialog = new UpdateDownloadDialog {UpdateManager = UpdateManager};
                if (downloadDialog.ShowDialog() != DialogResult.OK)
                    return;

                bool valid;
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
        }
    }
}