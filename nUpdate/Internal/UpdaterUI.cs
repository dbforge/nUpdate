using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Windows.Forms;
using nUpdate.Core.Language;
using nUpdate.Dialogs;
using nUpdate.UI.Dialogs;

namespace nUpdate.Internal
{
    public class UpdaterUI
    {
        bool updateAvailable = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdaterUI"/>-class.
        /// </summary>
        /// <param name="updateManagerInstance">The instance of the <see cref="UpdateManager"/> to handle over.</param>
        public UpdaterUI(UpdateManager updateManagerInstance)
        {
            UpdateManagerInstance = updateManagerInstance;
        }

        /// <summary>
        /// Sets the given instance of the <see cref="UpdateManager"/>-class.
        /// </summary>
        internal UpdateManager UpdateManagerInstance { get; set; }

        internal void SearchFinishedEventHandler(bool updatesFound)
        {
            updateAvailable = updatesFound;
        }

        /// <summary>
        /// Shows the built-in UI while the updates are managed.
        /// </summary>
        public void ShowUserInterface()
        {
            var searchDialog = new UpdateSearchDialog();
            searchDialog.Language = UpdateManagerInstance.Language;
            UpdateManagerInstance.UpdateSearchFinished += this.SearchFinishedEventHandler;
            UpdateManagerInstance.UpdateSearchFinished += searchDialog.SearchFinishedEventHandler;
            UpdateManagerInstance.UpdateSearchFailed += searchDialog.SearchFailedEventHandler;

            UpdateManagerInstance.CheckForUpdatesAsync();

            if (searchDialog.ShowDialog() == DialogResult.Cancel)
            {
                searchDialog.Close();
                return;
            }
            else
                searchDialog.Close();

            UpdateManagerInstance.UpdateSearchFinished -= this.SearchFinishedEventHandler;
            UpdateManagerInstance.UpdateSearchFinished -= searchDialog.SearchFinishedEventHandler;
            UpdateManagerInstance.UpdateSearchFailed -= searchDialog.SearchFailedEventHandler;

            var newUpdateDialog = new NewUpdateDialog()
            {
                Language = UpdateManagerInstance.Language,
                CurrentVersion = UpdateManagerInstance.CurrentVersion,
                UpdateVersion = UpdateManagerInstance.UpdateVersion,
                ChangelogText = UpdateManagerInstance.Changelog,
                PackageSize = UpdateManagerInstance.PackageSize,
            };

            if (updateAvailable)
            {
                if (newUpdateDialog.ShowDialog() == DialogResult.OK)
                    newUpdateDialog.Close();

                else
                    return;
            }

            else if (!updateAvailable && UpdateManagerInstance.UseHiddenSearch)
                return;

            else if (!updateAvailable && !UpdateManagerInstance.UseHiddenSearch)
            {
                var noUpdateDialog = new NoUpdateFoundDialog();
                if (noUpdateDialog.ShowDialog() == DialogResult.OK)
                    return;
            }

            var downloadDialog = new UpdateDownloadDialog();
            downloadDialog.Language = UpdateManagerInstance.Language;

            UpdateManagerInstance.PackageDownloadProgressChanged += new DownloadProgressChangedEventHandler(downloadDialog.ProgressChangedEventHandler);
            UpdateManagerInstance.PackageDownloadFinished += new System.ComponentModel.AsyncCompletedEventHandler(downloadDialog.DownloadFinishedEventHandler);
            UpdateManagerInstance.PackageDownloadFailed += new UpdateManager.FailedEventHandler(downloadDialog.DownloadFailedEventHandler);

            UpdateManagerInstance.DownloadPackageAsync();

            if (downloadDialog.ShowDialog() == DialogResult.Cancel)
            {
                UpdateManagerInstance.DeletePackage();
                return;
            }

            else
            {
                try
                {
                    if (!UpdateManagerInstance.CheckPackageValidility())
                    {
                        string errorMessage = String.Format("The package contains an invalid signature and could be dangerous.{0}In order to avoid any security risks, nUpdate will not allow to install the package and delete it unrecoverably.",
                            Environment.NewLine);

                        var errorDialog = new UpdateErrorDialog();
                        errorDialog.ErrorCode = 0;
                        errorDialog.InfoMessage = "Invalid signature.";
                        errorDialog.ErrorMessage = errorMessage;

                        if (errorDialog.ShowDialog() == DialogResult.OK)
                            return;
                    }

                    else
                        UpdateManagerInstance.InstallPackage();
                }
                catch (FileNotFoundException)
                {
                    // TODO: Show not found
                }
                catch (NullReferenceException)
                {
                    // TODO: Show no signature
                }
            }

        }
    }
}
