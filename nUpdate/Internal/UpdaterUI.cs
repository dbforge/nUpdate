using System;
using System.IO;
using System.Windows.Forms;
using nUpdate.Dialogs;
using nUpdate.UI.Dialogs;

namespace nUpdate.Internal
{
    public class UpdaterUI
    {
        private bool _updateAvailable;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdaterUI" />-class.
        /// </summary>
        /// <param name="updateManagerInstance">The instance of the <see cref="UpdateManager" /> to handle over.</param>
        public UpdaterUI(UpdateManager updateManagerInstance)
        {
            UpdateManagerInstance = updateManagerInstance;
        }

        /// <summary>
        ///     Sets the given instance of the <see cref="UpdateManager" />-class.
        /// </summary>
        internal UpdateManager UpdateManagerInstance { get; set; }

        internal void SearchFinishedEventHandler(bool updatesFound)
        {
            _updateAvailable = updatesFound;
        }

        /// <summary>
        ///     Shows the built-in UI while the updates are managed.
        /// </summary>
        public void ShowUserInterface()
        {
            var searchDialog = new UpdateSearchDialog {LanguageName = UpdateManagerInstance.LanguageCulture.Name};
            UpdateManagerInstance.UpdateSearchFinished += SearchFinishedEventHandler;
            UpdateManagerInstance.UpdateSearchFinished += searchDialog.SearchFinishedEventHandler;
            UpdateManagerInstance.UpdateSearchFailed += searchDialog.SearchFailedEventHandler;

            UpdateManagerInstance.SearchForUpdatesAsync();

            if (!UpdateManagerInstance.UseHiddenSearch)
            {
                if (searchDialog.ShowDialog() == DialogResult.Cancel)
                {
                    searchDialog.Close();
                    return;
                }
                searchDialog.Close();
            }

            UpdateManagerInstance.UpdateSearchFinished -= SearchFinishedEventHandler;
            UpdateManagerInstance.UpdateSearchFinished -= searchDialog.SearchFinishedEventHandler;
            UpdateManagerInstance.UpdateSearchFailed -= searchDialog.SearchFailedEventHandler;

            if (_updateAvailable)
            {
                var newUpdateDialog = new NewUpdateDialog
                {
                    LanguageName = UpdateManagerInstance.LanguageCulture.Name,
                    CurrentVersion = UpdateManagerInstance.CurrentVersion,
                    UpdateVersion = UpdateManagerInstance.UpdateVersion,
                    Changelog = UpdateManagerInstance.Changelog,
                    PackageSize = UpdateManagerInstance.PackageSize,
                    MustUpdate = UpdateManagerInstance.MustUpdate,
                    OperationAreas = UpdateManagerInstance.OperationAreas,
                };
                if (newUpdateDialog.ShowDialog() == DialogResult.OK)
                    newUpdateDialog.Close();
                else
                {
                    newUpdateDialog.Close();
                    return;
                }
            }
            else if (!_updateAvailable && UpdateManagerInstance.UseHiddenSearch)
                return;
            else if (!_updateAvailable && !UpdateManagerInstance.UseHiddenSearch)
            {
                var noUpdateDialog = new NoUpdateFoundDialog {LanguageName = UpdateManagerInstance.LanguageCulture.Name};
                if (noUpdateDialog.ShowDialog() == DialogResult.OK)
                    return;
            }

            var downloadDialog = new UpdateDownloadDialog {LanguageName = UpdateManagerInstance.LanguageCulture.Name};

            UpdateManagerInstance.PackageDownloadProgressChanged += downloadDialog.ProgressChangedEventHandler;
            UpdateManagerInstance.PackageDownloadFinished += downloadDialog.DownloadFinishedEventHandler;
            //UpdateManagerInstance.PackageDownloadFailed += downloadDialog.DownloadFailedEventHandler;

            UpdateManagerInstance.DownloadPackageAsync();

            if (downloadDialog.ShowDialog() == DialogResult.Cancel)
            {
                UpdateManagerInstance.CancelDownload();
                return;
            }

            try
            {
                if (!UpdateManagerInstance.CheckPackageValidity())
                {
                    string errorMessage =
                        String.Format(
                            "The package contains an invalid signature and could be dangerous.{0}In order to avoid any security risks, nUpdate will not allow to install the package and delete it unrecoverably.",
                            Environment.NewLine);

                    var errorDialog = new UpdateErrorDialog
                    {
                        ErrorCode = 0,
                        InfoMessage = "Invalid signature.",
                        Error = new Exception(errorMessage)
                    };

                    if (errorDialog.ShowDialog() == DialogResult.OK)
                    {
                    }
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