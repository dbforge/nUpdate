// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft;
using nUpdate.Core;
using nUpdate.Core.Localization;
using nUpdate.UI.Dialogs;
using nUpdate.UI.Popups;
using nUpdate.UpdateEventArgs;

namespace nUpdate.Updating
{
    public class UpdaterUi
    {
        private readonly ManualResetEvent _resetEvent = new ManualResetEvent(false);
        private readonly LocalizationProperties _lp;
        private SynchronizationContext _context;
        private UpdateManager _updateManager;
        private bool _updateAvailable;
        private bool _isTaskRunning;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdaterUi" />-class.
        /// </summary>
        /// <param name="updateManager">The instance of the <see cref="UpdateManager" /> to handle over.</param>
        /// <param name="context">The synchronization context to use.</param>
        public UpdaterUi(UpdateManager updateManager, SynchronizationContext context)
            : this(updateManager, context, false)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdaterUi"/> class.
        /// </summary>
        /// <param name="updateManager">The update manager.</param>
        /// <param name="context">The context.</param>
        /// <param name="useHiddenSearch">If set to <c>true</c> a hidden search will be provided in order to search in the background without informing the user.</param>
        public UpdaterUi(UpdateManager updateManager, SynchronizationContext context, bool useHiddenSearch)
        {
            UpdateManagerInstance = updateManager;
            Context = context;
            UseHiddenSearch = useHiddenSearch;

            string languageFilePath;
            try
            {
                languageFilePath =
                    _updateManager.CultureFilePaths.First(item => item.Key.Equals(_updateManager.LanguageCulture)).Value;
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
                string resourceName = String.Format("nUpdate.Core.Localization.{0}.json",
                    _updateManager.LanguageCulture.Name);
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
        ///     Gets or sets the synchronization context to use.
        /// </summary>
        internal SynchronizationContext Context
        {
            get { return _context; }
            set { _context = value; }
        }

        /// <summary>
        ///     Gets or sets the given instance of the <see cref="UpdateManager" />-class.
        /// </summary>
        internal UpdateManager UpdateManagerInstance
        {
            get { return _updateManager; }
            set { _updateManager = value; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether a hidden search should be provided in order to search in the background without informing the user, or not.
        /// </summary>
        public bool UseHiddenSearch { get; set; }

        internal void SearchFinishedEventHandler(object sender, UpdateSearchFinishedEventArgs e)
        {
            _updateAvailable = e.UpdateAvailable;
            _resetEvent.Set();
        }

        /// <summary>
        ///     Shows the built-in UI while the updates are managed.
        /// </summary>
        public void ShowUserInterface()
        {
            if (_isTaskRunning)
                return;

            var searchDialog = new UpdateSearchDialog { LanguageName = _updateManager.LanguageCulture.Name };
            searchDialog.CancelButtonClicked += UpdateSearchDialogCancelButtonClick;

            var newUpdateDialog = new NewUpdateDialog
            {
                LanguageName = _updateManager.LanguageCulture.Name,
                CurrentVersion = _updateManager.CurrentVersion,
            };

            var noUpdateDialog = new NoUpdateFoundDialog { LanguageName = _updateManager.LanguageCulture.Name };

            var progressIndicator = new Progress<UpdateDownloadProgressChangedEventArgs>();
            var downloadDialog = new UpdateDownloadDialog
            {
                LanguageName = _updateManager.LanguageCulture.Name
            };
            downloadDialog.CancelButtonClicked += UpdateDownloadDialogCancelButtonClick;

            TaskEx.Run(async delegate
            {
                _isTaskRunning = true;
                if (!UseHiddenSearch)
                    _context.Post(searchDialog.ShowModalDialog, null);
                
                try
                {
                    _updateAvailable = await _updateManager.SearchForUpdatesTask();
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    if (UseHiddenSearch)
                        _context.Send(
                            o =>
                                Popup.ShowPopup(SystemIcons.Error, _lp.UpdateSearchErrorCaption, ex,
                                    PopupButtons.Ok), null);
                    else
                    {
                        searchDialog.Fail(ex);
                        _context.Post(searchDialog.CloseDialog, null);
                    }
                    return;
                }

                if (!UseHiddenSearch)
                {
                    _context.Post(searchDialog.CloseDialog, null);
                    await TaskEx.Delay(100); // Prevents race conditions that cause that the UpdateSearchDialog can't be closed before further actions are done
                }

                if (_updateAvailable)
                {
                    newUpdateDialog.PackageSize = _updateManager.TotalSize;
                    newUpdateDialog.PackageConfigurations = _updateManager.PackageConfigurations;
                    var newUpdateDialogReference = new DialogResultReference();
                    _context.Send(newUpdateDialog.ShowModalDialog, newUpdateDialogReference);
                    if (newUpdateDialogReference.DialogResult == DialogResult.Cancel)
                        return;
                }
                else if (!_updateAvailable && UseHiddenSearch)
                    return;
                else if (!_updateAvailable && UseHiddenSearch)
                {
                    var noUpdateDialogResultReference = new DialogResultReference();
                    if (!UseHiddenSearch)
                        _context.Send(noUpdateDialog.ShowModalDialog, noUpdateDialogResultReference);
                }

                downloadDialog.PackagesCount = _updateManager.PackageConfigurations.Count();
                _context.Post(downloadDialog.ShowModalDialog, null);

                try
                {
                    progressIndicator.ProgressChanged += (sender, args) =>
                        downloadDialog.ProgressPercentage = (int) args.Percentage;
                    await _updateManager.DownloadPackagesTask(progressIndicator);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    downloadDialog.Fail(ex);
                    _context.Send(downloadDialog.CloseDialog, null);
                    return;
                }

                _context.Send(downloadDialog.CloseDialog, null);

                bool isValid = false;
                try
                {
                    isValid = _updateManager.CheckPackageValidity();
                }
                catch (FileNotFoundException)
                {
                    _context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                        _lp.PackageNotFoundErrorText,
                        PopupButtons.Ok), null);
                }
                catch (ArgumentException)
                {
                    _context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                        _lp.InvalidSignatureErrorText, PopupButtons.Ok), null);
                }
                catch (Exception ex)
                {
                    _context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                        ex, PopupButtons.Ok), null);
                }

                if (!isValid)
                    _context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.InvalidSignatureErrorCaption,
                        _lp.SignatureNotMatchingErrorText,
                        PopupButtons.Ok), null);
                else
                    _updateManager.InstallPackage();

                _isTaskRunning = false;
            });
        }

        private void UpdateSearchDialogCancelButtonClick(object sender, EventArgs e)
        {
            _updateManager.CancelSearch();
        }

        private void UpdateDownloadDialogCancelButtonClick(object sender, EventArgs e)
        {
            _updateManager.CancelDownload();
        }
    }
}