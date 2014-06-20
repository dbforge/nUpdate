using nUpdate.Dialogs;
using nUpdate.UI.Dialogs;
using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace nUpdate.Internal
{
    public class UpdaterUI
    {
        private bool updateAvailable = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdaterUI"/>-class.
        /// </summary>
        /// <param name="updateManagerInstance">The instance of the <see cref="UpdateManager"/> to handle over.</param>
        public UpdaterUI(UpdateManager updateManagerInstance)
        {
            this.UpdateManagerInstance = updateManagerInstance;
        }

        /// <summary>
        /// Sets the given instance of the <see cref="UpdateManager"/>-class.
        /// </summary>
        internal UpdateManager UpdateManagerInstance { get; set; }

        internal void SearchFinishedEventHandler(bool updatesFound)
        {
            this.updateAvailable = updatesFound;
        }

        /// <summary>
        /// Shows the built-in UI while the updates are managed.
        /// </summary>
        public void ShowUserInterface()
        {
            var searchDialog = new UpdateSearchDialog();
            searchDialog.Language = UpdateManagerInstance.Language;
            this.UpdateManagerInstance.UpdateSearchFinished += this.SearchFinishedEventHandler;
            this.UpdateManagerInstance.UpdateSearchFinished += searchDialog.SearchFinishedEventHandler;
            this.UpdateManagerInstance.UpdateSearchFailed += searchDialog.SearchFailedEventHandler;

            this.UpdateManagerInstance.CheckForUpdatesAsync();

            if (searchDialog.ShowDialog() == DialogResult.Cancel)
            {
                searchDialog.Close();
                return;
            }
            else
            {
                searchDialog.Close();
            }

            this.UpdateManagerInstance.UpdateSearchFinished -= this.SearchFinishedEventHandler;
            this.UpdateManagerInstance.UpdateSearchFinished -= searchDialog.SearchFinishedEventHandler;
            this.UpdateManagerInstance.UpdateSearchFailed -= searchDialog.SearchFailedEventHandler;

            var newUpdateDialog = new NewUpdateDialog()
            {
                Language = this.UpdateManagerInstance.Language,
                CurrentVersion = this.UpdateManagerInstance.CurrentVersion,
                UpdateVersion = this.UpdateManagerInstance.UpdateVersion,
                ChangelogText = this.UpdateManagerInstance.Changelog,
                PackageSize = this.UpdateManagerInstance.PackageSize,
            };

            if (this.updateAvailable)
            {
                if (newUpdateDialog.ShowDialog() == DialogResult.OK)
                {
                    newUpdateDialog.Close();
                }
                else
                {
                    return;
                }
            }
            else if (!this.updateAvailable && this.UpdateManagerInstance.UseHiddenSearch)
            {
                return;
            }
            else if (!this.updateAvailable && !this.UpdateManagerInstance.UseHiddenSearch)
            {
                var noUpdateDialog = new NoUpdateFoundDialog();
                if (noUpdateDialog.ShowDialog() == DialogResult.OK)
                {
                    return;
                }
            }

            var downloadDialog = new UpdateDownloadDialog();
            downloadDialog.Language = UpdateManagerInstance.Language;

            this.UpdateManagerInstance.PackageDownloadProgressChanged += new DownloadProgressChangedEventHandler(downloadDialog.ProgressChangedEventHandler);
            this.UpdateManagerInstance.PackageDownloadFinished += new System.ComponentModel.AsyncCompletedEventHandler(downloadDialog.DownloadFinishedEventHandler);
            this.UpdateManagerInstance.PackageDownloadFailed += new UpdateManager.FailedEventHandler(downloadDialog.DownloadFailedEventHandler);

            this.UpdateManagerInstance.DownloadPackageAsync();

            if (downloadDialog.ShowDialog() == DialogResult.Cancel)
            {
                this.UpdateManagerInstance.DeletePackage();
                return;
            }

            else
            {
                try
                {
                    if (!this.UpdateManagerInstance.CheckPackageValidility())
                    {
                        string errorMessage = String.Format("The package contains an invalid signature and could be dangerous.{0}In order to avoid any security risks, nUpdate will not allow to install the package and delete it unrecoverably.", Environment.NewLine);

                        var errorDialog = new UpdateErrorDialog();
                        errorDialog.ErrorCode = 0;
                        errorDialog.InfoMessage = "Invalid signature.";
                        errorDialog.ErrorMessage = errorMessage;

                        if (errorDialog.ShowDialog() == DialogResult.OK)
                        {
                            return;
                        }
                    }
                    else
                    {
                        this.UpdateManagerInstance.InstallPackage();
                    }
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
