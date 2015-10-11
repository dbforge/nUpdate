// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft;
using nUpdate.Localization;
using nUpdate.UI.Dialogs;
using nUpdate.UI.Popups;
using nUpdate.UpdateEventArgs;

namespace nUpdate.Updating
{
    public class UpdaterUI
    {
        private readonly ManualResetEvent _searchResetEvent = new ManualResetEvent(false);
        private readonly LocalizationProperties _lp = new LocalizationProperties();
        private bool _updatesAvailable;
        private bool _requirementMiss;
        private Dictionary<UpdateVersion, List<UpdateRequirement>> _missingRequirements;
        private bool _isTaskRunning;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdaterUI" />-class.
        /// </summary>
        /// <param name="updateManager">The instance of the <see cref="Updating.UpdateManager" /> to use in the background.</param>
        /// <param name="context">The <see cref="SynchronizationContext"/> that should be used to invoke the methods that show the dialogs.</param>
        public UpdaterUI(UpdateManager updateManager, SynchronizationContext context)
            : this(updateManager, context, false)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdaterUI"/> class.
        /// </summary>
        /// <param name="updateManager">The instance of the <see cref="Updating.UpdateManager" /> to use in the background.</param>
        /// <param name="context">The <see cref="SynchronizationContext"/> that should be used to invoke the methods that show the dialogs.</param>
        /// <param name="useHiddenSearch">If set to <c>true</c>, nUpdate will search for updates in the background without showing a search dialog.</param>
        public UpdaterUI(UpdateManager updateManager, SynchronizationContext context, bool useHiddenSearch)
        {
            UpdateManager = updateManager;
            Context = context;
            UseHiddenSearch = useHiddenSearch;

            string languageFilePath = null;
            if (UpdateManager.CultureFilePaths.Any(item => item.Key.Equals(UpdateManager.LanguageCulture)))
            {
                languageFilePath =
                    UpdateManager.CultureFilePaths.First(
                        item => item.Key.Equals(UpdateManager.LanguageCulture)).Value;
            }

            if (!string.IsNullOrEmpty(languageFilePath))
            {
                try
                {
                    _lp = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(languageFilePath));
                }
                catch (Exception ex)
                {
                    Debug.Print($"Deserializing the language data from \"{languageFilePath}\" failed: {ex.Message}");
                }
            }
            else if (string.IsNullOrEmpty(languageFilePath) && UpdateManager.LanguageCulture.Name != "en")
            {
                string resourceName = $"nUpdate.Localization.{UpdateManager.LanguageCulture.Name}.json";
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    _lp = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the <see cref="SynchronizationContext"/> that should be used to invoke the methods that show the dialogs.
        /// </summary>
        internal SynchronizationContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the instance of the <see cref="Updating.UpdateManager" /> class that is used to perform update-related actions.
        /// </summary>
        internal UpdateManager UpdateManager { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether a hidden search should be provided in order to search in the background without informing the user, or not.
        /// </summary>
        public bool UseHiddenSearch { get; set; }

        /// <summary>
        ///     Shows the built-in UI while the updates are managed.
        /// </summary>
        public void ShowUserInterface()
        {
            if (_isTaskRunning)
                return;

            string languageFilePath = null;
            if (UpdateManager.CultureFilePaths.ContainsKey(UpdateManager.LanguageCulture))
            {
                UpdateManager.CultureFilePaths.TryGetValue(UpdateManager.LanguageCulture, out languageFilePath);
            }

            _isTaskRunning = true;
            _requirementMiss = false;

            var searchDialog = new UpdateSearchDialog { LanguageName = UpdateManager.LanguageCulture.Name, LanguageFilePath = languageFilePath };
            searchDialog.CancelButtonClicked += UpdateSearchDialogCancelButtonClick;

            var newUpdateDialog = new NewUpdateDialog
            {
                LanguageName = UpdateManager.LanguageCulture.Name,
                LanguageFilePath = languageFilePath,
                CurrentVersion = UpdateManager.CurrentVersion,
            };

            var noUpdateDialog = new NoUpdateFoundDialog { LanguageName = UpdateManager.LanguageCulture.Name, LanguageFilePath = languageFilePath };

            var missingRequirementDialog = new MissingUpdateRequirementsDialog { LanguageName = UpdateManager.LanguageCulture.Name };

            // ReSharper disable once UnusedVariable
            var progressIndicator = new Progress<UpdateDownloadProgressChangedEventArgs>();
            var downloadDialog = new UpdateDownloadDialog
            {
                LanguageName = UpdateManager.LanguageCulture.Name,
                LanguageFilePath = languageFilePath,
            };
            downloadDialog.CancelButtonClicked += UpdateDownloadDialogCancelButtonClick;

#if PROVIDE_TAP

            try
            {
                // TAP
                TaskEx.Run(async delegate
                {
                    if (!UseHiddenSearch)
                        _context.Post(searchDialog.ShowModalDialog, null);

                    try
                    {
                        _updatesAvailable = await _updateManager.SearchForUpdatesAsync();
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
                        await TaskEx.Delay(100);
                            // Prevents race conditions that cause that the UpdateSearchDialog can't be closed before further actions are done
                    }

                    if (_updatesAvailable)
                    {
                        newUpdateDialog.PackageSize = _updateManager.TotalSize;
                        newUpdateDialog.PackageConfigurations = _updateManager.PackageConfigurations;
                        var newUpdateDialogReference = new DialogResultReference();
                        _context.Send(newUpdateDialog.ShowModalDialog, newUpdateDialogReference);
                        if (newUpdateDialogReference.DialogResult == DialogResult.Cancel)
                            return;
                    }else if (!_updatesAvailable && _requirementMiss)
                    {
                        string message = "";
                        foreach (var version in _missingRequirements)
                        {
                            message += "The following requirements are missing to install version " + version.Key.ToString() + Environment.NewLine;
                            foreach (var requirement in version.Value)
                            {
                                message += requirement.ErrorMessage + Environment.NewLine;
                            }
                        }
                        missingRequirementDialog.requirementsTextBox.Text = message;
                       var MissingRequirementsDialogReference = new DialogResultReference();
                        Context.Send(missingRequirementDialog.ShowModalDialog, MissingRequirementsDialogReference);
                        if (MissingRequirementsDialogReference.DialogResult == DialogResult.OK)
                            return;
                        return;
                    }
                    else if (!_updatesAvailable && UseHiddenSearch)
                        return;
                    else if (!_updatesAvailable && !UseHiddenSearch)
                    {
                        var noUpdateDialogResultReference = new DialogResultReference();
                        if (!UseHiddenSearch)
                            _context.Send(noUpdateDialog.ShowModalDialog, noUpdateDialogResultReference);
                        return;
                    }

                    downloadDialog.PackagesCount = _updateManager.PackageConfigurations.Count();
                    _context.Post(downloadDialog.ShowModalDialog, null);

                    try
                    {
                        progressIndicator.ProgressChanged += (sender, args) =>
                            downloadDialog.ProgressPercentage = (int) args.Percentage;
                        
                        await _updateManager.DownloadPackagesAsync(progressIndicator);
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
                        isValid = _updateManager.ValidatePackages();
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
                });
            }
            finally
            {
                _isTaskRunning = false;
            }

#else
            try
            {
                //EAP
                UpdateManager.UpdateSearchFinished += SearchFinished;
                UpdateManager.UpdateSearchFinished += searchDialog.Finished;
                UpdateManager.UpdateSearchFailed += searchDialog.Failed;
                UpdateManager.PackagesDownloadProgressChanged += downloadDialog.ProgressChanged;
                UpdateManager.PackagesDownloadFinished += downloadDialog.Finished;
                UpdateManager.PackagesDownloadFailed += downloadDialog.Failed;
                UpdateManager.MissingRequirement += MissingRequirements;

                Task.Factory.StartNew(() =>
                {
                    UpdateManager.SearchForUpdatesAsync();
                    if (!UseHiddenSearch)
                    {
                        var searchDialogResultReference = new DialogResultReference();
                        Context.Send(searchDialog.ShowModalDialog, searchDialogResultReference);
                        Context.Send(searchDialog.CloseDialog, null);
                        if (searchDialogResultReference.DialogResult == DialogResult.Cancel)
                            return;
                    }
                    else
                    {
                        _searchResetEvent.WaitOne();
                    }

                    if (_updatesAvailable)
                    {
                        newUpdateDialog.PackageSize = UpdateManager.TotalSize;
                        newUpdateDialog.PackageConfigurations = UpdateManager.PackageConfigurations;

                        var newUpdateDialogResultReference = new DialogResultReference();
                        Context.Send(newUpdateDialog.ShowModalDialog, newUpdateDialogResultReference);
                        if (newUpdateDialogResultReference.DialogResult == DialogResult.Cancel)
                            return;
                    }
                    else if (!_updatesAvailable && _requirementMiss)
                    {
                        string message = "";
                        foreach (var version in _missingRequirements)
                        {
                            message += "The following requirements are missing to install version " + version.Key.ToString() + Environment.NewLine;
                            foreach (var requirement in version.Value)
                            {
                                message += requirement.ErrorMessage + Environment.NewLine;
                            }
                        }
                        missingRequirementDialog.requirementsTextBox.Text = message;

                        var MissingRequirementsDialogReference = new DialogResultReference();
                        Context.Send(missingRequirementDialog.ShowModalDialog, MissingRequirementsDialogReference);
                        if (MissingRequirementsDialogReference.DialogResult == DialogResult.OK)
                            return;
                        
                        return;
                    }
                    else if (!_updatesAvailable && UseHiddenSearch)
                        return;
                    else if (!_updatesAvailable && !UseHiddenSearch)
                    {
                        Context.Send(noUpdateDialog.ShowModalDialog, null);
                        Context.Send(noUpdateDialog.CloseDialog, null);
                        return;
                    }

                    UpdateManager.DownloadPackagesAsync();

                    var downloadDialogResultReference = new DialogResultReference();
                    Context.Send(downloadDialog.ShowModalDialog, downloadDialogResultReference);
                    Context.Send(downloadDialog.CloseDialog, null);
                    if (downloadDialogResultReference.DialogResult == DialogResult.Cancel)
                        return;

                    bool isValid = false;
                    try
                    {
                        isValid = UpdateManager.ValidatePackages();
                    }
                    catch (FileNotFoundException)
                    {
                        Context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                            _lp.PackageNotFoundErrorText,
                            PopupButtons.Ok), null);
                    }
                    catch (ArgumentException)
                    {
                        Context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                            _lp.InvalidSignatureErrorText, PopupButtons.Ok), null);
                    }
                    catch (Exception ex)
                    {
                        Context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.PackageValidityCheckErrorCaption,
                            ex, PopupButtons.Ok), null);
                    }

                    if (!isValid)
                        Context.Send(o => Popup.ShowPopup(SystemIcons.Error, _lp.InvalidSignatureErrorCaption,
                            _lp.SignatureNotMatchingErrorText,
                            PopupButtons.Ok), null);
                    else
                        UpdateManager.InstallPackage();
                });
            }
            finally
            {
                _isTaskRunning = false;
            }
#endif
        }

        private void SearchFinished(object sender, UpdateSearchFinishedEventArgs e)
        {
            _updatesAvailable = e.UpdatesAvailable;
            _searchResetEvent.Set();
        }

        private void MissingRequirements(object sender, MissingRequirementsEventArgs e)
        {
            _requirementMiss = true;
            _missingRequirements = e.Requirements;
        }

        private void UpdateSearchDialogCancelButtonClick(object sender, EventArgs e)
        {
            UpdateManager.CancelSearch();
        }

        private void UpdateDownloadDialogCancelButtonClick(object sender, EventArgs e)
        {
            UpdateManager.CancelDownload();
        }
    }
}