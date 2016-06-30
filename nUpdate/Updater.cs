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
using nUpdate.Exceptions;
using nUpdate.Localization;
using nUpdate.Operations;
using nUpdate.Properties;
using Newtonsoft.Json;

namespace nUpdate
{
    /// <summary>
    ///     Provides the core functionality to update .NET-applications.
    /// </summary>
    public sealed class Updater
    {
        private readonly string _applicationUpdateDirectory = Path.Combine(Path.GetTempPath(), "nUpdate",
            Application.ProductName);
        private CultureInfo _languageCulture = new CultureInfo("en");

        private readonly Dictionary<UpdateVersion, string> _packageFilePaths = new Dictionary<UpdateVersion, string>();
        private readonly Dictionary<UpdateVersion, IEnumerable<Operation>> _packageOperations =
            new Dictionary<UpdateVersion, IEnumerable<Operation>>();
        private Dictionary<UpdateVersion, List<UpdateRequirement>> _unfulfilledRequirements =
            new Dictionary<UpdateVersion, List<UpdateRequirement>>();
        private LocalizationProperties _lp;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Updater" /> class.
        /// </summary>
        /// <param name="updateDirectoryUri">The <see cref="Uri" /> of the directory on the server that contains all relating update files.</param>
        /// <param name="publicKey">The public key used to verify the signature of the update packages.</param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The version string couldn't be loaded because the <see cref="nUpdateVersionAttribute"/> isn't implemented in the executing assembly.
        /// </exception>
        /// <remarks>
        ///     The public key can be found in the overview of your project when you're opening it inside nUpdate Administration.
        /// </remarks>
        public Updater(Uri updateDirectoryUri, string publicKey)
        {
            if (updateDirectoryUri == null)
                throw new ArgumentNullException(nameof(updateDirectoryUri));
            UpdateDirectoryUri = updateDirectoryUri;
            
            if (string.IsNullOrEmpty(publicKey))
                throw new ArgumentException(nameof(publicKey));
            PublicKey = publicKey;

            var projectAssembly = Assembly.GetCallingAssembly();
            var nUpateVersionAttribute =
                projectAssembly.GetCustomAttributes(false).OfType<nUpdateVersionAttribute>().SingleOrDefault();
            if (nUpateVersionAttribute == null)
                throw new ArgumentException(
                    "The version string couldn't be loaded because the nUpdateVersionAttribute isn't implemented in the executing assembly.");

            CurrentVersion = new UpdateVersion(nUpateVersionAttribute.VersionString);
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            _lp = LocalizationHelper.GetLocalizationProperties(new CultureInfo("en"), LocalizationFilePaths);

            if (Directory.Exists(_applicationUpdateDirectory))
                return;

            try
            {
                Directory.CreateDirectory(_applicationUpdateDirectory);
            }
            catch (Exception ex)
            {
                throw new IOException(string.Format(_lp.MainFolderCreationExceptionText,
                    ex.Message));
            }
        }

        /// <summary>
        ///     Gets the <see cref="Uri" /> of the directory on the server that contains all relating update files.
        /// </summary>
        public Uri UpdateDirectoryUri { get; }

        /// <summary>
        ///     Gets the public key used to verify the signature of the update packages.
        /// </summary>
        public string PublicKey { get; }

        /// <summary>
        ///     Gets the version of the current application.
        /// </summary>
        public UpdateVersion CurrentVersion { get; }

        /// <summary>
        ///     Gets or sets the <see cref="CultureInfo"/> of the language to use.
        /// </summary>
        /// <remarks>In order to use custom languages, you need to download the localization file template from <see href="http://www.nupdate.net/langtemplate.json" /> and modify it.</remarks>
        public CultureInfo LanguageCulture
        {
            get { return _languageCulture; }
            set
            {
                if (!LocalizationHelper.IsIntegratedCulture(value, LocalizationFilePaths) &&
                    !LocalizationFilePaths.ContainsKey(_languageCulture))
                    throw new ArgumentException(
                        "The localization file of the culture set does not exist.");
                _lp = LocalizationHelper.GetLocalizationProperties(value, LocalizationFilePaths);
                _languageCulture = value;
            }
        }

        /// <summary>
        ///     Gets or sets the paths of the localization files accessed by their relating <see cref="CultureInfo"/>s.
        /// </summary>
        public Dictionary<CultureInfo, string> LocalizationFilePaths { get; set; } = new Dictionary<CultureInfo, string>();

        /// <summary>
        ///     Gets or sets the path of the assembly that contains the custom user interface data for the nUpdate UpdateInstaller.
        /// </summary>
        public string CustomInstallerUIAssemblyPath { get; set; }
        
        /// <summary>
        ///     Gets or sets a value indicating whether the user should be able to update to Alpha-versions, or not.
        /// </summary>
        public bool IncludeAlpha { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the user should be able to update to Beta-versions, or not.
        /// </summary>
        public bool IncludeBeta { get; set; }

        /// <summary>
        ///     Gets the filtered <see cref="UpdatePackage"/> collection that should be downloaded and installed after the update search has been finished.
        /// </summary>
        public IEnumerable<UpdatePackage> FilteredUpdatePackageCollection { get; internal set; } =
            Enumerable.Empty<UpdatePackage>();

        /// <summary>
        ///     Gets or sets a value indicating whether the current PC should be included into the statistics, or not.
        /// </summary>
        public bool IncludeIntoStatistics { get; set; } = true;

        /// <summary>
        ///     Gets or sets a value indicating whether the host application should be closed as soon as the nUpdate UpdateInstaller is started, or not.
        /// </summary>
        public bool CloseHostApplication { get; set; } = true;

        /// <summary>
        ///     Gets the total size of all update packages.
        /// </summary>
        public double TotalSize { get; private set; }

        /// <summary>
        ///     Gets or sets the <see cref="WebProxy"/> that should be used.
        /// </summary>
        public WebProxy Proxy { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="UpdateArgument"/>s that should be handled over to the application by the nUpdate UpdateInstaller in order to determine whether the installation process has been successful, or not.
        /// </summary>
        public List<UpdateArgument> Arguments { get; set; } = new List<UpdateArgument>();

        /// <summary>
        ///     Searches for updates.
        /// </summary>
        /// <returns>Returns <c>true</c> if updates were found, otherwise, <c>false</c>.</returns>
        /// <exception cref="SizeCalculationException">The calculation of the size of the available updates has failed.</exception>
        /// <exception cref="OperationCanceledException">The update search was canceled.</exception>
        public async Task<bool> SearchForUpdatesTask(CancellationToken cancellationToken)
        {
            CleanupData();
            if (!WebConnection.IsAvailable())
                return false;

            // Check for SSL and ignore it
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            IEnumerable<UpdatePackage> configuration;

            try
            {
                configuration =
                    await UpdatePackage.GetRemotePackageData(new Uri(UpdateDirectoryUri, "updates.json"), Proxy);
            }
            catch (JsonReaderException ex)
            {
                throw new Exception(_lp.InvalidJsonExceptionText, ex);
            }

            var result = new UpdateResult(configuration, CurrentVersion,
                IncludeAlpha, IncludeBeta);

            _unfulfilledRequirements = result.UnfulfilledRequirements;
            if (!result.UpdatesFound)
                return false;

            FilteredUpdatePackageCollection = result.NewestPackages;
            double updatePackageSize = 0;
            foreach (var updateConfiguration in FilteredUpdatePackageCollection)
            {
                var newPackageSize = SizeHelper.GetRemoteFileSize(updateConfiguration.UpdatePackageUri);
                if (newPackageSize == null)
                    throw new SizeCalculationException(_lp.PackageSizeCalculationExceptionText);

                updatePackageSize += newPackageSize.Value;
                _packageOperations.Add(new UpdateVersion(updateConfiguration.LiteralVersion),
                    updateConfiguration.Operations);
            }

            if (cancellationToken.IsCancellationRequested) // Only for async calls
            {
                CleanupData();
                throw new OperationCanceledException(cancellationToken);
            }

            TotalSize = updatePackageSize;
            return true;
        }

        /// <summary>
        ///     Downloads the available update packages from the server.
        /// </summary>
        /// <exception cref="WebException">The download process has failed because of an <see cref="WebException" />.</exception>
        /// <exception cref="ArgumentException">The URI of the update package is null.</exception>
        /// <exception cref="IOException">The creation of the directory, where the update packages should be saved in, failed.</exception>
        /// <exception cref="IOException">An exception occured while writing to the file.</exception>
        /// <exception cref="OperationCanceledException">The download was canceled.</exception>
        public async Task DownloadUpdatesTask(CancellationToken cancellationToken, IProgress<UpdateProgressData> progress)
        {
            long received = 0;
            foreach (var updateConfiguration in FilteredUpdatePackageCollection)
            {
                WebResponse webResponse;
                    var webRequest = WebRequest.Create(updateConfiguration.UpdatePackageUri);
                using (webResponse = await webRequest.GetResponseAsync())
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

                            int size = await input.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                            while (size > 0)
                            {
                                if (cancellationToken.IsCancellationRequested)
                                    // Only for async calls
                                {
                                    throw new OperationCanceledException(cancellationToken);
                                }

                                await fileStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
                                received += size;
                                progress?.Report(new UpdateProgressData(received,
                                    (long) TotalSize, (float) (received/TotalSize)*100));
                                size = await input.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                            }

                            if (!updateConfiguration.UseStatistics || !IncludeIntoStatistics)
                                continue;

                            try
                            {
                                string response =
                                    await new WebClient().DownloadStringTaskAsync(
                                        $"{updateConfiguration.UpdatePhpFileUri}?versionid={updateConfiguration.VersionId}&os={SystemInformation.OperatingSystemName}");
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
        }
        
        /// <summary>
        ///     Determines whether all RSA-signatures are valid, or not. If not, all packages will be deleted.
        /// </summary>
        /// <returns><c>true</c> if all update packages are valid, otherwise <c>false</c>.</returns>
        /// <exception cref="FileNotFoundException">The current update package to check could not be found.</exception>
        /// <exception cref="ArgumentException">The signature of the current update package is null or empty.</exception>
        public bool ValidateSignatures()
        {
            foreach (var filePathItem in _packageFilePaths)
            {
                if (!File.Exists(filePathItem.Value))
                    throw new FileNotFoundException(string.Format(_lp.PackageFileNotFoundExceptionText,
                        filePathItem.Key.Description));

                var configuration =
                    FilteredUpdatePackageCollection.First(config => config.LiteralVersion == filePathItem.Key.ToString());
                if (configuration.Signature == null || configuration.Signature.Length <= 0)
                    throw new ArgumentException($"Signature of version \"{configuration}\" is null or empty."); // TODO: Localize

                // TODO: Check if packages will be disposed properly
                using (var stream = File.Open(filePathItem.Value, FileMode.Open))
                {
                    try
                    {
                        var rsa = new RsaManager(PublicKey);
                        if (rsa.VerifyData(stream, Convert.FromBase64String(configuration.Signature)))
                            continue;
                    }
                    catch
                    {
                        // Don't throw an exception. Let the method clean up all the packages before we return 'false'.
                        CleanupData();
                    }

                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///     Starts the nUpdate UpdateInstaller to unpack the packages and starts the updating process.
        /// </summary>
        /// <exception cref="ApplicationTerminateException">The application is being terminated. To implement custom actions, catch this exception, execute your action(s) and rethrow it.</exception>
        public void InstallUpdates()
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

            var installerUiAssemblyPath = !string.IsNullOrEmpty(CustomInstallerUIAssemblyPath)
                ? $"\"{CustomInstallerUIAssemblyPath}\""
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

                CleanupData();
                return;
            }

            if (!CloseHostApplication)
                return;

            throw new ApplicationTerminateException();
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (DomainHelper.EnumerateAppDomains().Count() > 1)
                throw new InvalidOperationException(
                    "There is more than one AppDomain active. The application can not be terminated."); // TODO: Localize

            if (((Exception) e.ExceptionObject).GetType() == typeof (ApplicationTerminateException))
                Process.GetCurrentProcess().Kill();
        }

        private void CleanupData()
        {
            // Delete the recently downloaded data
            foreach (var filePathItem in _packageFilePaths.Where(item => File.Exists(item.Value)))
            {
                try
                {
                    File.Delete(filePathItem.Value);
                }
                catch (Exception ex)
                {
                    throw new Exception($"The package of version {filePathItem.Key.Description} could not be deleted: {ex.Message}"); // TODO: Localize
                }
            }

            _packageOperations.Clear();
            _packageFilePaths.Clear();
        }
    }
}