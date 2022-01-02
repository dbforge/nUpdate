// UpdaterUI.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using nUpdate.Localization;
using nUpdate.UI.WindowsForms.Dialogs;
using nUpdate.UI.WindowsForms.Popups;
using nUpdate.Updating;

namespace nUpdate.UI.WindowsForms
{
    // PROVIDE TAP
    public sealed class UpdaterUI
    {
        private readonly LocalizationProperties _lp = new LocalizationProperties();
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