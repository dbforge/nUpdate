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
    ///     Provides functionality to update .NET-applications.
    /// </summary>
    public class UpdateManager : IDisposable
    {
        private bool _closeHostApplication = true;
        private bool _includeCurrentPcIntoStatistics = true;
        private bool _disposed;
        private CancellationTokenSource _searchCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _downloadCancellationTokenSource = new CancellationTokenSource();
        private bool _hasDownloadCancelled;
        private bool _hasDownloadFailed;
        private IEnumerable<UpdateConfiguration> _updateConfigurations;
        private readonly Dictionary<UpdateVersion, string> _packageFilePaths = new Dictionary<UpdateVersion, string>();

        private readonly Dictionary<UpdateVersion, IEnumerable<Operation>> _packageOperations =
            new Dictionary<UpdateVersion, IEnumerable<Operation>>();

        private readonly string _applicationUpdateDirectory = Path.Combine(Path.GetTempPath(), "nUpdate",
            Application.ProductName);
        private LocalizationProperties _lp;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateManager"/> class.
        /// </summary>
        /// <param name="updateConfigurationFileUri">The URI of the update configuration file.</param>
        /// <param name="publicKey">The public key for the validity check of the update packages.</param>
        /// <param name="languageCulture">The language culture to use for the localization of the integrated UpdaterUI.</param>
        /// <remarks>The public key can be found in the overview of your project when you're opening it in nUpdate Administration. If you have problems inserting the data (or if you want to save time) you can scroll down there and follow the steps of the category "Copy data" which will automatically generate the necessray code for you.</remarks>
        public UpdateManager(Uri updateConfigurationFileUri, string publicKey,
            CultureInfo languageCulture)
        {
            UpdateConfigurationFileUri = updateConfigurationFileUri;
            PublicKey = publicKey;
            LanguageCulture = languageCulture;
            if (CultureFilePaths == null)
                CultureFilePaths = new Dictionary<CultureInfo, string>();
            if (Arguments == null)
                Arguments = new List<UpdateArgument>();

            var projectAssembly = Assembly.GetCallingAssembly();
            var nUpateVersionAttribute = projectAssembly.GetCustomAttributes(false).OfType<nUpdateVersionAttribute>().SingleOrDefault();
            if (nUpateVersionAttribute == null)
                throw new ArgumentException("The version string couldn't be loaded because the nUpdateVersionAttribute isn't implemented in the executing assembly.");

            CurrentVersion = new UpdateVersion(nUpateVersionAttribute.VersionString);
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
        ///     Gets the configurations for the update packages that should be downloaded and installed.
        /// </summary>
        public IEnumerable<UpdateConfiguration> PackageConfigurations
        {
            get { return _updateConfigurations; }
            internal set { _updateConfigurations = value; }
        }

        /// <summary>
        ///     Gets or sets if the current PC should be involved in entries made on the statistics server, if one is available.
        /// </summary>
        public bool IncludeCurrentPcIntoStatistics
        {
            get { return _includeCurrentPcIntoStatistics; }
            set { _includeCurrentPcIntoStatistics = value; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the host application should be closed when the update installer begins, or not.
        /// </summary>
        public bool CloseHostApplication
        {
            get { return _closeHostApplication; }
            set { _closeHostApplication = value; }
        }

        /// <summary>
        ///     Gets the total size of all update packages.
        /// </summary>
        public double TotalSize { get; private set; }

        /// <summary>
        ///     Gets or sets the proxy to use, if wished.
        /// </summary>
        public WebProxy Proxy { get; set; }

        /// <summary>
        ///     Gets or sets the arguments that should be handled over to the application.
        /// </summary>
        public List<UpdateArgument> Arguments { get; set; } 

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

            _searchCancellationTokenSource.Dispose();
            _downloadCancellationTokenSource.Dispose();
            _disposed = true;
        }

        private double? GetUpdatePackageSize(Uri packageUri)
        {
            try
            {
                var req = WebRequest.Create(packageUri);
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

        private void ResetTokens()
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
        /// <returns>Returns <c>true</c> if updates were found; otherwise, <c>false</c>.</returns>
        /// <exception cref="SizeCalculationException">The calculation of the size of the available updates has failed.</exception>
        /// <exception cref="OperationCanceledException">The update search was canceled.</exception>
        public bool SearchForUpdates()
        {
            ResetTokens();
            _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (!ConnectionChecker.IsConnectionAvailable())
                return false;

            _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();

            // Check for SSL and ignore it
            ServicePointManager.ServerCertificateValidationCallback += delegate { return (true); };
            var configurations = UpdateConfiguration.Download(UpdateConfigurationFileUri, Proxy);
            _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();

            var result = new UpdateResult(configurations, CurrentVersion,
                IncludeAlpha, IncludeBeta);

            if (!result.UpdatesFound)
                return false;

            _updateConfigurations = result.NewestConfigurations;
            double updatePackageSize = 0;
            foreach (var updateConfiguration in _updateConfigurations)
            {
                var newPackageSize = GetUpdatePackageSize(updateConfiguration.UpdatePackageUri);
                if (newPackageSize == null)
                    throw new SizeCalculationException(_lp.PackageSizeCalculationExceptionText);

                updatePackageSize += newPackageSize.Value;
                _packageOperations.Add(new UpdateVersion(updateConfiguration.LiteralVersion),
                    updateConfiguration.Operations);
            }

            TotalSize = updatePackageSize;
            _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();

            return true;
        }

        /// <summary>
        ///     Searches for updates, asynchronously.
        /// </summary>
        public void SearchForUpdatesAsync()
        {
            TaskEx.Run(() => SearchForUpdates()).ContinueWith(SearchTaskCompleted,
                    _searchCancellationTokenSource.Token,
                    TaskContinuationOptions.None,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void SearchTaskCompleted(Task<bool> task)
        {
            if (_searchCancellationTokenSource.IsCancellationRequested)
                return;

            var exception = task.Exception;
            if (exception != null)
                OnUpdateSearchFailed(exception.InnerException ?? exception);
            OnUpdateSearchFinished(task.Result);
        }

        /// <summary>
        ///     Provides a task that searches for updates and can be wrapped asynchronously.
        /// </summary>
        /// <returns>Returns <c>true</c> if updates are available; otherwise, <c>false</c>.</returns>
        /// <exception cref="SizeCalculationException">The calculation of total size of the packages has failed.</exception>
        /// <exception cref="OperationCanceledException">The update search was canceled.</exception>
        public async Task<bool> SearchForUpdatesTask()
        {
            return SearchForUpdates();
        }

        /// <summary>
        ///     Cancels the active update search.
        /// </summary>
        /// <remarks>If there is no search task running, nothing will happen.</remarks>
        public void CancelSearch()
        {
            _searchCancellationTokenSource.Cancel();
        }

        /// <summary>
        ///     Downloads the available update packages from the server.
        /// </summary>
        /// <exception cref="WebException">The download process has failed because of an <see cref="WebException"/>.</exception>
        /// <exception cref="ArgumentException">The URI of the update package is null.</exception>
        /// <exception cref="IOException">The creation of the directory, where the update packages should be saved in, failed.</exception>
        /// <exception cref="IOException">An exception occured while writing to the file.</exception>
        /// <exception cref="OperationCanceledException">The download was canceled.</exception>
        /// <seealso cref="DownloadPackagesAsync"/>
        /// <seealso cref="DownloadPackagesTask"/>
        public void DownloadPackages()
        {
            InternalDownloadPackage(ImplementationPattern.Synchronous, null);
        }

        /// <summary>
        ///     Downloads the available update packages from the server, asynchronously.
        /// </summary>
        /// <seealso cref="DownloadPackagesTask"/>
        /// <seealso cref="DownloadPackages"/>
        public void DownloadPackagesAsync()
        {
            TaskEx.Run(() => InternalDownloadPackage(ImplementationPattern.EventBasedAsynchronous, null)).ContinueWith(DownloadTaskCompleted,
                    _searchCancellationTokenSource.Token,
                    TaskContinuationOptions.None,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void DownloadTaskCompleted(Task task)
        {
            if (_downloadCancellationTokenSource.IsCancellationRequested)
                return;

            var exception = task.Exception;
            if (exception != null)
                OnUpdateDownloadFailed(exception.InnerException ?? exception);
            OnUpdateDownloadFinished(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Provides a task that downloads the available update packages from the server and can be wrapped asynchronously.
        /// </summary>
        /// <exception cref="WebException">The download process has failed because of an <see cref="WebException"/>.</exception>
        /// <exception cref="ArgumentException">The URI of the update package is null.</exception>
        /// /// <exception cref="IOException">The creation of the directory, where the update packages should be saved in, failed.</exception>
        /// <exception cref="IOException">An exception occured while writing to the file.</exception>
        /// <exception cref="OperationCanceledException">The download was canceled.</exception>
        /// <seealso cref="DownloadPackagesAsync"/>
        /// <seealso cref="DownloadPackages"/>
        public async Task DownloadPackagesTask(IProgress<UpdateDownloadProgressChangedEventArgs> progress)
        {
            await InternalDownloadPackage(ImplementationPattern.TaskBasedAsynchronous, progress);
        }

        private async Task InternalDownloadPackage(ImplementationPattern pattern,
            IProgress<UpdateDownloadProgressChangedEventArgs> progress)
        {
            ResetTokens();
            int received = 0;
            double total = _updateConfigurations.Select(config => GetUpdatePackageSize(config.UpdatePackageUri)).Where(updatePackageSize => updatePackageSize != null).Sum(updatePackageSize => updatePackageSize.Value);

            if (!Directory.Exists(_applicationUpdateDirectory))
                Directory.CreateDirectory(_applicationUpdateDirectory);

            foreach (var updateConfiguration in _updateConfigurations)
            {
                WebResponse webResponse = null;
                try
                {
                    if (_downloadCancellationTokenSource.Token.IsCancellationRequested)
                    {
                        DeletePackages();
                        throw new OperationCanceledException();
                    }

                    var webRequest = WebRequest.Create(updateConfiguration.UpdatePackageUri);
                    switch (pattern)
                    {
                        case ImplementationPattern.TaskBasedAsynchronous:
                            webResponse = await webRequest.GetResponseAsync();
                            break;
                        default:
                            webResponse = webRequest.GetResponse();
                            break;
                    }

                    var buffer = new byte[1024];
                    _packageFilePaths.Add(new UpdateVersion(updateConfiguration.LiteralVersion),
                        Path.Combine(_applicationUpdateDirectory,
                            String.Format("{0}.zip", updateConfiguration.LiteralVersion)));
                    using (FileStream fileStream = File.Create(Path.Combine(_applicationUpdateDirectory,
                        String.Format("{0}.zip", updateConfiguration.LiteralVersion))))
                    {
                        using (Stream input = webResponse.GetResponseStream())
                        {
                            if (input == null)
                                throw new Exception("The response stream couldn't be read.");

                            if (_downloadCancellationTokenSource.Token.IsCancellationRequested)
                            {
                                DeletePackages();
                                throw new OperationCanceledException();
                            }
                            int size;
                            switch (pattern)
                            {
                                case ImplementationPattern.TaskBasedAsynchronous:
                                    size = await input.ReadAsync(buffer, 0, buffer.Length);
                                    break;
                                default:
                                    size = input.Read(buffer, 0, buffer.Length);
                                    break;
                            }

                            while (size > 0)
                            {
                                if (_downloadCancellationTokenSource.Token.IsCancellationRequested)
                                {
                                    DeletePackages();
                                    throw new OperationCanceledException();
                                }

                                switch (pattern)
                                {
                                    case ImplementationPattern.TaskBasedAsynchronous:
                                        await fileStream.WriteAsync(buffer, 0, size);
                                        received += size;
                                        progress.Report(new UpdateDownloadProgressChangedEventArgs(received,
                                            (long)total,
                                            (float)(received / total) * 100));
                                        size = await input.ReadAsync(buffer, 0, buffer.Length);
                                        break;
                                    default:
                                        fileStream.Write(buffer, 0, size);
                                        received += size;
                                        if (pattern == ImplementationPattern.EventBasedAsynchronous)
                                            OnUpdateDownloadProgressChanged(received,
                                                (long) total,
                                                (float) (received/total)*100);
                                        size = input.Read(buffer, 0, buffer.Length);
                                        break;
                                }
                            }

                            if (!updateConfiguration.UseStatistics || !_includeCurrentPcIntoStatistics)
                                continue;

                            try
                            {
                                string response =
                                    new WebClient().DownloadString(String.Format("{0}?versionid={1}&os={2}",
                                        updateConfiguration.UpdatePhpFileUri, updateConfiguration.VersionId,
                                        SystemInformation.OperatingSystemName)); // Only for calling it
                                if (!String.IsNullOrEmpty(response))
                                {
                                    switch (pattern)
                                    {
                                        case ImplementationPattern.TaskBasedAsynchronous:
                                            throw new StatisticsException(String.Format(
                                                _lp.StatisticsScriptExceptionText, response));
                                        default:
                                            OnStatisticsEntryFailed(new Exception(String.Format(
                                                _lp.StatisticsScriptExceptionText, response)));
                                            break;
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                switch (pattern)
                                {
                                    case ImplementationPattern.TaskBasedAsynchronous:
                                        throw new StatisticsException(ex.Message);
                                    default:
                                        OnStatisticsEntryFailed(ex);
                                        break;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    if (webResponse != null) 
                        webResponse.Close();
                }
            }
        }

        /// <summary>
        ///     Cancels the active download.
        /// </summary>
        /// <remarks>If there is no download task running, nothing will happen.</remarks>
        public void CancelDownload()
        {
            _downloadCancellationTokenSource.Cancel();
        }

        /// <summary>
        ///     Returns a value indicating whether the package is valid or not. If it contains an invalid signature, it will be deleted directly.
        /// </summary>
        /// <returns>Returns <c>true</c>' if the package is valid; otherwise <c>false</c>.</returns>
        /// <exception cref="FileNotFoundException">The update package to check could not be found.</exception>
        /// <exception cref="ArgumentException">The signature of the update package is null or empty.</exception>
        public bool CheckPackageValidity()
        {
            foreach (var filePathItem in _packageFilePaths)
            {
                if (!File.Exists(filePathItem.Value))
                    throw new FileNotFoundException(String.Format(_lp.PackageFileNotFoundExceptionText, filePathItem.Key.FullText));

                var configuration = _updateConfigurations.First(config => config.LiteralVersion == filePathItem.Key.ToString());
                if (configuration.Signature == null || configuration.Signature.Length <= 0)
                    throw new ArgumentException(String.Format("Signature of version \"{0}\" is null or empty.", configuration));

                byte[] data;
                using (var reader =
                    new BinaryReader(File.Open(filePathItem.Value,
                        FileMode.Open)))
                {
                    data = reader.ReadBytes((int) reader.BaseStream.Length);
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
                        DeletePackages();
                    }
                    catch (Exception ex)
                    {
                        throw new PackageDeleteException(ex.Message);
                    }
                    return false;
                }

                if (rsa.VerifyData(data, Convert.FromBase64String(configuration.Signature))) 
                    continue;

                try
                {
                    DeletePackages();
                }
                catch (Exception ex)
                {
                    throw new PackageDeleteException(ex.Message);
                }
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Terminates the application.
        /// </summary>
        /// <remarks>If your apllication doesn't terminate correctly or if you want to perform custom actions before terminating, then override this method and implement your own code.</remarks>
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
                ? String.Format("\"{0}\"", CustomInstallerUiAssemblyPath) : String.Empty;
            string[] args =
            {
                String.Format("\"{0}\"", String.Join("%", _packageFilePaths.Select(item => item.Value))), String.Format("\"{0}\"", Application.StartupPath),
                String.Format("\"{0}\"", Application.ExecutablePath),
                String.Format("\"{0}\"", Application.ProductName),
                String.Format("\"{0}\"",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(Serializer.Serialize(_packageOperations)))), 
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
                    _lp.InstallerInitializingErrorCaption,
                    String.Format("\"{0}\"",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(Serializer.Serialize(Arguments)))),
                    String.Format("\"{0}\"", _closeHostApplication)
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
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode != 1223)
                    throw;
                DeletePackages();
                return;
            }

            if (_closeHostApplication)
                TerminateApplication();
        }

        /// <summary>
        ///     Deletes the downloaded update packages locally.
        /// </summary>
        public void DeletePackages()
        {
            foreach (var filePathItem in _packageFilePaths.Where(item => File.Exists(item.Value)))
            {
                File.Delete(filePathItem.Value);
            }
        }

        private void CheckArguments()
        {
            if (UpdateConfigurationFileUri == null)
                throw new ArgumentException("The property \"UpdateInfoFileUrl\" is not initialized.");

            if (!UpdateConfigurationFileUri.ToString().EndsWith(".json"))
                throw new StatisticsException("The update configuration file is not a valid JSON-file.");

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
        ///     Occurs when the download of the packages begins.
        /// </summary>
        public event EventHandler<EventArgs> PackagesDownloadStarted;

        /// <summary>
        ///     Occurs when the download of the package fails.
        /// </summary>
        public event EventHandler<FailedEventArgs> PackagesDownloadFailed;

        /// <summary>
        ///     Occurs when the packages download progress has changed.
        /// </summary>
        public event EventHandler<UpdateDownloadProgressChangedEventArgs> PackagesDownloadProgressChanged;

        /// <summary>
        ///     Occurs when the packages download is finished.
        /// </summary>
        public event EventHandler<EventArgs> PackagesDownloadFinished;

        /// <summary>
        ///     Occurs when the statistics entry failed.
        /// </summary>
        /// <remarks>This event is meant to provide the user with a warning if the statistic server entry fails. The update process shouldn't be cancelled as this doesn't cause any conflicts that could affect it.</remarks>
        public event EventHandler<FailedEventArgs> StatisticsEntryFailed; 

        /// <summary>
        ///     Called when the update search is started.
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
        ///     Called when the download of the updates is started.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnUpdateDownloadStarted(Object sender, EventArgs e)
        {
            if (PackagesDownloadStarted != null)
                PackagesDownloadStarted(sender, e);
        }

        /// <summary>
        ///     Called when the update download progress has changed.
        /// </summary>
        /// <param name="bytesReceived">The amount of bytes received.</param>
        /// <param name="totalBytesToReceive">The total bytes to receive.</param>
        /// <param name="percentage">The progress percentage.</param>
        protected virtual void OnUpdateDownloadProgressChanged(long bytesReceived, long totalBytesToReceive, float percentage)
        {
            if (PackagesDownloadProgressChanged != null)
                PackagesDownloadProgressChanged(this, new UpdateDownloadProgressChangedEventArgs(bytesReceived, totalBytesToReceive, percentage));
        }

        /// <summary>
        ///     Called when the download of the updates is finished.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnUpdateDownloadFinished(Object sender, EventArgs e)
        {
            if (PackagesDownloadFinished != null)
                PackagesDownloadFinished(sender, e);
        }

        /// <summary>
        ///     Called when the download of the updates has failed.
        /// </summary>
        /// <param name="exception">The exception that occured.</param>
        protected virtual void OnUpdateDownloadFailed(Exception exception)
        {
            if (PackagesDownloadFailed != null)
                PackagesDownloadFailed(this, new FailedEventArgs(exception));
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