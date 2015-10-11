// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using nUpdate.Exceptions;
using nUpdate.Properties;
using nUpdate.UpdateEventArgs;
using System.Diagnostics;
using nUpdate.Localization;
using nUpdate.Operations;

namespace nUpdate.Updating
{
    /// <summary>
    ///     Provides functionality to update .NET-applications.
    /// </summary>
    public class UpdateManager : IDisposable
    {
        private bool _closeHostApplication = true;
        private bool _includeCurrentPcIntoStatistics = true;
        private readonly string _publicKey;
        private readonly Uri _updateConfigurationFileUri;
        private CultureInfo _languageCulture = new CultureInfo("en");
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
        ///     Initializes a new instance of the <see cref="UpdateManager" /> class.
        /// </summary>
        /// <param name="updateConfigurationFileUri">The <see cref="Uri"/> of the update configuration.</param>
        /// <param name="publicKey">The public key for the validity check of the update packages.</param>
        /// <remarks>
        ///     The public key can be found in the overview of your project when you're opening it in nUpdate Administration.
        ///     If you have problems inserting the data (or if you want to save time) you can scroll down and follow the steps of "Copy data" which will automatically generate the necessary code for you.
        /// </remarks>
        public UpdateManager(Uri updateConfigurationFileUri, string publicKey)
        {
            if (updateConfigurationFileUri == null)
                throw new ArgumentNullException(nameof(updateConfigurationFileUri));
            _updateConfigurationFileUri = updateConfigurationFileUri;

            if (string.IsNullOrEmpty(publicKey))
                throw new ArgumentNullException(nameof(publicKey));
            _publicKey = publicKey;

            var projectAssembly = Assembly.GetCallingAssembly();
            var nUpateVersionAttribute =
                projectAssembly.GetCustomAttributes(false).OfType<nUpdateVersionAttribute>().SingleOrDefault();
            if (nUpateVersionAttribute == null)
                throw new ArgumentException(
                    "The version string couldn't be loaded because the nUpdateVersionAttribute isn't implemented in the executing assembly.");

            CurrentVersion = new UpdateVersion(nUpateVersionAttribute.VersionString);
            if (UseCustomInstallerUserInterface && string.IsNullOrEmpty(CustomInstallerUiAssemblyPath))
                throw new ArgumentException(
                    "The property \"CustomInstallerUiAssemblyPath\" is not initialized although \"UseCustomInstallerUserInterface\" is set to \"true\"");
            Initialize();
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="UpdateManager" /> class.
        /// </summary>
        ~UpdateManager()
        {
            Dispose(true);
        }

        /// <summary>
        ///     Gets or sets the URI of the update configuration file.
        /// </summary>
        public Uri UpdateConfigurationFileUri => _updateConfigurationFileUri;

        /// <summary>
        ///     Gets or sets the public key for checking the validity of the signature.
        /// </summary>
        public string PublicKey => _publicKey;

        /// <summary>
        ///     Gets or sets the version of the current application.
        /// </summary>
        internal UpdateVersion CurrentVersion { get; set; }

        /// <summary>
        ///     Gets or sets the culture of the language to use.
        /// </summary>
        /// <remarks>
        ///     "en" (English) and "de" (German) are currently the only language cultures that are already implemented in
        ///     nUpdate. In order to use own languages download the language template from
        ///     <see href="http://www.nupdate.net/langtemplate.json" />, edit it, save it as a JSON-file and add a new entry to
        ///     property
        ///     CultureFilePaths with the relating CultureInfo and path which locates the JSON-file on the client's
        ///     system (e. g. AppData).
        /// </remarks>
        public CultureInfo LanguageCulture
        {
            get { return _languageCulture; }
            set
            {
                var existingCultureInfos = new[] { new CultureInfo("en"), new CultureInfo("de-DE"), new CultureInfo("de-AT"), new CultureInfo("de-CH") };
                if (!existingCultureInfos.Any(item => item.Equals(_languageCulture)) &&
                    !CultureFilePaths.ContainsKey(_languageCulture))
                    throw new ArgumentException(
                        "The given culture info does neither exist in nUpdate's resources, nor in property \"CultureFilePaths\".");
                _languageCulture = value;
            }
        }

        /// <summary>
        ///     Gets or sets the paths the language files of to the relating cultures.
        /// </summary>
        public Dictionary<CultureInfo, string> CultureFilePaths { get; set; } = new Dictionary<CultureInfo, string>();

        /// <summary>
        ///     Gets or sets a value indicating whether a custom user interface should be used for the update installer, or not.
        /// </summary>
        public bool UseCustomInstallerUserInterface { get; set; }

        /// <summary>
        ///     Gets or sets the path of the custom assembly'S location that should be used for the update installer's user
        ///     interface.
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
        ///     Gets or sets a value indicating whether the host application should be closed when the update installer begins, or
        ///     not.
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
        public List<UpdateArgument> Arguments { get; set; } = new List<UpdateArgument>();

        /// <summary>
        ///     Releases all managed and unmanaged resources used by the current <see cref="UpdateManager" />-instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
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

        /// <summary>
        ///     Searches for updates.
        /// </summary>
        /// <returns>Returns <c>true</c> if updates were found; otherwise, <c>false</c>.</returns>
        /// <exception cref="SizeCalculationException">The calculation of the size of the available updates has failed.</exception>
        /// <exception cref="OperationCanceledException">The update search was canceled.</exception>
        public bool SearchForUpdates()
        {
            _searchCancellationTokenSource?.Dispose();
            _searchCancellationTokenSource = new CancellationTokenSource();

            if (!WebConnection.IsAvailable())
                return false;

            // Check for SSL and ignore it
            ServicePointManager.ServerCertificateValidationCallback += delegate { return (true); };
            var configuration = UpdateConfiguration.Download(_updateConfigurationFileUri, Proxy);

            var result = new UpdateResult(configuration, CurrentVersion,
                IncludeAlpha, IncludeBeta);

            if (result.Requirements.Count != 0)
                OnMissingRequirement(new MissingRequirementsEventArgs(result.Requirements));

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
            if (_searchCancellationTokenSource.Token.IsCancellationRequested)
            {
                _packageOperations.Clear();
                _packageFilePaths.Clear();
                throw new OperationCanceledException();
            }

            return true;
        }

        /// <summary>
        ///     Searches for updates, asynchronously.
        /// </summary>
        /// <seealso cref="SearchForUpdates"/>
        public
#if PROVIDE_TAP
      async Task<bool>
#else
 void
#endif
 SearchForUpdatesAsync()
        {
#if PROVIDE_TAP
            return SearchForUpdates();
#else
            Task.Factory.StartNew(() => SearchForUpdates()).ContinueWith(SearchTaskCompleted,
                _searchCancellationTokenSource.Token,
                TaskContinuationOptions.None, TaskScheduler.Default);
#endif
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
        /// <exception cref="WebException">The download process has failed because of an <see cref="WebException" />.</exception>
        /// <exception cref="ArgumentException">The URI of the update package is null.</exception>
        /// <exception cref="IOException">The creation of the directory, where the update packages should be saved in, failed.</exception>
        /// <exception cref="IOException">An exception occured while writing to the file.</exception>
        /// <exception cref="OperationCanceledException">The download was canceled.</exception>
        /// <seealso cref="DownloadPackagesAsync" />
        /// <seealso cref="DownloadPackagesTask" />
        public void DownloadPackages()
        {
            _downloadCancellationTokenSource?.Dispose();
            _downloadCancellationTokenSource = new CancellationTokenSource();

            long received = 0;
            double total = _updateConfigurations.Select(config => GetUpdatePackageSize(config.UpdatePackageUri))
                .Where(updatePackageSize => updatePackageSize != null)
                .Sum(updatePackageSize => updatePackageSize.Value);

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
                        _packageOperations.Clear();
                        _packageFilePaths.Clear();
                        throw new OperationCanceledException();
                    }

                    var webRequest = WebRequest.Create(updateConfiguration.UpdatePackageUri);
                    using (webResponse = webRequest.GetResponse())
                    {
                        var buffer = new byte[1024];
                        _packageFilePaths.Add(new UpdateVersion(updateConfiguration.LiteralVersion),
                            Path.Combine(_applicationUpdateDirectory,
                                $"{updateConfiguration.LiteralVersion}.zip"));
                        using (FileStream fileStream = File.Create(Path.Combine(_applicationUpdateDirectory,
                            $"{updateConfiguration.LiteralVersion}.zip")))
                        {
                            using (Stream input = webResponse.GetResponseStream())
                            {
                                if (input == null)
                                    throw new Exception("The response stream couldn't be read.");

                                if (_downloadCancellationTokenSource.Token.IsCancellationRequested)
                                {
                                    DeletePackages();
                                    _packageOperations.Clear();
                                    _packageFilePaths.Clear();
                                    throw new OperationCanceledException();
                                }

                                int size = input.Read(buffer, 0, buffer.Length);
                                while (size > 0)
                                {
                                    if (_downloadCancellationTokenSource.Token.IsCancellationRequested)
                                    {
                                        fileStream.Flush();
                                        fileStream.Close();
                                        DeletePackages();
                                        _packageOperations.Clear();
                                        _packageFilePaths.Clear();
                                        throw new OperationCanceledException();
                                    }

                                    fileStream.Write(buffer, 0, size);
                                    received += size;
                                    OnUpdateDownloadProgressChanged(received,
                                        (long)total, (float)(received / total) * 100);
                                    size = input.Read(buffer, 0, buffer.Length);
                                }

                                if (!updateConfiguration.UseStatistics || !_includeCurrentPcIntoStatistics)
                                    continue;

                                try
                                {
                                    string response =
                                        new WebClient().DownloadString(
                                            $"{updateConfiguration.UpdatePhpFileUri}?versionid={updateConfiguration.VersionId}&os={SystemInformation.OperatingSystemName}"); // Only for calling it
                                    if (!string.IsNullOrEmpty(response))
                                    {
                                        throw new StatisticsException(string.Format(
                                            _lp.StatisticsScriptExceptionText, response));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new StatisticsException(ex.Message);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    webResponse?.Close();
                }
            }
        }

        public void DownloadPackagesAsync()
        {
            Task.Factory.StartNew(DownloadPackages).ContinueWith(DownloadTaskCompleted,
                _downloadCancellationTokenSource.Token,
                TaskContinuationOptions.None,
                TaskScheduler.Default);
        }

#if PROVIDE_TAP

        /// <summary>
        ///     Downloads the available update packages from the server, asynchronously.
        /// </summary>
        /// <seealso cref="DownloadPackages" />
        public async Task DownloadPackagesAsync(IProgress<UpdateDownloadProgressChangedEventArgs> progress)
        {
            if (_downloadCancellationTokenSource != null)
                _downloadCancellationTokenSource.Dispose();
            _downloadCancellationTokenSource = new CancellationTokenSource();

            long received = 0;
            double total = _updateConfigurations.Select(config => GetUpdatePackageSize(config.UpdatePackageUri))
                    .Where(updatePackageSize => updatePackageSize != null)
                    .Sum(updatePackageSize => updatePackageSize.Value);

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
                        _packageOperations.Clear();
                        _packageFilePaths.Clear();
                        throw new OperationCanceledException();
                    }

                    var webRequest = WebRequest.Create(updateConfiguration.UpdatePackageUri);
                    webResponse = await webRequest.GetResponseAsync();

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
                                _packageOperations.Clear();
                                _packageFilePaths.Clear();
                                throw new OperationCanceledException();
                            }

                            int size = await input.ReadAsync(buffer, 0, buffer.Length);
                            while (size > 0)
                            {
                                if (_downloadCancellationTokenSource.Token.IsCancellationRequested)
                                {
                                    fileStream.Flush();
                                    fileStream.Close();
                                    DeletePackages();
                                    _packageOperations.Clear();
                                    _packageFilePaths.Clear();
                                    throw new OperationCanceledException();
                                }

                                await fileStream.WriteAsync(buffer, 0, size);
                                received += size;
                                progress.Report(new UpdateDownloadProgressChangedEventArgs(received,
                                    (long) total, (float) (received/total)*100));
                                size = await input.ReadAsync(buffer, 0, buffer.Length);
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
                                    throw new StatisticsException(String.Format(
                                        _lp.StatisticsScriptExceptionText, response));
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new StatisticsException(ex.Message);
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

    
#endif

        private void DownloadTaskCompleted(Task task)
        {
            if (_downloadCancellationTokenSource.IsCancellationRequested)
                return;

            var exception = task.Exception;
            if (exception != null)
                OnUpdateDownloadFailed(exception.InnerException ?? exception);
            else
                OnUpdateDownloadFinished(this, EventArgs.Empty);
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
        ///     Returns a value indicating whether the signature of each package is valid, or not. If a package contains an invalid
        ///     signature, it will be deleted directly.
        /// </summary>
        /// <returns>Returns <c>true</c> if the package is valid; otherwise <c>false</c>.</returns>
        /// <exception cref="FileNotFoundException">The update package to check could not be found.</exception>
        /// <exception cref="ArgumentException">The signature of the update package is null or empty.</exception>
        public bool ValidatePackages()
        {
            foreach (var filePathItem in _packageFilePaths)
            {
                if (!File.Exists(filePathItem.Value))
                    throw new FileNotFoundException(string.Format(_lp.PackageFileNotFoundExceptionText,
                        filePathItem.Key.FullText));

                var configuration =
                    _updateConfigurations.First(config => config.LiteralVersion == filePathItem.Key.ToString());
                if (configuration.Signature == null || configuration.Signature.Length <= 0)
                    throw new ArgumentException($"Signature of version \"{configuration}\" is null or empty.");

                using (var stream = File.Open(filePathItem.Value, FileMode.Open))
                {
                    RsaManager rsa;

                    try
                    {
                        rsa = new RsaManager(_publicKey);
                    }
                    catch
                    {
                        try
                        {
                            DeletePackages();
                            _packageFilePaths.Clear();
                            _packageOperations.Clear();
                        }
                        catch (Exception ex)
                        {
                            throw new PackageDeleteException(ex.Message);
                        }
                        return false;
                    }

                    if (rsa.VerifyData(stream, Convert.FromBase64String(configuration.Signature)))
                        continue;

                    try
                    {
                        DeletePackages();
                    }
                    catch (Exception ex)
                    {
                        throw new PackageDeleteException(ex.Message);
                    }

                    _packageFilePaths.Clear();
                    _packageOperations.Clear();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///     Terminates the application.
        /// </summary>
        /// <remarks>
        ///     If your apllication doesn't terminate correctly or if you want to perform custom actions before terminating,
        ///     then override this method and implement your own code.
        /// </remarks>
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
                ? $"\"{CustomInstallerUiAssemblyPath}\""
                : string.Empty;
            string[] args =
            {
                $"\"{string.Join("%", _packageFilePaths.Select(item => item.Value))}\"",
                $"\"{Application.StartupPath}\"",
                $"\"{Application.ExecutablePath}\"",
                $"\"{Application.ProductName}\"",
                $"\"{Convert.ToBase64String(Encoding.UTF8.GetBytes(Serializer.Serialize(_packageOperations)))}\"",
                $"\"{installerUiAssemblyPath}\"",
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
                $"\"{Convert.ToBase64String(Encoding.UTF8.GetBytes(Serializer.Serialize(Arguments)))}\"",
                $"\"{_closeHostApplication}\"",
                $"\"{_lp.InstallerFileInUseError}\"",
            };

            var startInfo = new ProcessStartInfo
            {
                FileName = unpackerAppPath,
                Arguments = string.Join("|", args),
                UseShellExecute = true,
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
                _packageFilePaths.Clear();
                _packageOperations.Clear();
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
                    throw new IOException(string.Format(_lp.MainFolderCreationExceptionText,
                        ex.Message));
                }
            }

            var languageFilePath = CultureFilePaths.Any(item => item.Key.Equals(_languageCulture))
                ? CultureFilePaths.First(item => item.Key.Equals(_languageCulture)).Value
                : null;

            if (!string.IsNullOrEmpty(languageFilePath))
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
            else if (string.IsNullOrEmpty(languageFilePath) && _languageCulture.Name != "en")
            {
                string resourceName = $"nUpdate.Core.Localization.{_languageCulture.Name}.json";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    _lp = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }
            else if (string.IsNullOrEmpty(languageFilePath) && _languageCulture.Name == "en")
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
        /// <remarks>
        ///     This event is meant to provide the user with a warning if the statistic server entry fails. The update process
        ///     shouldn't be cancelled as this doesn't cause any conflicts that could affect it.
        /// </remarks>
        public event EventHandler<FailedEventArgs> StatisticsEntryFailed;

        /// <summary>
        ///     Occurs when some requirements are missing
        /// </summary>
        public event EventHandler<MissingRequirementsEventArgs> MissingRequirement;

        /// <summary>
        ///     Called when the update search is started.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnUpdateSearchStarted(object sender, EventArgs e)
        {
            UpdateSearchStarted?.Invoke(sender, e);
        }

        /// <summary>
        ///     Called when the update search is finished.
        /// </summary>
        /// <param name="updateAvailable">if set to <c>true</c> updates are available.</param>
        protected virtual void OnUpdateSearchFinished(bool updateAvailable)
        {
            UpdateSearchFinished?.Invoke(this, new UpdateSearchFinishedEventArgs(updateAvailable));
        }

        /// <summary>
        ///     Called when the update search failed.
        /// </summary>
        /// <param name="exception">The exception that occured.</param>
        protected virtual void OnUpdateSearchFailed(Exception exception)
        {
            UpdateSearchFailed?.Invoke(this, new FailedEventArgs(exception));
        }

        /// <summary>
        ///     Called when the download of the updates is started.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnUpdateDownloadStarted(object sender, EventArgs e)
        {
            PackagesDownloadStarted?.Invoke(sender, e);
        }

        /// <summary>
        ///     Called when the update download progress has changed.
        /// </summary>
        /// <param name="bytesReceived">The amount of bytes received.</param>
        /// <param name="totalBytesToReceive">The total bytes to receive.</param>
        /// <param name="percentage">The progress percentage.</param>
        protected virtual void OnUpdateDownloadProgressChanged(long bytesReceived, long totalBytesToReceive,
            float percentage)
        {
            PackagesDownloadProgressChanged?.Invoke(this,
                new UpdateDownloadProgressChangedEventArgs(bytesReceived, totalBytesToReceive, percentage));
        }

        /// <summary>
        ///     Called when the download of the updates is finished.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnUpdateDownloadFinished(object sender, EventArgs e)
        {
            PackagesDownloadFinished?.Invoke(sender, e);
        }

        /// <summary>
        ///     Called when the download of the updates has failed.
        /// </summary>
        /// <param name="exception">The exception that occured.</param>
        protected virtual void OnUpdateDownloadFailed(Exception exception)
        {
            PackagesDownloadFailed?.Invoke(this, new FailedEventArgs(exception));
        }

        /// <summary>
        ///     Called when the statistics entry failed.
        /// </summary>
        /// <param name="exception">The exception that occured.</param>
        protected virtual void OnStatisticsEntryFailed(Exception exception)
        {
            StatisticsEntryFailed?.Invoke(this, new FailedEventArgs(exception));
        }

        /// <summary>
        ///     Called when some requirements are missing
        /// </summary>
        /// <param name="exception">The exception that occured.</param>
        protected virtual void OnMissingRequirement(MissingRequirementsEventArgs requirements)
        {
            if (MissingRequirement != null)
                MissingRequirement(this, new MissingRequirementsEventArgs(requirements.Requirements));
        }
    }
}