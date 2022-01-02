// UpdateManager.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

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
using nUpdate.Exceptions;
using nUpdate.Localization;
using nUpdate.Operations;
using nUpdate.Properties;
using nUpdate.UpdateEventArgs;

namespace nUpdate.Updating
{
    /// <summary>
    ///     Provides functionality to update .NET-applications.
    /// </summary>
    public class UpdateManager : IDisposable
    {
        private readonly string _applicationUpdateDirectory;
        private readonly string _executablePath;
        private readonly string _startupPath;
        private readonly string _productName;

        private readonly Dictionary<UpdateVersion, string> _packageFilePaths = new Dictionary<UpdateVersion, string>();

        private Dictionary<UpdateVersion, IEnumerable<Operation>> _packageOperations; // obsolete

        private bool _disposed;

        private CancellationTokenSource _downloadCancellationTokenSource = new CancellationTokenSource();
        private CultureInfo _languageCulture = new CultureInfo("en");

        private LocalizationProperties _lp;
        private CancellationTokenSource _searchCancellationTokenSource = new CancellationTokenSource();

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
        /// <param name="applicationName">
        ///     The application's name.
        ///     By default, this will be derived from the entry assembly. Set this manually, if this results in a wrong application name.
        /// </param>
        /// <param name="applicationExecutablePath">
        ///     The path of the application's executable file (.exe) that should be (re)started after the update installation.
        ///     By default, this will be derived from the entry assembly. Set this manually, if this results in the wrong application being opened.
        /// </param>
        /// <remarks>
        ///     The public key can be found in the overview of your project when you're opening it in nUpdate Administration.
        ///     If you have problems inserting the data (or if you want to save time) you can scroll down there and follow the
        ///     steps of the category "Copy data" which will automatically generate the necessary code for you.
        /// </remarks>
        public UpdateManager(Uri updateConfigurationFileUri, string publicKey,
            CultureInfo languageCulture = null, UpdateVersion currentVersion = null, string applicationName = null, string applicationExecutablePath = null)
        {
            UpdateConfigurationFileUri = updateConfigurationFileUri ??
                                         throw new ArgumentNullException(nameof(updateConfigurationFileUri));

            if (string.IsNullOrEmpty(publicKey))
                throw new ArgumentNullException(nameof(publicKey));
            PublicKey = publicKey;

            CultureFilePaths = new Dictionary<CultureInfo, string>();
            Arguments = new List<UpdateArgument>();

            var projectAssembly = Assembly.GetEntryAssembly();
            if (projectAssembly == null)
                throw new Exception("The entry assembly could not be determined.");

            var nUpdateVersionAttribute =
                projectAssembly.GetCustomAttributes(false).OfType<nUpdateVersionAttribute>().SingleOrDefault();

            _productName = applicationName ?? projectAssembly.GetName().Name;

            if (applicationExecutablePath != null)
            {
                _executablePath = applicationExecutablePath;
            }
            else
            {
                _executablePath = projectAssembly.Location;

                // It may happen that the calling assembly is not the .exe, but another library. This is the case in .NET Core 6, for example.
                // We try to fix the path automatically. If this still does not work, the user can use the ExecutableFilePath property to set the path manually.
                if (!_executablePath.EndsWith(".exe"))
                    _executablePath = Path.Combine(Path.GetDirectoryName(_executablePath) ?? string.Empty, $"{_productName}.exe");
            }

            _startupPath = Path.GetDirectoryName(_executablePath);
            _applicationUpdateDirectory = Path.Combine(Path.GetTempPath(), "nUpdate",
                _productName);

            // TODO: This is just there to make sure we don't create an API-change that would require a new Major version. This will be changed/removed in v5.0.
            // Before v3.0-beta8 it was not possible to provide the current version except using the nUpdateVersionAttribute.
            // In order to allow specific features, e.g. updating another application and not the current one (as it's done by a launcher), there must be a way to provide this version separately.
            // So, if an argument is specified for the "currentVersion" parameter, we will use this one instead of the nUpdateVersionAttribute.
            if (currentVersion != null)
            {
                CurrentVersion = currentVersion;
            }
            else
            {
                // Neither the nUpdateVersionAttribute nor the additional parameter argument was provided.
                if (nUpdateVersionAttribute == null)
                    throw new ArgumentException(
                        "The version string couldn't be loaded because the nUpdateVersionAttribute isn't implemented in the executing assembly and no version was provided explicitly.");

                CurrentVersion = new UpdateVersion(nUpdateVersionAttribute.VersionString);
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
        ///     Gets or sets the arguments that should be handled over to the application once the update installation has
        ///     completed.
        /// </summary>
        public List<UpdateArgument> Arguments { get; set; }

        /// <summary>
        ///     Gets or sets the paths to the files that contain the localized strings of their corresponding
        ///     <see cref="CultureInfo" />.
        /// </summary>
        public Dictionary<CultureInfo, string> CultureFilePaths { get; set; }

        /// <summary>
        ///     Gets or sets the version of the current application.
        /// </summary>
        internal UpdateVersion CurrentVersion { get; set; }

        /// <summary>
        ///     Gets or sets the path of the assembly file that contains the user interface data for nUpdate UpdateInstaller.
        /// </summary>
        public string CustomInstallerUiAssemblyPath { get; set; }

        /// <summary>
        ///     Gets or sets the update installer options for the host application.
        /// </summary>
        public HostApplicationOptions HostApplicationOptions { get; set; }

        /// <summary>
        ///     Gets or sets the HTTP(S) authentication credentials.
        /// </summary>
        public NetworkCredential HttpAuthenticationCredentials { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the user should be able to update to alpha versions, or not.
        /// </summary>
        public bool IncludeAlpha { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the user should be able to update to beta versions, or not.
        /// </summary>
        public bool IncludeBeta { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the current computer should be included into the statistics, or not.
        /// </summary>
        public bool IncludeCurrentPcIntoStatistics { get; set; } = true;

        /// <summary>
        ///     Gets or sets the additional conditions that determine whether an update should be loaded or not.
        /// </summary>
        public List<KeyValuePair<string, string>> Conditions { get; set; }

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
        ///     Gets the package configurations for all available updates.
        /// </summary>
        public IEnumerable<UpdateConfiguration> PackageConfigurations { get; internal set; }

        /// <summary>
        ///     Gets or sets the proxy to use.
        /// </summary>
        public WebProxy Proxy { get; set; }

        /// <summary>
        ///     Gets or sets the public key for checking the validity of the signature.
        /// </summary>
        public string PublicKey { get; }

        public bool RunInstallerAsAdmin { get; set; } = true;

        /// <summary>
        ///     Gets or sets the timeout in milliseconds that should be used when searching for updates.
        /// </summary>
        /// <remarks>By default, this is set to 10.000 milliseconds.</remarks>
        public int SearchTimeout { get; set; } = 10000;

        /// <summary>
        ///     Gets the total size of all update packages.
        /// </summary>
        public double TotalSize { get; private set; }

        /// <summary>
        ///     Gets or sets the URI of the update configuration file.
        /// </summary>
        public Uri UpdateConfigurationFileUri { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the nUpdate UpdateInstaller should use a custom user interface, or not.
        /// </summary>
        /// <remarks>
        ///     This property also requires <see cref="CustomInstallerUiAssemblyPath" /> to be set, if the value is
        ///     <c>true</c>.
        /// </remarks>
        public bool UseCustomInstallerUserInterface { get; set; }

        public bool UseDynamicUpdateUri { get; set; } = false;

        /// <summary>
        ///     Cancels the download.
        /// </summary>
        /// <remarks>If there is no download task running, nothing will happen.</remarks>
        [Obsolete("CancelDownload has been renamed to CancelDownloadAsync which should be used instead.")]
        public void CancelDownload()
        {
            CancelDownloadAsync();
        }

        /// <summary>
        ///     Cancels the download, if it is running asynchronously.
        /// </summary>
        /// <remarks>If there is no asynchronous download task running, nothing will happen.</remarks>
        public void CancelDownloadAsync()
        {
            _downloadCancellationTokenSource.Cancel();
        }

        /// <summary>
        ///     Cancels the update search, if it is running asynchronously.
        /// </summary>
        /// <remarks>If there is no asynchronous search task running, nothing will happen.</remarks>
        [Obsolete("CancelSearch has been renamed to CancelSearchAsync which should be used instead.")]
        public void CancelSearch()
        {
            CancelSearchAsync();
        }

        /// <summary>
        ///     Cancels the update search.
        /// </summary>
        /// <remarks>If there is no search task running, nothing will happen.</remarks>
        public void CancelSearchAsync()
        {
            _searchCancellationTokenSource.Cancel();
        }

        /// <summary>
        ///     Downloads the available update packages from the server.
        /// </summary>
        /// <seealso cref="DownloadPackagesAsync" />
        public void DownloadPackages()
        {
            if (!Directory.Exists(_applicationUpdateDirectory))
                Directory.CreateDirectory(_applicationUpdateDirectory);

            foreach (var updateConfiguration in PackageConfigurations)
            {
                WebResponse webResponse = null;
                try
                {
                    var webRequest = WebRequestWrapper.Create(updateConfiguration.UpdatePackageUri);
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

                                var size = input.Read(buffer, 0, buffer.Length);
                                while (size > 0)
                                {
                                    fileStream.Write(buffer, 0, size);
                                    size = input.Read(buffer, 0, buffer.Length);
                                }

                                if (!updateConfiguration.UseStatistics || !IncludeCurrentPcIntoStatistics)
                                    continue;

                                var response =
                                    new WebClient { Credentials = HttpAuthenticationCredentials }.DownloadString(
                                        $"{updateConfiguration.UpdatePhpFileUri}?versionid={updateConfiguration.VersionId}&os={SystemInformation.OperatingSystemName}"); // Only for calling it

                                if (string.IsNullOrEmpty(response))
                                    return;
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
        /// <exception cref="OperationCanceledException" />
        /// <exception cref="StatisticsException" />
        /// <seealso cref="DownloadPackages" />
        public Task DownloadPackagesAsync(IProgress<UpdateDownloadProgressChangedEventArgs> progress = null)
        {
            return Task.Run(async () =>
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

                        var webRequest = WebRequestWrapper.Create(updateConfiguration.UpdatePackageUri);
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
                                    progress?.Report(new UpdateDownloadProgressChangedEventArgs(received,
                                        (long)total, (float)(received / total) * 100));
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
                                if (!string.IsNullOrEmpty(response))
                                    throw new StatisticsException(string.Format(
                                        _lp.StatisticsScriptExceptionText, response));
                            }
                        }
                    }
                    finally
                    {
                        webResponse?.Close();
                    }
                }
            });
        }

        /// <summary>
        ///     Searches for updates.
        /// </summary>
        /// <returns>Returns <c>true</c> if updates were found; otherwise, <c>false</c>.</returns>
        /// <exception cref="SizeCalculationException">The calculation of the size of the available updates has failed.</exception>
        public bool SearchForUpdates()
        {
            // It may be that this is not the first search call and previously saved data needs to be disposed.
            Cleanup();

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            var configuration =
                UpdateConfiguration.Download(UpdateConfigurationFileUri, HttpAuthenticationCredentials, Proxy,
                    SearchTimeout);

            var result = new UpdateResult(configuration, CurrentVersion,
                IncludeAlpha, IncludeBeta, Conditions);
            if (!result.UpdatesFound)
                return false;

            PackageConfigurations = result.NewestConfigurations;
            double updatePackageSize = 0;
            foreach (var updateConfiguration in PackageConfigurations)
            {
                updateConfiguration.UpdatePackageUri = ConvertPackageUri(updateConfiguration.UpdatePackageUri);
                updateConfiguration.UpdatePhpFileUri = ConvertStatisticsUri(updateConfiguration.UpdatePhpFileUri);

                var newPackageSize = GetUpdatePackageSize(updateConfiguration.UpdatePackageUri);
                if (newPackageSize == null)
                    throw new SizeCalculationException(_lp.PackageSizeCalculationExceptionText);

                updatePackageSize += newPackageSize.Value;
                if (updateConfiguration.Operations == null) continue;
                if (_packageOperations == null)
                    _packageOperations = new Dictionary<UpdateVersion, IEnumerable<Operation>>();
                _packageOperations.Add(new UpdateVersion(updateConfiguration.LiteralVersion),
                    updateConfiguration.Operations);
            }

            TotalSize = updatePackageSize;
            return true;
        }

        /// <summary>
        ///     Searches for updates, asynchronously.
        /// </summary>
        /// <seealso cref="SearchForUpdates" />
        /// <exception cref="SizeCalculationException" />
        /// <exception cref="OperationCanceledException" />
        public Task<bool> SearchForUpdatesAsync()
        {
            return Task.Run(async () =>
            {
                // It may be that this is not the first search call and previously saved data needs to be disposed.
                Cleanup();
                _searchCancellationTokenSource?.Dispose();
                _searchCancellationTokenSource = new CancellationTokenSource();

                _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                var configuration =
                    await UpdateConfiguration.DownloadAsync(UpdateConfigurationFileUri, HttpAuthenticationCredentials,
                        Proxy, _searchCancellationTokenSource, SearchTimeout);

                _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var result = new UpdateResult(configuration, CurrentVersion,
                    IncludeAlpha, IncludeBeta, Conditions);
                if (!result.UpdatesFound)
                    return false;

                PackageConfigurations = result.NewestConfigurations;
                double updatePackageSize = 0;
                foreach (var updateConfiguration in PackageConfigurations)
                {
                    updateConfiguration.UpdatePackageUri = ConvertPackageUri(updateConfiguration.UpdatePackageUri);
                    updateConfiguration.UpdatePhpFileUri = ConvertStatisticsUri(updateConfiguration.UpdatePhpFileUri);

                    _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var newPackageSize = GetUpdatePackageSize(updateConfiguration.UpdatePackageUri);
                    if (newPackageSize == null)
                        throw new SizeCalculationException(_lp.PackageSizeCalculationExceptionText);

                    updatePackageSize += newPackageSize.Value;
                    if (updateConfiguration.Operations == null) continue;
                    if (_packageOperations == null)
                        _packageOperations = new Dictionary<UpdateVersion, IEnumerable<Operation>>();
                    _packageOperations.Add(new UpdateVersion(updateConfiguration.LiteralVersion),
                        updateConfiguration.Operations);
                }

                TotalSize = updatePackageSize;
                if (!_searchCancellationTokenSource.Token.IsCancellationRequested)
                    return true;
                throw new OperationCanceledException();
            });
        }

        private void Cleanup()
        {
            _packageFilePaths.Clear();
        }

        private Uri ConvertPackageUri(Uri updatePackageUri)
        {
            if (!UseDynamicUpdateUri)
                return updatePackageUri;
            if (updatePackageUri == null)
                throw new ArgumentNullException(nameof(updatePackageUri));

            // The segment of the correct update package URI should include: "/", "x.x.x.x/", "*.zip".
            if (updatePackageUri.Segments.Length < 3)
                throw new ArgumentException($@"""{updatePackageUri}"" is not a valid update package URI.",
                    nameof(updatePackageUri));

            var packageNameSegment = updatePackageUri.Segments.Last();
            var versionSegment = updatePackageUri.Segments[updatePackageUri.Segments.Length - 2];
            var baseUri = UpdateConfigurationFileUri.GetLeftPart(UriPartial.Authority);
            var path = string.Join(string.Empty, UpdateConfigurationFileUri.Segments, 0,
                UpdateConfigurationFileUri.Segments.Length - 1);

            return new Uri($"{baseUri}{path}{versionSegment}{packageNameSegment}");
        }

        private Uri ConvertStatisticsUri(Uri statisticsUri)
        {
            if (!UseDynamicUpdateUri)
                return statisticsUri;
            if (statisticsUri == null)
                throw new ArgumentNullException(nameof(statisticsUri));

            // The segment of the correct update php file URI should include: "/", "*.php".
            if (statisticsUri.Segments.Length < 2)
                throw new ArgumentException($@"""{statisticsUri}"" is not a valid statistics file URI.",
                    nameof(statisticsUri));

            var phpFileName = statisticsUri.Segments.Last();
            var baseUri = UpdateConfigurationFileUri.GetLeftPart(UriPartial.Authority);
            var path = string.Join(string.Empty, UpdateConfigurationFileUri.Segments, 0,
                UpdateConfigurationFileUri.Segments.Length - 1);

            return new Uri($"{baseUri}{path}{phpFileName}");
        }

        /// <summary>
        ///     Deletes the downloaded update packages.
        /// </summary>
        public void DeletePackages()
        {
            foreach (var filePathItem in _packageFilePaths.Where(item => File.Exists(item.Value)))
                File.Delete(filePathItem.Value);
        }

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
                var req = WebRequestWrapper.Create(packageUri);
                req.Method = "HEAD";
                if (HttpAuthenticationCredentials != null)
                    req.Credentials = HttpAuthenticationCredentials;
                using (var resp = req.GetResponse())
                {
                    if (double.TryParse(resp.Headers.Get("Content-Length"), out var contentLength))
                        return contentLength;
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        private void Initialize()
        {
            try
            {
                var updateDirDirectoryInfo = new DirectoryInfo(_applicationUpdateDirectory);
                if (updateDirDirectoryInfo.Exists)
                    updateDirDirectoryInfo.Empty();
                else
                    updateDirDirectoryInfo.Create();
            }
            catch (Exception ex)
            {
                throw new IOException(string.Format(_lp.MainFolderCreationExceptionText,
                    ex.Message));
            }
        }

        /// <summary>
        ///     Starts the nUpdate UpdateInstaller to unpack the package and start the updating process.
        /// </summary>
        public void InstallPackage()
        {
            var installerDirectory = Path.Combine(Path.GetTempPath(), "nUpdate Installer");
            var dotNetZipPath = Path.Combine(installerDirectory, "DotNetZip.dll");
            var nUpdatePath = Path.Combine(installerDirectory, "nUpdate.dll");
            var uiBasePath = Path.Combine(installerDirectory, "nUpdate.UpdateInstaller.UIBase.dll");
            var jsonNetPath = Path.Combine(installerDirectory, "Newtonsoft.Json.dll");
            var installerFilePath = Path.Combine(installerDirectory, "nUpdate UpdateInstaller.exe");
            var unpackerAppPdbPath = Path.Combine(installerDirectory, "nUpdate UpdateInstaller.pdb");

            if (Directory.Exists(installerDirectory))
                Directory.Delete(installerDirectory, true);
            Directory.CreateDirectory(installerDirectory);

            File.WriteAllBytes(dotNetZipPath, Resources.DotNetZip);
            File.WriteAllBytes(nUpdatePath, Resources.DotNetZip);
            File.WriteAllBytes(uiBasePath, Resources.nUpdate_UpdateInstaller_UIBase);
            File.WriteAllBytes(jsonNetPath, Resources.Newtonsoft_Json);
            File.WriteAllBytes(installerFilePath, Resources.nUpdate_UpdateInstaller);
            File.WriteAllBytes(unpackerAppPdbPath, Resources.nUpdate_UpdateInstaller_pdb);

            string[] args =
            {
                $"\"{string.Join("%", _packageFilePaths.Select(item => item.Value))}\"",
                $"\"{_startupPath}\"",
                $"\"{_executablePath}\"",
                $"\"{_productName}\"",
                _packageOperations == null ? string.Empty : $"\"{Convert.ToBase64String(Encoding.UTF8.GetBytes(Serializer.Serialize(_packageOperations)))}\"",
                $"\"{(UseCustomInstallerUserInterface ? CustomInstallerUiAssemblyPath : string.Empty)}\"",
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
                $"\"{HostApplicationOptions}\"",
                $"\"{_lp.InstallerFileInUseError}\"",
                $"\"{Process.GetCurrentProcess().Id}\""
            };

            var startInfo = new ProcessStartInfo
            {
                FileName = installerFilePath,
                Arguments = string.Join("|", args),
                UseShellExecute = true,
            };

            if (RunInstallerAsAdmin)
                startInfo.Verb = "runas";

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

            if (HostApplicationOptions != HostApplicationOptions.None)
                TerminateApplication();
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
        ///     Returns a value indicating whether the signature of each package is valid, or not. If a package contains an invalid
        ///     signature, it will be deleted.
        /// </summary>
        /// <returns>Returns <c>true</c> if the package is valid; otherwise <c>false</c>.</returns>
        /// <exception cref="FileNotFoundException">The update package to check could not be found.</exception>
        /// <exception cref="ArgumentException">The signature of the update package is invalid.</exception>
        public bool ValidatePackages()
        {
            bool Validate(KeyValuePair<UpdateVersion, string> filePathItem)
            {
                if (!File.Exists(filePathItem.Value))
                    throw new FileNotFoundException(string.Format(_lp.PackageFileNotFoundExceptionText,
                        filePathItem.Key.FullText));

                var configuration =
                    PackageConfigurations.First(config => config.LiteralVersion == filePathItem.Key.ToString());
                if (configuration.Signature == null || configuration.Signature.Length <= 0)
                    throw new ArgumentException($"Signature of version \"{configuration}\" is null or empty.");

                using (var stream = File.Open(filePathItem.Value, FileMode.Open))
                {
                    RsaManager rsa;

                    try
                    {
                        rsa = new RsaManager(PublicKey);
                    }
                    catch
                    {
                        return false;
                    }

                    return rsa.VerifyData(stream, Convert.FromBase64String(configuration.Signature));
                }
            }

            if (_packageFilePaths.All(Validate))
                return true;

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
    }
}