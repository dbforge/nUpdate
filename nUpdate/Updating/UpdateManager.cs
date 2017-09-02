// Copyright © Dominic Beger 2017

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
        private bool _disposed;
        private CancellationTokenSource _searchCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _downloadCancellationTokenSource = new CancellationTokenSource();
        private readonly Dictionary<UpdateVersion, string> _packageFilePaths = new Dictionary<UpdateVersion, string>();

        private readonly Dictionary<UpdateVersion, IEnumerable<Operation>> _packageOperations =
            new Dictionary<UpdateVersion, IEnumerable<Operation>>();

        private readonly string _applicationUpdateDirectory = Path.Combine(Path.GetTempPath(), "nUpdate",
            Application.ProductName);

        private LocalizationProperties _lp;
        private CultureInfo _languageCulture = new CultureInfo("en");

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateManager" /> class.
        /// </summary>
        /// <param name="updateConfigurationFileUri">The URI of the update configuration file.</param>
        /// <param name="publicKey">The public key for the validity check of the update packages.</param>
        /// <param name="languageCulture">
        ///     The language culture to use. If no value is provided, the default one ("en") will be
        ///     used.
        /// </param>
        /// <param name="currentVersion">
        ///     The current version of that should be used for the update checks. This parameter has a
        ///     higher priority than the <see cref="nUpdateVersionAttribute" /> and will replace it, if specified.
        /// </param>
        /// <remarks>
        ///     The public key can be found in the overview of your project when you're opening it in nUpdate Administration.
        ///     If you have problems inserting the data (or if you want to save time) you can scroll down there and follow the
        ///     steps of the category "Copy data" which will automatically generate the necessray code for you.
        /// </remarks>
        public UpdateManager(Uri updateConfigurationFileUri, string publicKey,
            CultureInfo languageCulture = null, UpdateVersion currentVersion = null)
        {
            if (updateConfigurationFileUri == null)
                throw new ArgumentNullException(nameof(updateConfigurationFileUri));
            UpdateConfigurationFileUri = updateConfigurationFileUri;

            if (string.IsNullOrEmpty(publicKey))
                throw new ArgumentNullException(nameof(publicKey));
            PublicKey = publicKey;

            CultureFilePaths = new Dictionary<CultureInfo, string>();
            Arguments = new List<UpdateArgument>();

            var projectAssembly = Assembly.GetCallingAssembly();
            var nUpateVersionAttribute =
                projectAssembly.GetCustomAttributes(false).OfType<nUpdateVersionAttribute>().SingleOrDefault();

            // TODO: This is just there to make sure we don't create an API-change that would require a new Major version. This will be changed/removed in v4.0.
            // Before v3.0-beta8 it was not possible to provide the current version except using the nUpdateVersionAttribute.
            // In order to allow specific features, e.g. updating another application and not the current one (as it's done by a launcher), there must be a way to provide this version separately.
            // So, if an argument is specified for the "currentVersion" parameter, we will use this one instead of the nUpdateVersionAttribute.
            if (currentVersion != null)
                CurrentVersion = currentVersion;
            else
            {
                // Neither the nUpdateVersionAttribute nor the additional parameter argument was provided.
                if (nUpateVersionAttribute == null)
                    throw new ArgumentException(
                        "The version string couldn't be loaded because the nUpdateVersionAttribute isn't implemented in the executing assembly and no version was provided explicitly.");

                CurrentVersion = new UpdateVersion(nUpateVersionAttribute.VersionString);
            }

            // TODO: This is just there to make sure we don't create an API-change that would require a new Major version. This will be changed/removed in v4.0.
            // Before v3.0-beta5 it was not possible to use custom languages due to a mistake in the architecture. So we can be pretty sure that nobody specifies a custom CultureInfo in the constructor.
            // We only need these two lines for those who specified one of the implemented CultureInfos here as they shouldn't have to change anything when updating to v3.0-beta5.
            // Nevertheless, it's therefore possible to use custom CultureInfos just by leaving the optional parameter "null" and specifying the culture using the corresponding properties. So, both cases are covered with that solution.
            if (languageCulture != null && LocalizationHelper.IsIntegratedCulture(languageCulture, CultureFilePaths))
                LanguageCulture = languageCulture;
            else
                throw new ArgumentException($"The culture \"{languageCulture}\" is not defined.");

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

        public SynchronizationContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the URI of the update configuration file.
        /// </summary>
        public Uri UpdateConfigurationFileUri { get; }

        /// <summary>
        ///     Gets or sets the public key for checking the validity of the signature.
        /// </summary>
        public string PublicKey { get; }

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
            get => _languageCulture;
            set
            {
                if (!LocalizationHelper.IsIntegratedCulture(value, CultureFilePaths) &&
                    !CultureFilePaths.ContainsKey(_languageCulture))
                    throw new ArgumentException(
                        "The localization file of the culture set does not exist.");
                _lp = LocalizationHelper.GetLocalizationProperties(value, CultureFilePaths);
                _languageCulture = value;
            }
        }

        /// <summary>
        ///     Gets or sets the paths to the files that contain the localized strings of their corresponding <see cref="CultureInfo"/>.
        /// </summary>
        public Dictionary<CultureInfo, string> CultureFilePaths { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the nUpdate UpdateInstaller should use a custom user interface, or not.
        /// </summary>
        /// <remarks>This property also requires <see cref="CustomInstallerUiAssemblyPath"/> to be set, if the value is <c>true</c>.</remarks>
        public bool UseCustomInstallerUserInterface { get; set; }

        /// <summary>
        ///     Gets or sets the path of the assembly file that contains the user interface data for nUpdate UpdateInstaller.
        /// </summary>
        public string CustomInstallerUiAssemblyPath { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the user should be able to update to alpha versions, or not.
        /// </summary>
        public bool IncludeAlpha { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the user should be able to update to beta versions, or not.
        /// </summary>
        public bool IncludeBeta { get; set; }

        /// <summary>
        ///     Gets the package configurations for all available updates.
        /// </summary>
        public IEnumerable<UpdateConfiguration> PackageConfigurations { get; internal set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the current computer should be included into the statistics, or not.
        /// </summary>
        public bool IncludeCurrentPcIntoStatistics { get; set; } = true;

        /// <summary>
        ///     Gets or sets a value indicating whether the host application should be closed when the nUpdate UpdateInstaller is started, or
        ///     not.
        /// </summary>
        public bool CloseHostApplication { get; set; } = true;

        /// <summary>
        ///     Gets or sets a value indicating whether the host application should be restarted once the update installation has
        ///     completed, or not.
        /// </summary>
        public bool RestartHostApplication { get; set; } = true;

        /// <summary>
        ///     Gets the total size of all update packages.
        /// </summary>
        public double TotalSize { get; private set; }

        /// <summary>
        ///     Gets or sets the proxy to use.
        /// </summary>
        public WebProxy Proxy { get; set; }

        /// <summary>
        ///     Gets or sets the HTTP(S) authentication credentials.
        /// </summary>
        public NetworkCredential HttpAuthenticationCredentials { get; set; }

        /// <summary>
        ///     Gets or sets the arguments that should be handled over to the application once the update installation has completed.
        /// </summary>
        public List<UpdateArgument> Arguments { get; set; }

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
                if (HttpAuthenticationCredentials != null)
                    req.Credentials = HttpAuthenticationCredentials;
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
            // It may be that this is not the first search call and previously saved data needs to be disposed.
            Cleanup();
            _searchCancellationTokenSource?.Dispose();
            _searchCancellationTokenSource = new CancellationTokenSource();

            OnUpdateSearchStarted(this, EventArgs.Empty);
            if (!ConnectionManager.IsConnectionAvailable())
                return false;

            // Check for SSL and ignore it
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            var configuration =
                UpdateConfiguration.Download(UpdateConfigurationFileUri, HttpAuthenticationCredentials, Proxy);

            var result = new UpdateResult(configuration, CurrentVersion,
                IncludeAlpha, IncludeBeta);
            if (!result.UpdatesFound)
                return false;

            PackageConfigurations = result.NewestConfigurations;
            double updatePackageSize = 0;
            foreach (var updateConfiguration in PackageConfigurations)
            {
                var newPackageSize = GetUpdatePackageSize(updateConfiguration.UpdatePackageUri);
                if (newPackageSize == null)
                    throw new SizeCalculationException(_lp.PackageSizeCalculationExceptionText);

                updatePackageSize += newPackageSize.Value;
                _packageOperations.Add(new UpdateVersion(updateConfiguration.LiteralVersion),
                    updateConfiguration.Operations);
            }

            TotalSize = updatePackageSize;
            if (!_searchCancellationTokenSource.Token.IsCancellationRequested)
                return true;

            Cleanup();
            throw new OperationCanceledException();
        }

        /// <summary>
        ///     Searches for updates, asynchronously.
        /// </summary>
        /// <seealso cref="SearchForUpdates" />
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
            Task.Factory.StartNew(SearchForUpdates).ContinueWith(SearchTaskCompleted,
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
        ///     Cancels the update search.
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

            OnUpdateDownloadStarted(this, EventArgs.Empty);

            long received = 0;
            var total = PackageConfigurations.Select(config => GetUpdatePackageSize(config.UpdatePackageUri))
                .Where(updatePackageSize => updatePackageSize != null)
                .Sum(updatePackageSize => updatePackageSize.Value);

            if (!Directory.Exists(_applicationUpdateDirectory))
                Directory.CreateDirectory(_applicationUpdateDirectory);

            foreach (var updateConfiguration in PackageConfigurations)
            {
                WebResponse webResponse = null;
                try
                {
                    if (_downloadCancellationTokenSource.Token.IsCancellationRequested)
                    {
                        DeletePackages();
                        Cleanup();
                        throw new OperationCanceledException();
                    }

                    var webRequest = WebRequest.Create(updateConfiguration.UpdatePackageUri);
                    if (HttpAuthenticationCredentials != null)
                        webRequest.Credentials = HttpAuthenticationCredentials;
                    using (webResponse = webRequest.GetResponse())
                    {
                        var buffer = new byte[1024];
                        _packageFilePaths.Add(new UpdateVersion(updateConfiguration.LiteralVersion),
                            Path.Combine(_applicationUpdateDirectory,
                                $"{updateConfiguration.LiteralVersion}.zip"));
                        using (var fileStream = File.Create(Path.Combine(_applicationUpdateDirectory,
                            $"{updateConfiguration.LiteralVersion}.zip")))
                        {
                            using (var input = webResponse.GetResponseStream())
                            {
                                if (input == null)
                                    throw new Exception("The response stream couldn't be read.");

                                if (_downloadCancellationTokenSource.Token.IsCancellationRequested)
                                {
                                    DeletePackages();
                                    Cleanup();
                                    throw new OperationCanceledException();
                                }

                                var size = input.Read(buffer, 0, buffer.Length);
                                while (size > 0)
                                {
                                    if (_downloadCancellationTokenSource.Token.IsCancellationRequested)
                                    {
                                        fileStream.Flush();
                                        fileStream.Close();
                                        DeletePackages();
                                        Cleanup();
                                        throw new OperationCanceledException();
                                    }

                                    fileStream.Write(buffer, 0, size);
                                    received += size;
                                    OnUpdateDownloadProgressChanged(received,
                                        (long) total, (float) (received / total) * 100);
                                    size = input.Read(buffer, 0, buffer.Length);
                                }

                                if (!updateConfiguration.UseStatistics || !IncludeCurrentPcIntoStatistics)
                                    continue;

                                var response =
                                    new WebClient {Credentials = HttpAuthenticationCredentials}.DownloadString(
                                        $"{updateConfiguration.UpdatePhpFileUri}?versionid={updateConfiguration.VersionId}&os={SystemInformation.OperatingSystemName}"); // Only for calling it

                                if (string.IsNullOrEmpty(response))
                                    return;
                                OnStatisticsEntryFailed(new StatisticsException(string.Format(
                                    _lp.StatisticsScriptExceptionText, response)));
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

        /// <summary>
        ///     Downloads the available update packages from the server, asynchronously.
        /// </summary>
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
            _downloadCancellationTokenSource?.Dispose();
            _downloadCancellationTokenSource = new CancellationTokenSource();

            long received = 0;
            var total = PackageConfigurations.Select(config => GetUpdatePackageSize(config.UpdatePackageUri))
                .Where(updatePackageSize => updatePackageSize != null)
                .Sum(updatePackageSize => updatePackageSize.Value);

            if (!Directory.Exists(_applicationUpdateDirectory))
                Directory.CreateDirectory(_applicationUpdateDirectory);

            foreach (var updateConfiguration in PackageConfigurations)
            {
                WebResponse webResponse = null;
                try
                {
                    if (_downloadCancellationTokenSource.Token.IsCancellationRequested)
                    {
                        DeletePackages();
                        Cleanup();
                        throw new OperationCanceledException();
                    }

                    var webRequest = WebRequest.Create(updateConfiguration.UpdatePackageUri);
                    if (HttpAuthenticationCredentials != null)
                        webRequest.Credentials = HttpAuthenticationCredentials;
                    webResponse = await webRequest.GetResponseAsync();

                    var buffer = new byte[1024];
                    _packageFilePaths.Add(new UpdateVersion(updateConfiguration.LiteralVersion),
                        Path.Combine(_applicationUpdateDirectory,
                            $"{updateConfiguration.LiteralVersion}.zip"));
                    using (var fileStream = File.Create(Path.Combine(_applicationUpdateDirectory,
                        $"{updateConfiguration.LiteralVersion}.zip")))
                    {
                        using (var input = webResponse.GetResponseStream())
                        {
                            if (input == null)
                                throw new Exception("The response stream couldn't be read.");

                            if (_downloadCancellationTokenSource.Token.IsCancellationRequested)
                            {
                                DeletePackages();
                                Cleanup();
                                throw new OperationCanceledException();
                            }

                            var size = await input.ReadAsync(buffer, 0, buffer.Length);
                            while (size > 0)
                            {
                                if (_downloadCancellationTokenSource.Token.IsCancellationRequested)
                                {
                                    fileStream.Flush();
                                    fileStream.Close();
                                    DeletePackages();
                                    Cleanup();
                                    throw new OperationCanceledException();
                                }

                                await fileStream.WriteAsync(buffer, 0, size);
                                received += size;
                                progress.Report(new UpdateDownloadProgressChangedEventArgs(received,
                                    (long) total, (float) (received / total) * 100));
                                size = await input.ReadAsync(buffer, 0, buffer.Length);
                            }

                            if (!updateConfiguration.UseStatistics || !IncludeCurrentPcIntoStatistics)
                                continue;

                            var response =
                                new WebClient
                                {
                                    Credentials =
                                        HttpAuthenticationCredentials
                                }.DownloadString(
                                    $"{updateConfiguration.UpdatePhpFileUri}?versionid={updateConfiguration.VersionId}&os={SystemInformation.OperatingSystemName}"); // Only for calling it
                            if (!String.IsNullOrEmpty(response))
                            {
                                throw new StatisticsException(String.Format(
                                    _lp.StatisticsScriptExceptionText, response));
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
        ///     Cancels the download.
        /// </summary>
        /// <remarks>If there is no download task running, nothing will happen.</remarks>
        public void CancelDownload()
        {
            _downloadCancellationTokenSource.Cancel();
        }

        /// <summary>
        ///     Returns a value indicating whether the signature of each package is valid, or not. If a package contains an invalid
        ///     signature, it will be deleted.
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
                    PackageConfigurations.First(config => config.LiteralVersion == filePathItem.Key.ToString());
                if (configuration.Signature == null || configuration.Signature.Length <= 0)
                    throw new ArgumentException($"Signature of version \"{configuration}\" is null or empty.");

                var stream = File.Open(filePathItem.Value, FileMode.Open);
                try
                {
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
                            Cleanup();
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

                    Cleanup();
                    return false;
                }
                finally
                {
                    stream.Close();
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
            Environment.Exit(0);
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

            File.WriteAllBytes(unpackerZipPath, Resources.Ionic_Zip);
            File.WriteAllBytes(guiInterfacePath, Resources.nUpdate_UpdateInstaller_Client_GuiInterface);
            File.WriteAllBytes(jsonNetPath, Resources.Newtonsoft_Json);
            File.WriteAllBytes(jsonNetPdbPath, Resources.Newtonsoft_Json_Pdb);
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
                $"\"{CloseHostApplication}\"",
                $"\"{RestartHostApplication}\"",
                $"\"{_lp.InstallerFileInUseError}\""
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
                DeletePackages();
                Cleanup();
                if (ex.NativeErrorCode != 1223)
                    throw;
                return;
            }

            if (CloseHostApplication)
                TerminateApplication();
        }

        /// <summary>
        ///     Deletes the downloaded update packages.
        /// </summary>
        public void DeletePackages()
        {
            foreach (var filePathItem in _packageFilePaths.Where(item => File.Exists(item.Value)))
                File.Delete(filePathItem.Value);
        }

        private void Initialize()
        {
            if (Directory.Exists(Path.Combine(Path.GetTempPath(), "nUpdate")))
                return;

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

        private void Cleanup()
        {
            _packageFilePaths.Clear();
            _packageOperations.Clear();
        }

        /// <summary>
        ///     Occurs when an update search is started.
        /// </summary>
        public event EventHandler<EventArgs> UpdateSearchStarted;

        /// <summary>
        ///     Occurs when an active update search has finished.
        /// </summary>
        public event EventHandler<UpdateSearchFinishedEventArgs> UpdateSearchFinished;

        /// <summary>
        ///     Occurs when an update search has failed.
        /// </summary>
        public event EventHandler<FailedEventArgs> UpdateSearchFailed;

        /// <summary>
        ///     Occurs when the download of the packages begins.
        /// </summary>
        public event EventHandler<EventArgs> PackagesDownloadStarted;

        /// <summary>
        ///     Occurs when the download of the packages fails.
        /// </summary>
        public event EventHandler<FailedEventArgs> PackagesDownloadFailed;

        /// <summary>
        ///     Occurs when the download progress of the packages has changed.
        /// </summary>
        public event EventHandler<UpdateDownloadProgressChangedEventArgs> PackagesDownloadProgressChanged;

        /// <summary>
        ///     Occurs when the download of the packages has finished.
        /// </summary>
        public event EventHandler<EventArgs> PackagesDownloadFinished;

        /// <summary>
        ///     Occurs when the statistics entry failed.
        /// </summary>
        /// <remarks>
        ///     This event is meant to provide the user with a warning, if the statistic server entry fails. The update process
        ///     should not be canceled as this does not cause any problems that could affect it.
        /// </remarks>
        public event EventHandler<FailedEventArgs> StatisticsEntryFailed;
        
        protected virtual void OnUpdateSearchStarted(object sender, EventArgs e)
        {
            UpdateSearchStarted?.Invoke(sender, e);
        }

        protected virtual void OnUpdateSearchFinished(bool updateAvailable)
        {
            UpdateSearchFinished?.Invoke(this, new UpdateSearchFinishedEventArgs(updateAvailable));
        }
        
        protected virtual void OnUpdateSearchFailed(Exception exception)
        {
            UpdateSearchFailed?.Invoke(this, new FailedEventArgs(exception));
        }
        
        protected virtual void OnUpdateDownloadStarted(object sender, EventArgs e)
        {
            PackagesDownloadStarted?.Invoke(sender, e);
        }

        protected virtual void OnUpdateDownloadProgressChanged(long bytesReceived, long totalBytesToReceive,
            float percentage)
        {
            PackagesDownloadProgressChanged?.Invoke(this,
                new UpdateDownloadProgressChangedEventArgs(bytesReceived, totalBytesToReceive, percentage));
        }

        protected virtual void OnUpdateDownloadFinished(object sender, EventArgs e)
        {
            PackagesDownloadFinished?.Invoke(sender, e);
        }
        
        protected virtual void OnUpdateDownloadFailed(Exception exception)
        {
            PackagesDownloadFailed?.Invoke(this, new FailedEventArgs(exception));
        }
        
        protected virtual void OnStatisticsEntryFailed(Exception exception)
        {
            StatisticsEntryFailed?.Invoke(this, new FailedEventArgs(exception));
        }
    }
}