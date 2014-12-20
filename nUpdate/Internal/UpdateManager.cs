using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using nUpdate.Core;
using nUpdate.Core.Operations;
using nUpdate.Internal.Exceptions;
using nUpdate.Properties;

namespace nUpdate.Internal
{
    /// <summary>
    ///     Offers functions to update .NET-applications.
    /// </summary>
    public class UpdateManager
    {
        private bool _hasDownloadCancelled;
        private bool _hasDownloadFailed;

        private readonly string _applicationUpdateDirectory = Path.Combine(Path.GetTempPath(), "nUpdate",
            Application.ProductName);

        private readonly WebClientWrapper _searchWebClient = new WebClientWrapper();
        private readonly ManualResetEvent _downloadResetEvent = new ManualResetEvent(false);
        private readonly WebClient _packageDownloader = new WebClient();
        private CancellationTokenSource _downloadCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _searchCancellationTokenSource = new CancellationTokenSource();
        private UpdateConfiguration _updateConfiguration = new UpdateConfiguration();

        private string _updateFilePath;
        internal List<OperationArea> OperationAreas = new List<OperationArea>();

        #region "Members"

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateManager" />
        /// </summary>
        /// <param name="updateConfigurationFileUrl">The url of the configuration file.</param>
        /// <param name="publicKey">The public key to check the signature of the update packages.</param>
        /// <param name="currentVersion">The current version of the application.</param>
        /// <param name="languageCulture">The language culture to use for the localization of the integrated UpdaterUI.</param>
        public UpdateManager(Uri updateConfigurationFileUrl, string publicKey, UpdateVersion currentVersion, CultureInfo languageCulture)
        {
            UpdateConfigurationFileUrl = updateConfigurationFileUrl;
            PublicKey = publicKey;
            CurrentVersion = currentVersion;
            LanguageCulture = languageCulture;

            CheckArguments();
            InitializeWorkingArea();
        }

        /// <summary>
        ///     Returns if there were updates found.
        /// </summary>
        public bool UpdatesFound { get; private set; }

        /// <summary>
        ///     Gets or sets the url of the configuration file.
        /// </summary>
        public Uri UpdateConfigurationFileUrl { get; set; }

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
        public CultureInfo LanguageCulture { get; set; }

        /// <summary>
        ///     Gets or sets the paths for the file with the content for the cultures.
        /// </summary>
        public Dictionary<CultureInfo, string> CultureFilePaths { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the user should be able to update to Alpha-versions or not.
        /// </summary>
        public bool IncludeAlpha { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the user should be able to update to Beta-versions or not.
        /// </summary>
        public bool IncludeBeta { get; set; }

        /// <summary>
        ///     Sets if a hidden search should be provided in order to search in the background without informing the user.
        /// </summary>
        public bool UseHiddenSearch { get; set; }

        /// <summary>
        ///     Sets if the found update is a duty update and must be installed.
        /// </summary>
        public bool MustUpdate { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the user can choose between newer versions or must use the very newest one.
        /// </summary>
        public bool VersionIsChoosable { get; set; }

        /// <summary>
        ///     Sets if the current PC using nUpdate for updating should be involved in stats of a statistics server, if available.
        /// </summary>
        public bool IncludeCurrentPcIntoStatistics { get; set; }

        /// <summary>
        ///     Gets the version of the update package.
        /// </summary>
        public UpdateVersion UpdateVersion { get; private set; }

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
        private byte[] Signature { get; set; }

        /// <summary>
        ///     Checks if all arguments have been given.
        /// </summary>
        private void CheckArguments()
        {
            // UpdateInfo-property
            if (UpdateConfigurationFileUrl == null)
                throw new ArgumentException("The Property \"UpdateInfoFileUrl\" is not initialized.");

            if (!UpdateConfigurationFileUrl.ToString().EndsWith(".json"))
                throw new InvalidJsonFileException("The info file is not a valid JSON-file.");

            // PublicKey-property
            if (String.IsNullOrEmpty(PublicKey))
                throw new ArgumentException("The Property \"PublicKey\" is not initialized.");

            // CurrentVersion-property
            if (ReferenceEquals(CurrentVersion, null))
                throw new ArgumentException("The current version must have a value.");

            if (LanguageCulture == null)
                throw new ArgumentException("The Property \"Language\" is not initialized.");

        }

        /// <summary>
        ///     Creates the necessary data for nUpdate.
        /// </summary>
        private void InitializeWorkingArea()
        {
            if (!Directory.Exists(Path.Combine(Path.GetTempPath(), "nUpdate")))
            {
                try
                {
                    Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "nUpdate", Application.ProductName));
                }
                catch (Exception ex)
                {
                    throw new IOException(String.Format("The application's main folder could not be created. {0}",
                        ex.Message));
                }
            }

            CultureFilePaths = new Dictionary<CultureInfo, string> {{new CultureInfo("en"), "en.json"}};
        }

        #endregion

        #region "Events"

        public delegate void FailedEventHandler(Exception exception);
        public delegate void UpdateSearchFinishedEventHandler(bool updateFound);

        /// <summary>
        ///     The event fired when the update search begins.
        /// </summary>
        public event EventHandler<EventArgs> UpdateSearchStarted;

        /// <summary>
        ///     The event fired when the update search is finished.
        /// </summary>
        public event UpdateSearchFinishedEventHandler UpdateSearchFinished;

        /// <summary>
        ///     The event fired when the download of the package begins.
        /// </summary>
        public event FailedEventHandler UpdateSearchFailed;

        /// <summary>
        ///     The event fired when the download of the package begins.
        /// </summary>
        public event EventHandler<EventArgs> PackageDownloadStarted;

        /// <summary>
        ///     The event fired when the download of the package fails.
        /// </summary>
        public event FailedEventHandler PackageDownloadFailed;

        public event DownloadProgressChangedEventHandler PackageDownloadProgressChanged
        {
            add { _packageDownloader.DownloadProgressChanged += value; }
            remove { _packageDownloader.DownloadProgressChanged -= value; }
        }

        public event AsyncCompletedEventHandler PackageDownloadFinished
        {
            add { _packageDownloader.DownloadFileCompleted += value; }
            remove { _packageDownloader.DownloadFileCompleted -= value; }
        }

        protected virtual void OnUpdateSearchStarted(Object sender, EventArgs e)
        {
            if (UpdateSearchStarted != null)
                UpdateSearchStarted(sender, e);
        }

        protected virtual void OnUpdateSearchFinished(bool updateFound)
        {
            if (UpdateSearchFinished != null)
                UpdateSearchFinished(updateFound);
        }

        protected virtual void OnUpdateSearchFailed(Exception exception)
        {
            if (UpdateSearchFailed != null)
                UpdateSearchFailed(exception);
        }

        protected virtual void OnPackageDownloadStarted(Object sender, EventArgs e)
        {
            if (PackageDownloadStarted != null)
                PackageDownloadStarted(sender, e);
        }

        protected virtual void OnPackageDownloadFailed(Exception exception)
        {
            if (PackageDownloadFailed != null)
                PackageDownloadFailed(exception);
        }

        #endregion

        /// <summary>
        ///     Gets the size of the update package.
        /// </summary>
        /// <param name="packageUrl">The url where the update package can be found.</param>
        /// <returns>Returns the size in bytes as a double.</returns>
        private double? GetUpdatePackageSize(Uri packageUrl)
        {
            try
            {
                WebRequest req = WebRequest.Create(packageUrl);
                req.Method = "HEAD";
                using (WebResponse resp = req.GetResponse())
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

        /// <summary>
        ///     Refreshes and re-initializes the cancellation tokens.
        /// </summary>
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
        ///     Checks if updates are available.
        /// </summary>
        /// <exception cref="InvalidOperationException">There is already a search process running.</exception>
        /// <exception cref="NetworkException">There is no network connection available.</exception>
        public void SearchForUpdates()
        {
            if (_searchWebClient.IsBusy)
                throw new InvalidOperationException("There is already a search process running.");

            if (!ConnectionChecker.IsConnectionAvailable())
                throw new NetworkException("No network connection available.");

            //var wc = new WebClientWrapper();
            //if (proxy != null)
            //    wc.Proxy = proxy;

            // Check for SSL and ignore it
            ServicePointManager.ServerCertificateValidationCallback += delegate { return (true); };

            var configurations = UpdateConfiguration.Download(UpdateConfigurationFileUrl, null);
            var result = new UpdateResult(configurations, CurrentVersion,
                IncludeAlpha, IncludeBeta);

            if (!result.UpdatesFound)
            {
                OnUpdateSearchFinished(false);
            }
            else
            {
                _updateConfiguration = result.NewestConfiguration;
                UpdateVersion = new UpdateVersion(_updateConfiguration.LiteralVersion);
                Changelog = _updateConfiguration.Changelog.ContainsKey(LanguageCulture) ? _updateConfiguration.Changelog.First(item => item.Key.Name == LanguageCulture.Name).Value : _updateConfiguration.Changelog.First(item => item.Key.Name == new CultureInfo("en").Name).Value;
                Signature = Convert.FromBase64String(_updateConfiguration.Signature);
                MustUpdate = _updateConfiguration.MustUpdate;

                if (_updateConfiguration.Operations != null)
                {
                    foreach (Operation operation in _updateConfiguration.Operations)
                    {
                        OperationAreas.Add(operation.Area);
                    }
                }

                var updatePackageSize = GetUpdatePackageSize(_updateConfiguration.UpdatePackageUrl);
                if (updatePackageSize == null)
                {
                    throw new SizeCalculationException(
                            "The package size couldn't be calculated because an unknown error appeared. Possibly the package file does not exist.");
                }

                PackageSize = updatePackageSize.Value;
                OnUpdateSearchFinished(true);
            }
        }

        /// <summary>
        ///     Checks if updates are available. This method does not block the calling thread.
        /// </summary>
        /// <exception cref="InvalidOperationException">There is already a search process running.</exception>
        /// <exception cref="NetworkException">There is no network connection available.</exception>
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

        /// <summary>
        ///     The handler set if the async task for the update search throws an exception.
        /// </summary>
        /// <param name="task">The task to handle the sended the exceptions.</param>
        private void SearchExceptionHandler(Task task)
        {
            AggregateException exception = task.Exception;
            if (exception != null && exception.InnerExceptions.Count > 0)
                OnUpdateSearchFailed(exception.InnerException);
        }

        /// <summary>
        ///     Internal method to call when the search task has completed.
        /// </summary>
        private void SearchTaskCompleted()
        {
        }

        /// <summary>
        ///     Downloads the update package.
        /// </summary>
        /// <exception cref="NetworkException">There is no network connection available.</exception>
        /// <exception cref="WebException">The download process has failed because of an <see cref="WebException"/>.</exception>
        /// <exception cref="StatisticsException">The call of the PHP-file for the statistics server entry failed.</exception>
        public void DownloadPackage()
        {
            if (_packageDownloader.IsBusy)
                throw new InvalidOperationException("There is already a download process running.");

            if (!ConnectionChecker.IsConnectionAvailable())
                throw new NetworkException("No network connection available.");

            if (_updateConfiguration.UpdatePackageUrl == null)
                throw new ArgumentException("UpdatePackageUrl");

            if (!Directory.Exists(_applicationUpdateDirectory))
                Directory.CreateDirectory(_applicationUpdateDirectory);

            _updateFilePath = Path.Combine(_applicationUpdateDirectory, "update.zip");
            OnPackageDownloadStarted(this, EventArgs.Empty);
            _packageDownloader.DownloadFileCompleted += DownloadFileCompleted;
            _packageDownloader.DownloadFileAsync(_updateConfiguration.UpdatePackageUrl,
                _updateFilePath);
            _downloadResetEvent.WaitOne();

            if (_hasDownloadCancelled)
            {
                if (MustUpdate)
                    Application.Exit();
                return;
            }

            if (_hasDownloadFailed || !_updateConfiguration.UseStatistics)
                return;

            try
            {
                new WebClient().DownloadString(String.Format("{0}?versionid={1}",
                    _updateConfiguration.UpdatePhpFileUrl, _updateConfiguration.VersionId)); // Only for calling it
            }
            catch (Exception ex)
            {
                throw new StatisticsException(ex.Message);
            }
        }

        private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                _packageDownloader.Dispose();
                DeletePackage();
            }

            if (e.Error != null && e.Error.InnerException != null)
            {
                _hasDownloadFailed = true;
                OnPackageDownloadFailed(e.Error.InnerException);
            }
            _downloadResetEvent.Set();
        }

        /// <summary>
        ///     Downloads the update package. This method does not block the calling thread.
        /// </summary>
        /// <exception cref="NetworkException">There is no network connection available..</exception>
        /// <exception cref="WebException">The download process has failed because of an WebException.</exception>
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

        /// <summary>
        ///     The handler set if the async task for the update search throws an exception.
        /// </summary>
        /// <param name="task">The task to handle the sended exceptions from.</param>
        private void DownloadExceptionHandler(Task task)
        {
            AggregateException exception = task.Exception;
            if (exception != null && exception.InnerExceptions.Count > 0)
                OnPackageDownloadFailed(exception.InnerException);
        }

        /// <summary>
        ///     Internal method to call when the search task has completed.
        /// </summary>
        private void DownloadTaskCompleted()
        {
        }

        /// <summary>
        ///     Cancels the package download.
        /// </summary>
        public void CancelDownload()
        {
            _hasDownloadCancelled = true;
            _packageDownloader.CancelAsync();
        }

        /// <summary>
        ///     Gets a value indicating whether a download is currently active or not.
        /// </summary>
        public bool IsDownloading
        {
            get { return _packageDownloader.IsBusy; }
        }

        /// <summary>
        ///     Checks whether the package is valid or not. If it contains an invalid signature, it will be deleted directly.
        /// </summary>
        /// <returns>Returns 'true' when the package is valid, otherwise 'false'.</returns>
        /// <exception cref="System.IO.FileNotFoundException">The update package to check could not be found.</exception>
        /// <exception cref="System.ArgumentException">The signature of the update package is null or invalid.</exception>
        public bool CheckPackageValidity()
        {
            if (!File.Exists(_updateFilePath))
                throw new FileNotFoundException("The update package to check could not be found.");

            if (Signature == null || Signature.Length <= 0)
                throw new ArgumentException("Signature");

            byte[] data;
            using (var reader =
                new BinaryReader(File.Open(_updateFilePath,
                    FileMode.Open)))
            {
                data = reader.ReadBytes((int)reader.BaseStream.Length);
                reader.Close();
            }

            RsaSignature rsa;

            try
            {
                rsa = new RsaSignature(PublicKey);
            }
            catch
            {
                DeletePackage();
                return false;
            }

            if (rsa.VerifyData(data, Signature))
                return true;
            DeletePackage();
            return false;
        }

        /// <summary>
        ///     Starts the nUpdate UpdateInstaller to unpack the package and start the updating process.
        /// </summary>
        public void InstallPackage()
        {
            string unpackerDirectory = Path.Combine(Path.GetTempPath(), "nUpdate Installer");
            string unpackerZipPath = Path.Combine(unpackerDirectory, "Ionic.Zip.dll");
            string unpackerAppPath = Path.Combine(unpackerDirectory, "nUpdate UpdateInstaller.exe");

            if (!Directory.Exists(unpackerDirectory))
                Directory.CreateDirectory(unpackerDirectory);

            if (!File.Exists(unpackerZipPath))
                File.WriteAllBytes(unpackerZipPath, Resources.Ionic_Zip);

            if (!File.Exists(unpackerAppPath))
                File.WriteAllBytes(unpackerAppPath, Resources.nUpdate_UpdateInstaller);

            string[] args =
            {
                _updateFilePath, Application.StartupPath, Application.ExecutablePath,
                Application.ProductName, "Unpacking", "The installation of the update failed.", "{0}... \nPossibly the program won't be able to run properly as important components could be missing then.", "OK"
            };
            // TODO: lp.UnpackingText and lp.UnpackingErrorTitle + lp.UnpackingErrorMessage

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

            Application.Exit();
        }

        /// <summary>
        ///     Deletes the package.
        /// </summary>
        public void DeletePackage()
        {
            if (File.Exists(_updateFilePath))
                File.Delete(_updateFilePath);
        }
    }
}