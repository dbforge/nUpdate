// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using nUpdate.Core;
using nUpdate.Core.Localization;
using nUpdate.UI.Dialogs;
using nUpdate.UI.Popups;
using nUpdate.UpdateEventArgs;

namespace nUpdate.Updating
{
    public class UpdaterUi
    {
        private readonly LocalizationProperties _lp;
        private UpdateManager _updateManager;
        private bool _updateAvailable;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdaterUi" />-class.
        /// </summary>
        /// <param name="updateManagerInstance">The instance of the <see cref="UpdateManager" /> to handle over.</param>
        public UpdaterUi(UpdateManager updateManagerInstance)
        {
            UpdateManagerInstance = updateManagerInstance;

            string languageFilePath;
            try
            {
                languageFilePath = _updateManager.CultureFilePaths.First(item => item.Key.Equals(_updateManager.LanguageCulture)).Value;
            }
            catch (InvalidOperationException)
            {
                languageFilePath = null;
            }

            if (!String.IsNullOrEmpty(languageFilePath))
            {
                try
                {
                    _lp = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(languageFilePath));
                }
                catch (Exception)
                {
                    _lp = new LocalizationProperties();
                }
            }
            else if (String.IsNullOrEmpty(languageFilePath) && _updateManager.LanguageCulture.Name != "en")
            {
                string resourceName = String.Format("nUpdate.Core.Localization.{0}.json", _updateManager.LanguageCulture.Name);
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    _lp = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }
            else if (String.IsNullOrEmpty(languageFilePath) && _updateManager.LanguageCulture.Name == "en")
            {
                _lp = new LocalizationProperties();
            }
        }

        /// <summary>
        ///     Sets the given instance of the <see cref="UpdateManager" />-class.
        /// </summary>
        internal UpdateManager UpdateManagerInstance
        {
            get { return _updateManager; }
            set { _updateManager = value; }
        }

        internal void SearchFinishedEventHandler(object sender, UpdateSearchFinishedEventArgs e)
        {
            _updateAvailable = e.UpdateAvailable;
        }

        /// <summary>
        ///     Shows the built-in UI while the updates are managed.
        /// </summary>
        public void ShowUserInterface()
        {
            var searchDialog = new UpdateSearchDialog {LanguageName = _updateManager.LanguageCulture.Name};
            _updateManager.UpdateSearchFinished += SearchFinishedEventHandler;
            _updateManager.UpdateSearchFinished += searchDialog.SearchFinishedEventHandler;
            _updateManager.UpdateSearchFailed += searchDialog.SearchFailedEventHandler;
            _updateManager.SearchForUpdatesAsync();

            if (!_updateManager.UseHiddenSearch)
            {
                if (searchDialog.ShowDialog() == DialogResult.Cancel)
                {
                    searchDialog.Close();
                    _updateManager.CancelSearchAsync();
                    return;
                }
                searchDialog.Close();
            }

            _updateManager.UpdateSearchFinished -= SearchFinishedEventHandler;
            _updateManager.UpdateSearchFinished -= searchDialog.SearchFinishedEventHandler;
            _updateManager.UpdateSearchFailed -= searchDialog.SearchFailedEventHandler;

            if (_updateAvailable)
            {
                var newUpdateDialog = new NewUpdateDialog
                {
                    LanguageName = _updateManager.LanguageCulture.Name,
                    CurrentVersion = _updateManager.CurrentVersion,
                    PackageSize = _updateManager.PackageSize,
                    OperationAreas = _updateManager.Operations.Select(item => item.Area).ToList(),
                    NewestVersion =_updateManager.NewestVersion,
                    Changelog =  _updateManager.Changelog,
                    MustUpdate = _updateManager.MustUpdate
                };

                if (newUpdateDialog.ShowDialog() != DialogResult.OK)
                    return;
            }
            else if (!_updateAvailable && _updateManager.UseHiddenSearch)
                return;
            else if (!_updateAvailable && !_updateManager.UseHiddenSearch)
            {
                var noUpdateDialog = new NoUpdateFoundDialog {LanguageName = _updateManager.LanguageCulture.Name};
                if (noUpdateDialog.ShowDialog() == DialogResult.OK)
                    return;
            }

            var downloadDialog = new UpdateDownloadDialog {LanguageName = _updateManager.LanguageCulture.Name};

            _updateManager.PackageDownloadProgressChanged += downloadDialog.ProgressChangedEventHandler;
            _updateManager.PackageDownloadFinished += downloadDialog.DownloadFinishedEventHandler;
            _updateManager.PackageDownloadFailed += downloadDialog.DownloadFailedEventHandler;
            _updateManager.StatisticsEntryFailed += downloadDialog.StatisticsEntryFailedEventHandler;
            _updateManager.DownloadPackageAsync();

            if (downloadDialog.ShowDialog() == DialogResult.Cancel)
            {
                if (_updateManager.IsDownloading)
                    _updateManager.CancelDownloadAsync();
                return;
            }

            bool isValid = false;
            try
            {
                isValid = _updateManager.CheckPackageValidity();
            }
            catch (FileNotFoundException)
            {
                Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                    _lp.PackageNotFoundErrorText,
                    PopupButtons.Ok);
            }
            catch (ArgumentException)
            {
                Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                    _lp.InvalidSignatureErrorText, PopupButtons.Ok);
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                    ex, PopupButtons.Ok);
            }

            if (!isValid)
                Popup.ShowPopup(SystemIcons.Error, _lp.InvalidSignatureErrorCaption,
                    _lp.SignatureNotMatchingErrorText,
                    PopupButtons.Ok);
            else
                _updateManager.InstallPackage();
        }
    }
}