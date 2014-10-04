using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using nUpdate.Core;
using nUpdate.Internal.Exceptions;
using nUpdate.Properties;

namespace nUpdate.Internal
{
    /// <summary>
    ///     Class that offers functions to update .NET-applications.
    /// </summary>
    public class UpdateManager
    {
        private readonly string _applicationUpdateDirectory = Path.Combine(Path.GetTempPath(), "nUpdate",
            Application.ProductName);

        private readonly WebClient _packageDownloader = new WebClient();
        private CancellationTokenSource _downloadCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _searchCancellationTokenSource = new CancellationTokenSource();
        private UpdateConfiguration _updateConfiguration = new UpdateConfiguration();

        private string _updateFilePath;

        #region "Members"

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateManager" /> class.
        /// </summary>
        public UpdateManager()
            : this(null, String.Empty, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateManager" /> class.
        /// </summary>
        /// <param name="updateInfoFileUrl">The url of the info file.</param>
        public UpdateManager(Uri updateConfigurationFileUrl)
            : this(updateConfigurationFileUrl, String.Empty, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateManager" />
        /// </summary>
        /// <param name="updateConfigurationFileUrl">The url of the info file.</param>
        /// <param name="publicKey">The public key to check the signature of an update package.</param>
        /// <param name="currentVersion">The current version of the current application.</param>
        public UpdateManager(Uri updateConfigurationFileUrl, string publicKey, UpdateVersion currentVersion)
        {
            UpdateConfigurationFileUrl = updateConfigurationFileUrl;
            PublicKey = publicKey;
            CurrentVersion = currentVersion;

            // Create all necessary data
            InitializeWorkaroundArea();

            //LanguageSerializer lang = LanguageSerializer.ReadXml(Properties.Resources.en);
        }

        /// <summary>
        ///     Returns if there were updates found.
        /// </summary>
        public bool UpdatesFound { get; private set; }

        /// <summary>
        ///     Sets the version id to use to get the package if a statistics server is used.
        /// </summary>
        private int VersionId { get; set; }

        /// <summary>
        ///     Sets the url of the configuration file.
        /// </summary>
        public Uri UpdateConfigurationFileUrl { get; set; }

        /// <summary>
        ///     Sets the PublicKey for the signature.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        ///     Sets the version of the current application.
        /// </summary>
        public UpdateVersion CurrentVersion { get; set; }

        /// <summary>
        ///     The culture of the language to use.
        /// </summary>
        public CultureInfo LanguageCulture { get; set; }

        /// <summary>
        ///     Sets the paths for the file with the content for the cultures of an element.
        /// </summary>
        public Dictionary<CultureInfo, string> CultureFilePaths { get; set; }

        /// <summary>
        ///     Sets if the user should be able to update to Alpha-versions.
        /// </summary>
        public bool IncludeAlpha { get; set; }

        /// <summary>
        ///     Sets if the user should be able to update to Beta-versions.
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
        ///     Gets the versions that the update package does not support.
        /// </summary>
        private Version[] UnsupportedVersions { get; set; }

        /// <summary>
        ///     Checks if all arguments have been given.
        /// </summary>
        private void CheckArguments()
        {
            // UpdateInfo-property
            if (UpdateConfigurationFileUrl == null)
                throw new ArgumentException("The Property \"UpdateInfoFileUrl\" is not initialized.");

            if (!UpdateConfigurationFileUrl.ToString().EndsWith(".json"))
                throw new FormatException("The info file is not a valid JSON-file.");

            // PublicKey-property
            if (String.IsNullOrEmpty(PublicKey))
                throw new ArgumentException("The Property \"PublicKey\" is not initialized.");

            // CurrentVersion-property
            if (CurrentVersion == null)
                throw new ArgumentException("The current version must have a value.");

            if (LanguageCulture == null)
                throw new ArgumentException("The Property \"Language\" is not initialized.");
        }

        /// <summary>
        ///     Creates the necessary data for nUpdate.
        /// </summary>
        private void InitializeWorkaroundArea()
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

            CultureFilePaths = new Dictionary<CultureInfo, string>();
            CultureFilePaths.Add(new CultureInfo("en"), "en.json");
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
        private double GetUpdatePackageSize(Uri packageUrl)
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
                return -1;
            }

            return -1;
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
        ///     Checks if there are updates available.
        /// </summary>
        /// <exception cref="InvalidOperationException">There is already a search process running.</exception>
        /// <exception cref="NetworkException">There is no network connection available..</exception>
        public void SearchForUpdates()
        {
            //RefreshCancellationTokens();

            if (!ConnectionChecker.IsConnectionAvailable())
                throw new NetworkException("No network connection available.");

            var wc = new WebClientWrapper();
            //if (proxy != null)
            //    wc.Proxy = proxy;

            // Check for SSL and ignore it
            ServicePointManager.ServerCertificateValidationCallback += delegate { return (true); };
            var list = Serializer.Deserialize<List<UpdateConfiguration>>(wc.DownloadString(UpdateConfigurationFileUrl));

            var result = new UpdateResult(list, CurrentVersion,
                IncludeAlpha, IncludeBeta);

            if (!result.UpdatesFound)
            {
                OnUpdateSearchFinished(false);
            }
            else
            {
                _updateConfiguration = result.NewestConfiguration;
                UpdateVersion = new UpdateVersion(_updateConfiguration.Version);
                Changelog = _updateConfiguration.Changelog; 
                Signature = Convert.FromBase64String(_updateConfiguration.Signature);
                MustUpdate = _updateConfiguration.MustUpdate;

                //if (updateConfig.Operations != null)
                //{
                // TODO: Operations
                //}

                PackageSize = GetUpdatePackageSize(_updateConfiguration.UpdatePackageUrl);
                OnUpdateSearchFinished(true);
            }
        }

        /// <summary>
        ///     Checks if there are updates available. This method does not block the calling thread.
        /// </summary>
        /// <exception cref="InvalidOperationException">There is already a search process running.</exception>
        /// <exception cref="NetworkException">There is no network connection available..</exception>
        public void SearchForUpdatesAsync()
        {
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
        /// <exception cref="NetworkException">There is no network connection available..</exception>
        /// <exception cref="WebException">The download process has failed because of an WebException.</exception>
        /// <exception cref="StatisticsException">The call of the PHP-file for the statistics server entry failed.</exception>
        private void DownloadPackage()
        {
            RefreshCancellationTokens();

            if (!ConnectionChecker.IsConnectionAvailable())
                throw new NetworkException("No network connection available.");

            if (_updateConfiguration.UpdatePackageUrl == null)
                throw new ArgumentException("UpdatePackageUrl");

            if (!Directory.Exists(_applicationUpdateDirectory))
                Directory.CreateDirectory(_applicationUpdateDirectory);

            _updateFilePath = Path.Combine(_applicationUpdateDirectory, "update.zip");
            OnPackageDownloadStarted(this, EventArgs.Empty);
            _packageDownloader.DownloadFileAsync(_updateConfiguration.UpdatePackageUrl,
                _updateFilePath);

            if (_updateConfiguration.UseStatistics)
            {
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
        }

        /// <summary>
        ///     Downloads the update package. This method does not block the calling thread.
        /// </summary>
        /// <exception cref="NetworkException">There is no network connection available..</exception>
        /// <exception cref="WebException">The download process has failed because of an WebException.</exception>
        public void DownloadPackageAsync()
        {
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
        ///     Checks if the size of the package is too big.
        /// </summary>
        /// <returns></returns>
        private bool CheckPackageSize()
        {
            if (
                SizeConverter.ConvertBytesToMegabytes(
                    (int)
                        new FileInfo(Path.Combine(Path.GetTempPath(), "nUpdate", Application.ProductName, "update.zip"))
                            .Length) > 1000)
                return false;
            return true;
        }

        /// <summary>
        ///     Checks if the package contains a valid signature.
        /// </summary>
        /// <returns>Returns if the package contains a valid signature.</returns>
        public bool CheckPackageValidity()
        {
            if (!File.Exists(_updateFilePath))
                throw new FileNotFoundException("The update package to check could not be found.");

            if (Signature == null || Signature.Length <= 0)
                throw new ArgumentNullException("Signature");

            byte[] data = File.ReadAllBytes(_updateFilePath);
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
                Application.ProductName, LanguageCulture.ToString()
            };

            var startInfo = new ProcessStartInfo();
            startInfo.FileName = unpackerAppPath;
            startInfo.Arguments = String.Join("|", args);
            startInfo.Verb = "runas";

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