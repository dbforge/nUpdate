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
using System.Windows.Forms;
using nUpdate.Core;
using nUpdate.Exceptions;
using nUpdate.Internal.Core;
using nUpdate.Internal.Core.Localization;
using nUpdate.Internal.Core.Operations;
using nUpdate.Internal.Properties;

namespace nUpdate.Updating
{
    /// <summary>
    ///     Provides functionality to update .NET-applications.
    /// </summary>
    public partial class UpdateManager : IDisposable

    {
        private readonly string _applicationUpdateDirectory = Path.Combine(Path.GetTempPath(), "nUpdate",
            Application.ProductName);

        private readonly Dictionary<UpdateVersion, string> _packageFilePaths = new Dictionary<UpdateVersion, string>();

        private readonly Dictionary<UpdateVersion, IEnumerable<Operation>> _packageOperations =
            new Dictionary<UpdateVersion, IEnumerable<Operation>>();

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
        /// <remarks>
        ///     The public key can be found in the overview of your project when you're opening it in nUpdate Administration.
        ///     If you have problems inserting the data (or if you want to save time) you can scroll down there and follow the
        ///     steps of the category "Copy data" which will automatically generate the necessray code for you.
        /// </remarks>
        public UpdateManager(Uri updateConfigurationFileUri, string publicKey,
            CultureInfo languageCulture = null, UpdateVersion currentVersion = null)
        {
            UpdateConfigurationFileUri = updateConfigurationFileUri ?? throw new ArgumentNullException(nameof(updateConfigurationFileUri));

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
            {
                CurrentVersion = currentVersion;
            }
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
        ///     Gets or sets the arguments that should be handled over to the application once the update installation has
        ///     completed.
        /// </summary>
        public List<UpdateArgument> Arguments { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the host application should be closed when the nUpdate UpdateInstaller is
        ///     started, or
        ///     not.
        /// </summary>
        public bool CloseHostApplication { get; set; } = true;

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

        /// <summary>
        ///     Gets or sets a value indicating whether the host application should be restarted once the update installation has
        ///     completed, or not.
        /// </summary>
        public bool RestartHostApplication { get; set; } = true;

        /// <summary>
        ///     Gets or sets the timeout that should be used when searching for updates. In milliseconds. 
        ///     By default, this is set to 10000 milliseconds.
        /// </summary>
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

        private void Cleanup()
        {
            _packageFilePaths.Clear();
            _packageOperations.Clear();
        }
        
        private Uri ConvertPackageUri(Uri updatePackageUri)
        {
            if (!UseDynamicUpdateUri)
                return updatePackageUri;
            if (updatePackageUri == null)
                throw new ArgumentNullException(nameof(updatePackageUri));
            // The segment of the correct update package URI should include: "/", "1.0.0.0(version)/", "*.zip".
            if (updatePackageUri.Segments.Length < 3)
                throw new ArgumentException($"{ nameof(updatePackageUri)} is not a valid update package URI.", nameof(updatePackageUri));
            var packageNameSegment = updatePackageUri.Segments.Last();
            var versionSegment = updatePackageUri.Segments[updatePackageUri.Segments.Length - 2];
            var baseUri = UpdateConfigurationFileUri.GetLeftPart(UriPartial.Authority);
            var configLocatedPath = string.Join(string.Empty, UpdateConfigurationFileUri.Segments, 0, UpdateConfigurationFileUri.Segments.Length - 1);
            return new Uri($"{baseUri}{configLocatedPath}{versionSegment}{packageNameSegment}");
        }

        private Uri ConvertStatisticsUri(Uri statisticsUri)
        {
            if (!UseDynamicUpdateUri)
                return statisticsUri;
            if (statisticsUri == null)
                throw new ArgumentNullException(nameof(statisticsUri));
            // The segment of the correct update php file URI should include: "/", "*.php".
            if (statisticsUri.Segments.Length < 2)
                throw new ArgumentException($"{ nameof(statisticsUri)} is not a valid update php file URI.", nameof(statisticsUri));
            var phpFileName = statisticsUri.Segments.Last();
            var baseUri = UpdateConfigurationFileUri.GetLeftPart(UriPartial.Authority);
            var configLocatedPath = string.Join(string.Empty, UpdateConfigurationFileUri.Segments, 0, UpdateConfigurationFileUri.Segments.Length - 1);
            return new Uri($"{baseUri}{configLocatedPath}{phpFileName}");
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
        ///     Finalizes an instance of the <see cref="UpdateManager" /> class.
        /// </summary>
        ~UpdateManager()
        {
            Dispose(true);
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

        /// <summary>
        ///     Starts the nUpdate UpdateInstaller to unpack the package and start the updating process.
        /// </summary>
        public void InstallPackage()
        {
            var installerDirectory = Path.Combine(Path.GetTempPath(), "nUpdate Installer");
            var dotNetZipPath = Path.Combine(installerDirectory, "Ionic.Zip.dll");
            var guiInterfacePath = Path.Combine(installerDirectory, "nUpdate.UpdateInstaller.Client.GuiInterface.dll");
            var jsonNetPath = Path.Combine(installerDirectory, "Newtonsoft.Json.dll");
            var installerFilePath = Path.Combine(installerDirectory, "nUpdate UpdateInstaller.exe");

            if (Directory.Exists(installerDirectory))
                Directory.Delete(installerDirectory, true);
            Directory.CreateDirectory(installerDirectory);

            File.WriteAllBytes(dotNetZipPath, Resources.Ionic_Zip);
            File.WriteAllBytes(guiInterfacePath, Resources.nUpdate_UpdateInstaller_Client_GuiInterface);
            File.WriteAllBytes(jsonNetPath, Resources.Newtonsoft_Json);
            File.WriteAllBytes(installerFilePath, Resources.nUpdate_UpdateInstaller);

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
                FileName = installerFilePath,
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