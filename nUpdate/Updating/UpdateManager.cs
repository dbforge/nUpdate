// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using nUpdate.Core;
using nUpdate.Core.Localization;
using nUpdate.Core.Operations;
using nUpdate.Exceptions;
using nUpdate.Properties;
using nUpdate.UpdateEventArgs;
using SystemInformation = nUpdate.Core.SystemInformation;

namespace nUpdate.Updating
{
    /// <summary>
    ///     Offers functions to update .NET-applications.
    /// </summary>
    public class UpdateManager : IDisposable
    {
        private bool _includeCurrentPcIntoStatistics = true;
        private bool _disposed;
        private CancellationTokenSource _searchCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _downloadCancellationTokenSource = new CancellationTokenSource();
        private bool _hasDownloadCancelled;
        private bool _hasDownloadFailed;
        private UpdateConfiguration _updateConfiguration = new UpdateConfiguration();
        private string _updateFilePath;
        private readonly string _applicationUpdateDirectory = Path.Combine(Path.GetTempPath(), "nUpdate",
            Application.ProductName);

        private readonly ManualResetEvent _downloadResetEvent = new ManualResetEvent(false);
        private readonly WebClientWrapper _packageDownloader = new WebClientWrapper();
        private readonly WebClientWrapper _searchWebClient = new WebClientWrapper();
        private LocalizationProperties _lp;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateManager"/> class.
        /// </summary>
        /// <param name="updateConfigurationFileUri">The URI of the update configuration file.</param>
        /// <param name="publicKey">The public key for the validity check of the update packages.</param>
        /// <param name="currentVersion">The current version of the application.</param>
        /// <param name="languageCulture">The language culture to use for the localization of the integrated UpdaterUI.</param>
        /// <remarks>The public key can be found in the overview of your project when you're opening it in nUpdate Administration. If you have problems inserting the data (or if you want to save time) you can scroll down there and follow the steps of the category "Copy data" which will automatically generate the necessray code for you.</remarks>
        public UpdateManager(Uri updateConfigurationFileUri, string publicKey, UpdateVersion currentVersion,
            CultureInfo languageCulture)
        {
            UpdateConfigurationFileUri = updateConfigurationFileUri;
            PublicKey = publicKey;
            CurrentVersion = currentVersion;
            LanguageCulture = languageCulture;
            if (CultureFilePaths == null)
                CultureFilePaths = new Dictionary<CultureInfo, string>();

            CheckArguments();
            Initialize();
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="UpdateManager"/> class.
        /// </summary>
        ~UpdateManager()
        {
            Dispose(true);
        }

        /// <summary>
        ///     Gets a value indicating whether a download is currently active or not.
        /// </summary>
        public bool IsDownloading
        {
            get { return _packageDownloader.IsBusy; }
        }

        /// <summary>
        ///     Gets or sets the URI of the configuration file.
        /// </summary>
        public Uri UpdateConfigurationFileUri { get; set; }

        /// <summary>
        ///     Gets or sets the public key for checking the signature.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        ///     Gets or sets the version of the current application.
        /// </summary>
        public UpdateVersion CurrentVersion { get; set; }

        /// <summary>
        ///     Gets or sets the culture of the language to use.
        /// </summary>
        /// <remarks>
        ///     "en" (English) and "de" (German) are currently the only language cultures that are already implemented in
        ///     nUpdate. In order to use own languages download the language template from
        ///     <see href="http://www.nupdate.net/langtemplate.json"/>, edit it, save it as a JSON-file and add a new entry to property
        ///     CultureFilePaths with the relating CultureInfo and path which locates the JSON-file on the client's
        ///     system (e. g. AppData).
        /// </remarks>
        public CultureInfo LanguageCulture { get; set; }

        /// <summary>
        ///     Gets or sets the paths for the file with the content relating to the cultures.
        /// </summary>
        public Dictionary<CultureInfo, string> CultureFilePaths { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether a custom user interface should be used for the update installer, or not.
        /// </summary>
        public bool UseCustomInstallerUserInterface { get; set; }

        /// <summary>
        ///     Gets or sets the path of the custom assembly'S location that should be used for the update installer's user interface.
        /// </summary>
        public string CustomInstallerUiAssemblyPath { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the user should be able to update to Alpha-versions or not.
        /// </summary>
        public bool IncludeAlpha { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the user should be able to update to Beta-versions or not.
        /// </summary>
        public bool IncludeBeta { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether a hidden search should be provided in order to search in the background without informing the user, or not.
        /// </summary>
        public bool UseHiddenSearch { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the found update must be installed, or not.
        /// </summary>
        public bool MustUpdate { get; private set; }

        /// <summary>
        ///     Gets or sets if the current PC should be involved in entries made on the statistics server, if one is available.
        /// </summary>
        public bool IncludeCurrentPcIntoStatistics
        {
            get { return _includeCurrentPcIntoStatistics; }
            set { _includeCurrentPcIntoStatistics = value; }
        }

        /// <summary>
        ///     Gets the version of the newest update package.
        /// </summary>
        public UpdateVersion NewestVersion { get; private set; }

        /// <summary>
        ///     Gets the changelog of the update package.
        /// </summary>
        public string Changelog { get; private set; }

        /// <summary>
        ///     Gets the size of the update package.
        /// </summary>
        public double PackageSize { get; private set; }

        /// <summary>
        ///     Gets the signature of the update package.
        /// </summary>
        public byte[] Signature { get; private set; }

        /// <summary>
        ///     Gets the operations of the update package.
        /// </summary>
        public IEnumerable<Operation> Operations { get; private set; }

        /// <summary>
        ///     Gets or sets the proxy to use, if wished.
        /// </summary>
        public WebProxy Proxy { get; set; }

        /// <summary>
        ///     Releases all managed and unmanaged resources used by the current <see cref="UpdateManager"/>-instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;

            _searchWebClient.Dispose();
            _packageDownloader.Dispose();
            _searchCancellationTokenSource.Dispose();
            _downloadCancellationTokenSource.Dispose();
            _downloadResetEvent.Dispose();
            _disposed = true;
        }

        private double? GetUpdatePackageSize(Uri packageUrl)
        {
            try
            {
                var req = WebRequest.Create(packageUrl);
                req.Method = "HEAD";
                using (var resp = req.GetResponse())
                {
                    double contentLength;
                    if (double.TryParse(resp.Headers.Get("Content-Length"), out contentLength))
                        return contentLength;
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        private void RefreshCancellationTokens()
        {
            if (_searchCancellationTokenSource != null)
                _searchCancellationTokenSource.Dispose();
            _searchCancellationTokenSource = new CancellationTokenSource();

            if (_downloadCancellationTokenSource != null)
                _downloadCancellationTokenSource.Dispose();
            _downloadCancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        ///     Searches for updates.
        /// </summary>
        /// <exception cref="InvalidOperationException">There is already a search process running.</exception>
        /// <exception cref="NetworkException">There is no network connection available.</exception>
        /// <seealso cref="SearchForUpdatesAsync"/>
        public void SearchForUpdates()
        {
            if (_searchWebClient.IsBusy)
                throw new InvalidOperationException(_lp.SearchProcessRunningExceptionText);

            if (!ConnectionChecker.IsConnectionAvailable())
                throw new NetworkException(_lp.NetworkConnectionExceptionText);

            // Check for SSL and ignore it
            ServicePointManager.ServerCertificateValidationCallback += delegate { return (true); };

            var configurations = UpdateConfiguration.Download(UpdateConfigurationFileUri, Proxy);
            var result = new UpdateResult(configurations, CurrentVersion,
                IncludeAlpha, IncludeBeta);

            if (!result.UpdatesFound)
            {
                OnUpdateSearchFinished(false);
            }
            else
            {
                _updateConfiguration = result.NewestConfiguration;
                NewestVersion = new UpdateVersion(_updateConfiguration.LiteralVersion);
                Changelog = _updateConfiguration.Changelog.ContainsKey(LanguageCulture)
                    ? _updateConfiguration.Changelog.First(item => item.Key.Name == LanguageCulture.Name).Value
                    : _updateConfiguration.Changelog.First(item => item.Key.Name == new CultureInfo("en").Name).Value;
                Signature = Convert.FromBase64String(_updateConfiguration.Signature);
                MustUpdate = _updateConfiguration.MustUpdate;
                Operations = _updateConfiguration.Operations;

                var updatePackageSize = GetUpdatePackageSize(_updateConfiguration.UpdatePackageUri);
                if (updatePackageSize == null)
                    throw new SizeCalculationException(_lp.PackageSizeCalculationExceptionText);

                PackageSize = updatePackageSize.Value;
                OnUpdateSearchFinished(true);
            }
        }

        /// <summary>
        ///     Searches for updates. This method does not block the calling thread.
        /// </summary>
        /// <exception cref="InvalidOperationException">There is already a search process running.</exception>
        /// <exception cref="NetworkException">There is no network connection available.</exception>
        /// <seealso cref="SearchForUpdates"/>
        /// <seealso cref="CancelSearchAsync"/>
        public void SearchForUpdatesAsync()
        {
            RefreshCancellationTokens();

            Task.Factory.StartNew(SearchForUpdates).ContinueWith(SearchExceptionHandler,
                _searchCancellationTokenSource.Token,
                TaskContinuationOptions.OnlyOnFaulted,
                TaskScheduler.FromCurrentSynchronizationContext()).ContinueWith(o => SearchTaskCompleted(),
                    _searchCancellationTokenSource.Token,
                    TaskContinuationOptions.NotOnFaulted,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void SearchExceptionHandler(Task task)
        {
            if (_searchCancellationTokenSource.IsCancellationRequested)
                return;

            var exception = task.Exception;
            if (exception != null && exception.InnerExceptions.Count > 0)
                OnUpdateSearchFailed(exception.InnerException);
        }

        private void SearchTaskCompleted()
        {
        }

        /// <summary>
        ///     Cancels the current asynchronous search task.
        /// </summary>
        /// <remarks>If there is no search task running, nothing will happen.</remarks>
        public void CancelSearchAsync()
        {
            _searchCancellationTokenSource.Cancel();
        }

        /// <summary>
        ///     Downloads the update package from the server.
        /// </summary>
        /// <exception cref="NetworkException">There is no network connection available.</exception>
        /// <exception cref="WebException">The download process has failed because of an <see cref="WebException"/>.</exception>
        /// <exception cref="ArgumentException">The URI of the update package is null.</exception>
        /// <seealso cref="DownloadPackageAsync"/>
        public void DownloadPackage()
        {
            if (_packageDownloader.IsBusy)
                throw new InvalidOperationException(_lp.DownloadingProcessRunningExceptionText);

            if (!ConnectionChecker.IsConnectionAvailable())
                throw new NetworkException(_lp.NetworkConnectionExceptionText);

            if (_updateConfiguration.UpdatePackageUri == null)
                throw new ArgumentException("UpdatePackageUri");

            if (!Directory.Exists(_applicationUpdateDirectory))
                Directory.CreateDirectory(_applicationUpdateDirectory);

            _updateFilePath = Path.Combine(_applicationUpdateDirectory, "update.zip");
            OnPackageDownloadStarted(this, EventArgs.Empty);
            _packageDownloader.Proxy = Proxy;
            _packageDownloader.DownloadFileCompleted += DownloadFileCompleted;
            _packageDownloader.DownloadFileAsync(_updateConfiguration.UpdatePackageUri,
                _updateFilePath);
            _downloadResetEvent.WaitOne();

            if (_hasDownloadCancelled)
            {
                if (MustUpdate)
                    Application.Exit();
                return;
            }

            if (_hasDownloadFailed)
                return;

            if (_updateConfiguration.UseStatistics && _includeCurrentPcIntoStatistics)
            {
                try
                {
                    string response = new WebClient().DownloadString(String.Format("{0}?versionid={1}&os={2}",
                        _updateConfiguration.UpdatePhpFileUri, _updateConfiguration.VersionId,
                        SystemInformation.GetOperatingSystemName())); // Only for calling it
                    if (!String.IsNullOrEmpty(response))
                    {
                        OnStatisticsEntryFailed(new Exception(
                            String.Format(
                                _lp.StatisticsScriptExceptionText, response)));
                    }
                }
                catch (Exception ex)
                {
                    OnStatisticsEntryFailed(ex);
                }
            }

            OnPackageDownloadFinished(this, EventArgs.Empty);
        }

        private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null && !e.Cancelled)
                {
                    _hasDownloadFailed = true;
                    DeletePackage();
                    OnPackageDownloadFailed(e.Error.InnerException ?? e.Error);
                }

                if (e.Cancelled)
                    DeletePackage();
                
            }
            finally
            {
                _downloadResetEvent.Set();
            }
        }

        /// <summary>
        ///     Downloads the update package from the server. This method does not block the calling thread.
        /// </summary>
        /// <exception cref="NetworkException">There is no network connection available.</exception>
        /// <exception cref="WebException">The download process has failed because of a <see cref="WebException"/>.</exception>
        /// <seealso cref="DownloadPackage"/>
        /// <seealso cref="CancelDownloadAsync"/>
        public void DownloadPackageAsync()
        {
            RefreshCancellationTokens();

            Task.Factory.StartNew(DownloadPackage).ContinueWith(DownloadExceptionHandler,
                _downloadCancellationTokenSource.Token,
                TaskContinuationOptions.OnlyOnFaulted,
                TaskScheduler.FromCurrentSynchronizationContext()).ContinueWith(o => DownloadTaskCompleted(),
                    _downloadCancellationTokenSource.Token,
                    TaskContinuationOptions.NotOnFaulted,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void DownloadExceptionHandler(Task task)
        {
            var exception = task.Exception;
            if (exception != null && exception.InnerExceptions.Count > 0)
                OnPackageDownloadFailed(exception.InnerException);
        }

        private void DownloadTaskCompleted()
        {
        }

        /// <summary>
        ///     Cancels the current asynchronous download task.
        /// </summary>
        /// <remarks>If there is no download task running, nothing will happen.</remarks>
        public void CancelDownloadAsync()
        {
            if (!IsDownloading)
                return;

            _hasDownloadCancelled = true;
            _packageDownloader.CancelAsync();
        }

        /// <summary>
        ///     Returns a value indicating whether the package is valid or not. If it contains an invalid signature, it will be deleted directly.
        /// </summary>
        /// <returns>Returns 'true' if the package is valid, otherwise 'false'.</returns>
        /// <exception cref="FileNotFoundException">The update package to check could not be found.</exception>
        /// <exception cref="ArgumentException">The signature of the update package is null or empty.</exception>
        public bool CheckPackageValidity()
        {
            if (!File.Exists(_updateFilePath))
                throw new FileNotFoundException(_lp.PackageFileNotFoundExceptionText);

            if (Signature == null || Signature.Length <= 0)
                throw new ArgumentException("Signature");

            byte[] data;
            using (var reader =
                new BinaryReader(File.Open(_updateFilePath,
                    FileMode.Open)))
            {
                data = reader.ReadBytes((int)reader.BaseStream.Length);
            }

            RsaManager rsa;

            try
            {
                rsa = new RsaManager(PublicKey);
            }
            catch
            {
                try
                {
                    DeletePackage();
                }
                catch (Exception ex)
                {
                    throw new PackageDeleteException(ex.Message);
                }
                return false;
            }

            if (rsa.VerifyData(data, Signature))
                return true;

            try
            {
                DeletePackage();
            }
            catch (Exception ex)
            {
                throw new PackageDeleteException(ex.Message);
            }
            return false;
        }

        /// <summary>
        ///     Terminates the application.
        /// </summary>
        /// <remarks>If your apllication doesn't terminate correctly or if you want to perform custom actions before termiating, then override this method and implement your own code.</remarks>
        public virtual void TerminateApplication()
        {
            Application.Exit();
        }

        /// <summary>
        ///     Starts the nUpdate UpdateInstaller to unpack the package and start the updating process.
        /// </summary>
        public void InstallPackage()
        {
            var unpackerDirectory = Path.Combine(Path.GetTempPath(), "nUpdate Installer");
            var unpackerZipPath = Path.Combine(unpackerDirectory, "Ionic.Zip.dll");
            var guiInterfacePath = Path.Combine(unpackerDirectory, "nUpdate.UpdateInstaller.Client.GuiInterface.dll");
            var jsonNetPath = Path.Combine(unpackerDirectory, "Newtonsoft.Json.dll");
            var jsonNetPdbPath = Path.Combine(unpackerDirectory, "Newtonsoft.Json.pdb");
            var unpackerAppPath = Path.Combine(unpackerDirectory, "nUpdate UpdateInstaller.exe");
            //var unpackerAppPdbPath = Path.Combine(unpackerDirectory, "nUpdate UpdateInstaller.pdb"); 

            if (Directory.Exists(unpackerDirectory))
                Directory.Delete(unpackerDirectory, true);
            Directory.CreateDirectory(unpackerDirectory);

            if (!File.Exists(unpackerZipPath))
                File.WriteAllBytes(unpackerZipPath, Resources.Ionic_Zip);

            if (!File.Exists(guiInterfacePath))
                File.WriteAllBytes(guiInterfacePath, Resources.nUpdate_UpdateInstaller_Client_GuiInterface);

            if (!File.Exists(jsonNetPath))
                File.WriteAllBytes(jsonNetPath, Resources.Newtonsoft_Json);

            if (!File.Exists(jsonNetPdbPath))
                File.WriteAllBytes(jsonNetPdbPath, Resources.Newtonsoft_Json_Pdb);

            if (!File.Exists(unpackerAppPath))
                File.WriteAllBytes(unpackerAppPath, Resources.nUpdate_UpdateInstaller);

            //if (!File.Exists(unpackerAppPdbPath))
            //    File.WriteAllBytes(unpackerAppPath, Resources.nUpdate_UpdateInstaller_Pdb);

            var installerUiAssemblyPath = UseCustomInstallerUserInterface
                ? String.Format("\"{0}\"", CustomInstallerUiAssemblyPath)
                : String.Empty;
            string[] args =
            {
                String.Format("\"{0}\"", _updateFilePath), String.Format("\"{0}\"", Application.StartupPath),
                String.Format("\"{0}\"", Application.ExecutablePath),
                String.Format("\"{0}\"", Application.ProductName),
                String.Format("\"{0}\"",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(Serializer.Serialize(Operations)))), 
                    String.Format("\"{0}\"", installerUiAssemblyPath),
                    _lp.InstallerExtractingFilesText,
                    _lp.InstallerCopyingText,
                    _lp.FileDeletingOperationText,
                    _lp.FileRenamingOperationText,
                    _lp.RegistrySubKeyCreateOperationText,
                    _lp.RegistrySubKeyDeleteOperationText,
                    _lp.RegistryNameValuePairDeleteValueOperationText,
                    _lp.RegistryNameValuePairSetValueOperationText,
                    _lp.ProcessStartOperationText,
                    _lp.ProcessStopOperationText,
                    _lp.ServiceStartOperationText,
                    _lp.ServiceStopOperationText,
                    _lp.InstallerUpdatingErrorCaption, 
                    _lp.InstallerInitializingErrorCaption
            };

            var startInfo = new ProcessStartInfo
            {
                FileName = unpackerAppPath,
                Arguments = String.Join("|", args),
                Verb = "runas"
            };

            try
            {
                Process.Start(startInfo);
            }
            catch (Win32Exception)
            {
                DeletePackage();
                if (!MustUpdate)
                    return;
            }

            TerminateApplication();
        }

        /// <summary>
        ///     Deletes the update package locally.
        /// </summary>
        public void DeletePackage()
        {
            if (File.Exists(_updateFilePath))
                File.Delete(_updateFilePath);
        }

        private void CheckArguments()
        {
            if (UpdateConfigurationFileUri == null)
                throw new ArgumentException("The property \"UpdateInfoFileUrl\" is not initialized.");

            if (!UpdateConfigurationFileUri.ToString().EndsWith(".json"))
                throw new InvalidJsonFileException("The update configuration file is not a valid JSON-file.");

            if (String.IsNullOrEmpty(PublicKey))
                throw new ArgumentException("The property \"PublicKey\" is not initialized.");

            if (ReferenceEquals(CurrentVersion, null))
                throw new ArgumentException("The current version must have a value.");

            if (LanguageCulture == null)
                throw new ArgumentException("The property \"LanguageCulture\" is not initialized.");

            var existingCultureInfos = new[] { new CultureInfo("en"), new CultureInfo("de-DE"), new CultureInfo("de-AT"), new CultureInfo("de-CH") };
            if (!existingCultureInfos.Any(item => item.Equals(LanguageCulture)) &&
                !CultureFilePaths.ContainsKey(LanguageCulture))
                throw new ArgumentException(
                    "The given culture info does neither exist in nUpdate's resources, nor in property \"CultureFilePaths\".");

            if (UseCustomInstallerUserInterface && String.IsNullOrEmpty(CustomInstallerUiAssemblyPath))
                throw new ArgumentException(
                    "The property \"CustomInstallerUiAssemblyPath\" is not initialized although \"UseCustomInstallerUserInterface\" is set to \"true\"");
        }

        private void Initialize()
        {
            if (!Directory.Exists(Path.Combine(Path.GetTempPath(), "nUpdate")))
            {
                try
                {
                    Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "nUpdate", Application.ProductName));
                }
                catch (Exception ex)
                {
                    throw new IOException(String.Format(_lp.MainFolderCreationExceptionText,
                        ex.Message));
                }
            }

            var languageFilePath = CultureFilePaths.Any(item => item.Key.Equals(LanguageCulture)) ? CultureFilePaths.First(item => item.Key.Equals(LanguageCulture)).Value : null;

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
            else if (String.IsNullOrEmpty(languageFilePath) && LanguageCulture.Name != "en")
            {
                string resourceName = String.Format("nUpdate.Core.Localization.{0}.json", LanguageCulture.Name);
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    _lp = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }
            else if (String.IsNullOrEmpty(languageFilePath) && LanguageCulture.Name == "en")
            {
                _lp = new LocalizationProperties();
            }
        }

        /// <summary>
        ///     Occurs when update search is started.
        /// </summary>
        public event EventHandler<EventArgs> UpdateSearchStarted;

        /// <summary>
        ///     Occurs when the update search is finished.
        /// </summary>
        public event EventHandler<UpdateSearchFinishedEventArgs> UpdateSearchFinished;

        /// <summary>
        ///     Occurs when the download of the package begins.
        /// </summary>
        public event EventHandler<FailedEventArgs> UpdateSearchFailed;

        /// <summary>
        ///     Occurs when the download of the package begins.
        /// </summary>
        public event EventHandler<EventArgs> PackageDownloadStarted;

        /// <summary>
        ///     Occurs when the download of the package fails.
        /// </summary>
        public event EventHandler<FailedEventArgs> PackageDownloadFailed;

        /// <summary>
        ///     Occurs when the package download progress has changed.
        /// </summary>
        public event DownloadProgressChangedEventHandler PackageDownloadProgressChanged
        {
            add { _packageDownloader.DownloadProgressChanged += value; }
            remove { _packageDownloader.DownloadProgressChanged -= value; }
        }

        /// <summary>
        ///     Occurs when the package download is finished.
        /// </summary>
        public event EventHandler<EventArgs> PackageDownloadFinished;

        /// <summary>
        ///     Occurs when the statistics entry failed.
        /// </summary>
        /// <remarks>This event is meant to provide the user with a warning if the statistic server entry fails. The update process shouldn't be cancelled as this doesn't cause any conflicts that could affect it.</remarks>
        public event EventHandler<FailedEventArgs> StatisticsEntryFailed; 

        /// <summary>
        /// Called when the update search is started.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnUpdateSearchStarted(Object sender, EventArgs e)
        {
            if (UpdateSearchStarted != null)
                UpdateSearchStarted(sender, e);
        }

        /// <summary>
        ///     Called when the update search is finished.
        /// </summary>
        /// <param name="updateAvailable">if set to <c>true</c> updates are available.</param>
        protected virtual void OnUpdateSearchFinished(bool updateAvailable)
        {
            if (UpdateSearchFinished != null)
                UpdateSearchFinished(this, new UpdateSearchFinishedEventArgs(updateAvailable));
        }

        /// <summary>
        ///     Called when the update search failed.
        /// </summary>
        /// <param name="exception">The exception that occured.</param>
        protected virtual void OnUpdateSearchFailed(Exception exception)
        {
            if (UpdateSearchFailed != null)
                UpdateSearchFailed(this, new FailedEventArgs(exception));
        }

        /// <summary>
        ///     Called when the package download is started.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnPackageDownloadStarted(Object sender, EventArgs e)
        {
            if (PackageDownloadStarted != null)
                PackageDownloadStarted(sender, e);
        }

        /// <summary>
        ///     Called when the package download is finished.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnPackageDownloadFinished(Object sender, EventArgs e)
        {
            if (PackageDownloadFinished != null)
                PackageDownloadFinished(sender, e);
        }

        /// <summary>
        ///     Called when the package download failed.
        /// </summary>
        /// <param name="exception">The exception that occured.</param>
        protected virtual void OnPackageDownloadFailed(Exception exception)
        {
            if (PackageDownloadFailed != null)
                PackageDownloadFailed(this, new FailedEventArgs(exception));
        }

        /// <summary>
        ///     Called when the statistics entry failed.
        /// </summary>
        /// <param name="exception">The exception that occured.</param>
        protected virtual void OnStatisticsEntryFailed(Exception exception)
        {
            if (StatisticsEntryFailed != null)
                StatisticsEntryFailed(this, new FailedEventArgs(exception));
        }
    }
}