using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using nUpdate;
using nUpdate.Core;
using nUpdate.Core.Language;
using nUpdate.Dialogs;
using nUpdate.Internal;
using nUpdate.Internal.Exceptions;
using nUpdate.Internal.UpdateEventArgs;
using nUpdate.UI.Dialogs;

namespace nUpdate.Internal
{
    /// <summary>
    /// Class that offers functions to update .NET-applications.
    /// </summary>
    public class UpdateManager 
    {
        private WebClient packageDownloader = new WebClient();
        private CancellationTokenSource searchCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource downloadCancellationTokenSource = new CancellationTokenSource();

        #region "Members"

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateManager"/> class.
        /// </summary>
        public UpdateManager()
            : this(null, String.Empty, null) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateManager"/> class.
        /// </summary>
        /// <param name="updateInfoFileUrl">The url of the info file.</param>
        public UpdateManager(Uri updateInfoFileUrl)
            : this(updateInfoFileUrl, String.Empty, null) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateManager"/>
        /// </summary>
        /// <param name="updateInfoFileUrl">The url of the info file.</param>
        /// <param name="publicKey">The public key to check the signature of an update package.</param>
        /// <param name="currentVersion">The current version of the current application.</param>
        public UpdateManager(Uri updateInfoFileUrl, string publicKey, Version currentVersion)
        {
            UpdateInfoFileUrl = updateInfoFileUrl;
            PublicKey = publicKey;
            CurrentVersion = currentVersion;

            // Create all necessary data
            InitializeWorkaroundArea();

            //LanguageSerializer lang = LanguageSerializer.ReadXml(Properties.Resources.en);
        }

        /// <summary>
        /// Returns if there were updates found.
        /// </summary>
        public bool UpdatesFound
        {
            get;
            private set;
        }

        /// <summary>
        /// Sets the url of the update package.
        /// </summary>
        private Uri UpdatePackageUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the url of the info file.
        /// </summary>
        public Uri UpdateInfoFileUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Sets if there is a proxy used.
        /// </summary>
        public bool UseWebProxy
        {
            get;
            set;
        }

        public ProxySettings WebProxySettings
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the PublicKey for the signature.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// Sets the version of the current application.
        /// </summary>
        public Version CurrentVersion { get; set; }

        /// <summary>
        /// The language to use.
        /// </summary>
        public Language Language { get; set; }

        /// <summary>
        /// The path to the language file. Requires "Language.Custom" and a valid XML-file.
        /// </summary>
        public string LanguageFilePath { get; set; }

        /// <summary>
        /// Sets if the user should be able to update to Alpha-versions.
        /// </summary>
        public bool IncludeAlpha { get; set; }

        /// <summary>
        /// Sets if the user should be able to update to Beta-versions.
        /// </summary>
        public bool IncludeBeta { get; set; }

        /// <summary>
        /// Sets if a hidden search should be provided in order to search in the background without informing the user.
        /// </summary>
        public bool UseHiddenSearch { get; set; }

        /// <summary>
        /// Sets if the found update is a duty update and must be installed.
        /// </summary>
        public bool MustUpdate { get; private set; }

        //bool allowInstallingAllAvailablePackages = false;
        ///// <summary>
        ///// Sets if the user is allowed to choose to which newer version of all available updates he wants to update.
        ///// </summary>
        //public bool AllowInstallingAllAvailablePackages
        //{
        //    get
        //    {
        //        return allowInstallingAllAvailablePackages;
        //    }
        //    set
        //    {
        //        value = allowInstallingAllAvailablePackages;
        //    }
        //}

        /// <summary>
        /// Gets the version of the update package.
        /// </summary>
        public Version UpdateVersion { get; private set; }

        /// <summary>
        /// Gets the changelog of the update package.
        /// </summary>
        public string Changelog { get; private set; }

        /// <summary>
        /// Gets the size of the update package.
        /// </summary>
        public double PackageSize { get; private set; }

        /// <summary>
        /// Gets the signature of the update package.
        /// </summary>
        private byte[] Signature { get; set; }

        /// <summary>
        /// Gets the versions that the update package does not support.
        /// </summary>
        private Version[] UnsupportedVersions { get; set; }

        /// <summary>
        /// Checks if all arguments have been given.
        /// </summary>
        private void CheckArguments() {

            // UpdateInfo-property
            if (UpdateInfoFileUrl == null) {
                throw new ArgumentException("The Property \"UpdateInfoFileUrl\" is not initialized.");
            }

            if (!UpdateInfoFileUrl.ToString().EndsWith(".json")) {
                throw new FormatException("The info file is not a valid JSON-file.");
            }

            // PublicKey-property
            if (String.IsNullOrEmpty(PublicKey))  {
                throw new ArgumentException("The Property \"PublicKey\" is not initialized.");
            }

            // CurrentVersion-property
            if (CurrentVersion == null) {
                CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            }

            if (Language == null)
                throw new ArgumentException("The Property \"Language\" is not initialized.");
        }

        /// <summary>
        /// Creates the necessary data for nUpdate.
        /// </summary>
        private void InitializeWorkaroundArea() {

            if (!Directory.Exists(Path.Combine(Path.GetTempPath(), "nUpdate")))
            {
                try { Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "nUpdate", Application.ProductName)); }
                catch (Exception ex) { throw new IOException(String.Format("The application's main folder could not be created. {0}", ex.Message)); }
            }
        }

        #endregion

        #region "Events"

        public delegate void UpdateSearchFinishedEventHandler(bool updateFound);
        public delegate void FailedEventHandler(Exception exception);

        /// <summary>
        /// The event fired when the update search begins.
        /// </summary>
        public event EventHandler<EventArgs> UpdateSearchStarted;

        /// <summary>
        /// The event fired when the update search is finished.
        /// </summary>
        public event UpdateSearchFinishedEventHandler UpdateSearchFinished;

        /// <summary>
        /// The event fired when the download of the package begins.
        /// </summary>
        public event FailedEventHandler UpdateSearchFailed;

        /// <summary>
        /// The event fired when the download of the package begins.
        /// </summary>
        public event EventHandler<EventArgs> PackageDownloadStarted;

        /// <summary>
        /// The event fired when the download of the package fails.
        /// </summary>
        public event FailedEventHandler PackageDownloadFailed;

        protected virtual void OnUpdateSearchStarted(Object sender, EventArgs e)
        {
            if (UpdateSearchStarted != null) UpdateSearchStarted(sender, e);
        }

        protected virtual void OnUpdateSearchFinished(bool updateFound)
        {
            if (UpdateSearchFinished != null) UpdateSearchFinished(updateFound);
        }

        protected virtual void OnUpdateSearchFailed(Exception exception)
        {
            if (UpdateSearchFailed != null) UpdateSearchFailed(exception);
        }

        protected virtual void OnPackageDownloadStarted(Object sender, EventArgs e)
        {
            if (PackageDownloadStarted != null) PackageDownloadStarted(sender, e);
        }

        public event DownloadProgressChangedEventHandler PackageDownloadProgressChanged
        {
            add { packageDownloader.DownloadProgressChanged += value; }
            remove { packageDownloader.DownloadProgressChanged -= value; }
        }

        public event AsyncCompletedEventHandler PackageDownloadFinished
        {
            add { packageDownloader.DownloadFileCompleted += value; }
            remove { packageDownloader.DownloadFileCompleted -= value; }
        }

        protected virtual void OnPackageDownloadFailed(Exception exception)
        {
            if (PackageDownloadFailed != null) PackageDownloadFailed(exception);
        }


        #endregion

        /// <summary>
        /// Gets the size of the update package.
        /// </summary>
        /// <param name="packageUrl">The link where the update package can be found.</param>
        /// <returns>Returns the size in bytes as a double.</returns>
        private double GetUpdatePackageSize(Uri packageUrl)
        {
            try
            {
                System.Net.WebRequest req = System.Net.HttpWebRequest.Create(packageUrl);
                req.Method = "HEAD";
                using (System.Net.WebResponse resp = req.GetResponse())
                {
                    double ContentLength;
                    if (double.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                    {
                        return ContentLength;
                    }
                }
            }

            catch
            {
                return -1;
            }

            return -1;
        }

        /// <summary>
        /// Refreshes and re-initializes the cancellation tokens.
        /// </summary>
        private void RefreshCancellationTokens()
        {
            if (searchCancellationTokenSource != null)
                searchCancellationTokenSource.Dispose();
            searchCancellationTokenSource = new CancellationTokenSource();

            if (downloadCancellationTokenSource != null)
                downloadCancellationTokenSource.Dispose();
            downloadCancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Checks if there are updates available.
        /// </summary>
        /// <exception cref="InvalidOperationException">There is already a search process running.</exception>
        /// <exception cref="NetworkException">There is no network connection available..</exception
        public void CheckForUpdates()
        {
            RefreshCancellationTokens();

            if (!ConnectionChecker.IsConnectionAvailable())
                throw new NetworkException("No network connection available.");

            UpdateConfiguration updateConfig = new UpdateConfiguration();
            UpdateResult result = new UpdateResult(updateConfig.LoadUpdateConfiguration(UpdateInfoFileUrl), 
                CurrentVersion, IncludeAlpha, IncludeBeta);

            if (!result.UpdatesFound) {
                OnUpdateSearchFinished(false);
                return;
            }

            updateConfig = result.NewestPackage;
            UpdateVersion = new Version(updateConfig.UpdateVersion);
            Changelog = updateConfig.Changelog;
            Signature = Convert.FromBase64String(updateConfig.Signature);
            UpdatePackageUrl = new Uri(updateConfig.UpdatePackageUrl);
            DevelopmentalStage developmentalStage = (DevelopmentalStage)Enum.Parse(typeof(DevelopmentalStage), updateConfig.DevelopmentalStage);
            MustUpdate = updateConfig.MustUpdate;

            // Set the size
            PackageSize = GetUpdatePackageSize(UpdatePackageUrl);
            OnUpdateSearchFinished(true);
        }

        /// <summary>
        /// Checks if there are updates available. This method does not block the calling thread.
        /// </summary>
        /// <exception cref="InvalidOperationException">There is already a search process running.</exception>
        /// <exception cref="NetworkException">There is no network connection available..</exception>
        public void CheckForUpdatesAsync() 
        {
            var task =
            Task.Factory.StartNew(this.CheckForUpdates).ContinueWith(this.SearchExceptionHandler,
                    searchCancellationTokenSource.Token,
                    TaskContinuationOptions.OnlyOnFaulted,
                    TaskScheduler.FromCurrentSynchronizationContext()).ContinueWith( o => this.SearchTaskCompleted(),
                            searchCancellationTokenSource.Token,
                            TaskContinuationOptions.NotOnFaulted,
                            TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// The handler set if the async task for the update search throws an exception.
        /// </summary>
        /// <param name="task">The taskto handle the sended the exceptions.</param>
        private void SearchExceptionHandler(Task task)
        {
            var exception = task.Exception;
            if (exception != null && exception.InnerExceptions.Count > 0)
            {
                OnUpdateSearchFailed(exception.InnerException);
            }
        }

        /// <summary>
        /// Internal method to call when the search task has completed.
        /// </summary>
        private void SearchTaskCompleted() { }

        /// <summary>
        /// Downloads the update package.
        /// </summary>
        /// <exception cref="NetworkException">There is no network connection available..</exception>
        /// <exception cref="WebException">The download process has failed because of an WebException.</exception>
        private void DownloadPackage()
        {
            RefreshCancellationTokens();

            if (!ConnectionChecker.IsConnectionAvailable())
                throw new NetworkException("No network connection available.");

            if (String.IsNullOrEmpty(this.UpdatePackageUrl.ToString()))
                throw new ArgumentException("UpdatePackageUrl");

            OnPackageDownloadStarted(this, EventArgs.Empty);

            if (!Directory.Exists(Path.Combine(Path.GetTempPath(), "nUpdate", Application.ProductName)))
            {
                try
                {
                    Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "nUpdate", Application.ProductName));
                }
                catch (Exception ex)
                {
                    OnPackageDownloadFailed(ex);
                }
            }

            packageDownloader.DownloadFileAsync(UpdatePackageUrl, Path.Combine(Path.GetTempPath(), "nUpdate", Application.ProductName, "update.zip"));
        }

        /// <summary>
        /// Downloads the update package. This method does not block the calling thread.
        /// </summary>
        /// <exception cref="NetworkException">There is no network connection available..</exception>
        /// <exception cref="WebException">The download process has failed because of an WebException.</exception>
        public void DownloadPackageAsync()
        {
            var task =
            Task.Factory.StartNew(this.DownloadPackage).ContinueWith(this.DownloadExceptionHandler,
                    downloadCancellationTokenSource.Token,
                    TaskContinuationOptions.OnlyOnFaulted,
                    TaskScheduler.FromCurrentSynchronizationContext()).ContinueWith(o => this.DownloadTaskCompleted(),
                            downloadCancellationTokenSource.Token,
                            TaskContinuationOptions.NotOnFaulted,
                            TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// The handler set if the async task for the update search throws an exception.
        /// </summary>
        /// <param name="task">The task to handle the sended exceptions from.</param>
        private void DownloadExceptionHandler(Task task)
        {
            var exception = task.Exception;
            if (exception != null && exception.InnerExceptions.Count > 0)
            {
                OnPackageDownloadFailed(exception.InnerException);
            }
        }

        /// <summary>
        /// Internal method to call when the search task has completed.
        /// </summary>
        private void DownloadTaskCompleted() { }

        /// <summary>
        /// Checks if the size of the package is too big.
        /// </summary>
        /// <returns></returns>
        internal bool CheckPackageSize()
        {
            if (SizeConverter.ConvertBytesToMegabytes((int)new FileInfo(Path.Combine(Path.GetTempPath(), "nUpdate", Application.ProductName, "update.zip")).Length) > 1000)
            {
                ErrorHandler.ShowErrorDialog(0, "The update file is too big.", "nUpdate will not allow to install the update in order to save your RAM.");
                try
                {
                    DeletePackage();
                    return false;
                }
                catch
                {
                    // TODO: Show "Did not work"
                    return false;
                }
            }
            else
                return true;
        }

        /// <summary>
        /// Checks if the package contains a valid signature.
        /// </summary>
        /// <returns>Returns if the package contains a valid signature.</returns>
        public bool CheckPackageValidility()
        {
            string packageOfUpdate = Path.Combine(Path.GetTempPath(), "nUpdate", Application.ProductName, "update.zip");

            if (!File.Exists(packageOfUpdate))
                throw new FileNotFoundException("The update package to check could not be found." + Environment.NewLine + "The function call was invalid at this point.");

            if (Signature == null || Signature.Length <= 0)
                throw new NullReferenceException("The signature is null or empty.");

            byte[] data = File.ReadAllBytes(packageOfUpdate); 

            RsaSignature rsa;

            try { rsa = new RsaSignature(PublicKey); }
            catch 
            {
                DeletePackage();
                return false; 
            }

            if (rsa.VerifyData(data, Signature))
                return true;

            else
            {
                DeletePackage();
                return false;
            }
        }

        /// <summary>
        /// Installs the update package and overwrites the old data in the directory.
        /// </summary>
        public void InstallPackage()
        {
            string unpackerDirectory = Path.Combine(Path.GetTempPath(), "nUpdate Installer");
            string unpackerZipPath = Path.Combine(unpackerDirectory, "Ionic.Zip.dll");
            string unpackerAppPath = Path.Combine(unpackerDirectory, "nUpdate UpdateInstaller.exe");

            if (!Directory.Exists(unpackerDirectory))
                Directory.CreateDirectory(unpackerDirectory);

            if (!File.Exists(unpackerZipPath))
                File.WriteAllBytes(unpackerZipPath, Properties.Resources.Ionic_Zip);

            if (!File.Exists(unpackerAppPath))
                File.WriteAllBytes(unpackerAppPath, Properties.Resources.nUpdate_UpdateInstaller);

            string unpackString = "Unpacking";
            string[] args = { Application.ProductName, unpackString };

            ProcessStartInfo updateInfo = new ProcessStartInfo();
            updateInfo.FileName = unpackerAppPath;
            updateInfo.Arguments = String.Join(String.Empty, args);
            updateInfo.Verb = "runas";

            try
            {
                Process.Start(updateInfo);
            }
            catch (Win32Exception)
            {
                DeletePackage();
                return;
            }

            Application.Exit();
        }

        /// <summary>
        /// Deletes the package.
        /// </summary>
        public void DeletePackage()
        {
            string packageFile = Path.Combine(Path.GetTempPath(), "nUpdate", Application.ProductName, "update.zip");
            if (File.Exists(packageFile))
                File.Delete(packageFile);
        }
    }
}


