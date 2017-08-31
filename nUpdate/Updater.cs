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
using nUpdate.Exceptions;
using nUpdate.Localization;
using nUpdate.Operations;
using nUpdate.Properties;
using Newtonsoft.Json;
using Splat;

namespace nUpdate
{
    /// <summary>
    ///     Provides the core functionality to update .NET-applications.
    /// </summary>
    public class Updater : IEnableLogger
    {
        private readonly string _applicationUpdateDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "nUpdate",
                "Updates", ApplicationParameters.ProductName);

        private CultureInfo _languageCulture = new CultureInfo("en");
        private LocalizationProperties _lp;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Updater" /> class.
        /// </summary>
        /// <param name="updateDirectoryUri">
        ///     The <see cref="Uri" /> of the directory on the server that contains all relating
        ///     update files.
        /// </param>
        /// <param name="publicKey">The public key used to verify the signature of the update packages.</param>
        /// <param name="applicationChannel">The current channel of the application.</param>
        /// <param name="lowestSearchChannel">
        ///     The lowest update channel that should be included into the update search. nUpdate
        ///     will check for updates inside of this channel and higher ones.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The version string couldn't be loaded because the <see cref="nUpdateVersionAttribute" /> isn't implemented in the
        ///     executing assembly.
        /// </exception>
        /// <remarks>
        ///     The public key can be found in the overview of your project when you're opening it inside nUpdate Administration.
        /// </remarks>
        public Updater(Uri updateDirectoryUri, string publicKey, string applicationChannel, string lowestSearchChannel)
        {
            if (updateDirectoryUri == null || !updateDirectoryUri.IsWellFormedOriginalString())
                throw new ArgumentException(nameof(updateDirectoryUri));
            UpdateDirectoryUri = updateDirectoryUri;

            if (string.IsNullOrEmpty(publicKey))
                throw new ArgumentException(nameof(publicKey));
            PublicKey = publicKey;

            if (string.IsNullOrWhiteSpace(applicationChannel))
                throw new ArgumentException(nameof(applicationChannel));
            ApplicationChannel = applicationChannel;

            if (string.IsNullOrWhiteSpace(lowestSearchChannel))
                throw new ArgumentException(nameof(lowestSearchChannel));
            LowestSearchChannel = lowestSearchChannel;

            var projectAssembly = Assembly.GetCallingAssembly();
            var nUpateVersionAttribute =
                projectAssembly.GetCustomAttributes(false).OfType<nUpdateVersionAttribute>().SingleOrDefault();
            ApplicationVersion = nUpateVersionAttribute == null
                ? ApplicationParameters.ProductVersion
                : new Version(nUpateVersionAttribute.VersionString);

            _lp = LocalizationHelper.GetLocalizationProperties(new CultureInfo("en"), LocalizationFilePaths);

            CreateAppUpdateDirectory();
        }

        /// <summary>
        ///     Gets the current channel's name.
        /// </summary>
        public string ApplicationChannel { get; }

        /// <summary>
        ///     Gets the version of the current application.
        /// </summary>
        public Version ApplicationVersion { get; }

        /// <summary>
        ///     Gets or sets the <see cref="UpdateArgument" />s that should be handled over to the application by the nUpdate
        ///     UpdateInstaller in order to determine whether the installation process has been successful, or not.
        /// </summary>
        public List<UpdateArgument> Arguments { get; set; } = new List<UpdateArgument>();

        /// <summary>
        ///     Gets all new update packages that have been found.
        /// </summary>
        public IEnumerable<UpdatePackage> AvailablePackages { get; private set; } =
            Enumerable.Empty<UpdatePackage>();

        /// <summary>
        ///     Gets or sets a value indicating whether the current client should be included into the statistics, or not.
        /// </summary>
        public bool CaptureStatistics { get; set; } = true;

        /// <summary>
        ///     Gets or sets the host application options during the update installation.
        /// </summary>
        public static HostApplicationOptions HostApplicationOptions { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the user should be able to update to Alpha-versions, or not.
        /// </summary>
        public bool IncludeAlpha { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the user should be able to update to Beta-versions, or not.
        /// </summary>
        public bool IncludeBeta { get; set; }

        /// <summary>
        ///     Gets or sets the path of the assembly that contains the custom user interface data for the nUpdate UpdateInstaller.
        /// </summary>
        public string InstallerUserInterfacePath { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="CultureInfo" /> of the language to use.
        /// </summary>
        /// <remarks>
        ///     In order to use custom languages, you need to download the localization file template from
        ///     <see href="https://www.nupdate.net/langtemplate.json" /> and modify it.
        /// </remarks>
        public CultureInfo LanguageCulture
        {
            get => _languageCulture;
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
        ///     Gets or sets the paths of the localization files accessed by their relating <see cref="CultureInfo" />s.
        /// </summary>
        public Dictionary<CultureInfo, string> LocalizationFilePaths { get; set; } =
            new Dictionary<CultureInfo, string>();

        /// <summary>
        ///     Gets the lowest update channel that should be used for the update search.
        /// </summary>
        public string LowestSearchChannel { get; }

        /// <summary>
        ///     Gets or sets the <see cref="WebProxy" /> that should be used.
        /// </summary>
        public WebProxy Proxy { get; set; }

        /// <summary>
        ///     Gets the public key used to verify the signature of the update packages.
        /// </summary>
        public string PublicKey { get; }

        /// <summary>
        ///     Gets the total size of all found update packages.
        /// </summary>
        public double TotalSize { get; private set; } = double.NaN;

        /// <summary>
        ///     Gets the <see cref="Uri" /> of the directory on the server that contains all relating update files.
        /// </summary>
        public Uri UpdateDirectoryUri { get; }

        /// <summary>
        ///     Prepares and installs the specified update packages.
        /// </summary>
        /// <seealso cref="ApplyUpdates(IEnumerable{UpdatePackage})" />
        public void ApplyUpdates()
        {
            ApplyUpdates(AvailablePackages);
        }

        /// <summary>
        ///     Prepares and installs the specified update packages.
        /// </summary>
        /// <seealso cref="ApplyUpdates()" />
        public void ApplyUpdates(IEnumerable<UpdatePackage> packages)
        {
            if (packages == null)
                throw new ArgumentNullException(nameof(packages));

            var unpackerDirectory = Path.Combine(Path.GetTempPath(), "nUpdate Installer");
            var unpackerZipPath = Path.Combine(unpackerDirectory, "Ionic.Zip.dll");
            var guiInterfacePath = Path.Combine(unpackerDirectory, "nUpdate.UpdateInstaller.Client.GuiInterface.dll");
            var jsonNetPath = Path.Combine(unpackerDirectory, "Newtonsoft.Json.dll");
            var jsonNetPdbPath = Path.Combine(unpackerDirectory, "Newtonsoft.Json.pdb");
            var unpackerAppPath = Path.Combine(unpackerDirectory, "nUpdate UpdateInstaller.exe");
            //var unpackerAppPdbPath = Path.Combine(unpackerDirectory, "nUpdate UpdateInstaller.pdb"); 

            this.Log().Info("Managing the installer files.");
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

            var updatePackages = packages.ToArray();
            var packageOperations =
                updatePackages.ToDictionary<UpdatePackage, Version, IEnumerable<Operation>>(
                    package => package.Version, package => package.Operations);

            var installerUiAssemblyPath = !string.IsNullOrEmpty(InstallerUserInterfacePath)
                ? $"\"{InstallerUserInterfacePath}\""
                : string.Empty;
            string[] args =
            {
                $"\"{string.Join("%", updatePackages.Select(GetLocalPackagePath))}\"",
                $"\"{ApplicationParameters.StartupPath}\"",
                $"\"{ApplicationParameters.ExecutablePath}\"",
                $"\"{ApplicationParameters.ProductName}\"",
                $"\"{Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(packageOperations)))}\"",
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
                $"\"{Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(Arguments)))}\"",
                $"\"{HostApplicationOptions}\"",
                $"\"{_lp.InstallerFileInUseError}\""
            };

            this.Log().Info("Starting nUpdate UpdateInstaller.");
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
                return;
            }

            if (HostApplicationOptions == HostApplicationOptions.LeaveOpen)
                return;

            this.Log().Info("Throwing final exception for terminating the application.");
            TerminateApplication();
        }

        /// <summary>
        ///     Creates the application's update directory where the downloaded packages and other files are stored.
        /// </summary>
        private void CreateAppUpdateDirectory()
        {
            this.Log().Info("Checking existence of the application's update directory.");
            if (Directory.Exists(_applicationUpdateDirectory))
                return;

            this.Log().Info("Creating the application's update directory.");
            try
            {
                Directory.CreateDirectory(_applicationUpdateDirectory);
            }
            catch (Exception ex)
            {
                this.Log().ErrorException(
                    "The application's update directory could not be created as an exception occured.", ex);
                throw new IOException(string.Format(_lp.MainFolderCreationExceptionText,
                    ex.Message));
            }
        }

        /// <summary>
        ///     Downloads all available update packages from the server.
        /// </summary>
        /// <exception cref="WebException">The download process has failed because of an <see cref="WebException" />.</exception>
        /// <exception cref="ArgumentException">The URI of the update package is null.</exception>
        /// <exception cref="IOException">The creation of the directory, where the update packages should be saved in, failed.</exception>
        /// <exception cref="IOException">An exception occured while writing to the file.</exception>
        /// <exception cref="OperationCanceledException">The download was canceled.</exception>
        public async Task DownloadUpdates(CancellationToken cancellationToken, IProgress<UpdateProgressData> progress)
        {
            await DownloadUpdates(AvailablePackages, cancellationToken, progress);
        }

        /// <summary>
        ///     Downloads the specified update packages from the server.
        /// </summary>
        /// <exception cref="WebException">The download process has failed because of an <see cref="WebException" />.</exception>
        /// <exception cref="ArgumentException">The URI of the update package is null.</exception>
        /// <exception cref="IOException">The creation of the directory, where the update packages should be saved in, failed.</exception>
        /// <exception cref="IOException">An exception occured while writing to the file.</exception>
        /// <exception cref="OperationCanceledException">The download was canceled.</exception>
        public async Task DownloadUpdates(IEnumerable<UpdatePackage> packages, CancellationToken cancellationToken,
            IProgress<UpdateProgressData> progress)
        {
            if (packages == null)
                throw new ArgumentNullException(nameof(packages));
            var updatePackages = packages.ToArray();

            this.Log().Info("Determining the total size of all packages.");
            var totalSize = await Utility.GetTotalPackageSize(updatePackages);
            await updatePackages.ForEachAsync(async p =>
            {
                if (Utility.IsHttpUri(p.UpdatePackageUri))
                {
                    // This package is located on a server.
                    this.Log().Info($"Downloading package of version {p.Version}.");
                    await Downloader.DownloadFile(p.UpdatePackageUri, GetLocalPackagePath(p), (long) totalSize,
                        cancellationToken, progress);
                }
                else
                {
                    // This package is located on a local drive.
                    this.Log().Info($"Copying package of version {p.Version}.");
                    File.Copy(p.UpdatePackageUri.ToString(), GetLocalPackagePath(p));
                }

                if (!p.UseStatistics || !CaptureStatistics)
                    return;

                try
                {
                    this.Log().Info($"Adding entry to the statistics for version {p.Version}.");
                    var response =
                        await new WebClient().DownloadStringTaskAsync(
                            $"{p.UpdatePhpFileUri}?versionid={p.VersionId}&os={SystemInformation.OperatingSystemName}");
                    if (!string.IsNullOrEmpty(response))
                        throw new StatisticsException(string.Format(
                            _lp.StatisticsScriptExceptionText, response));
                }
                catch (Exception ex)
                {
                    throw new StatisticsException(ex.Message);
                }
            });
        }

        private string GetLocalPackagePath(UpdatePackage package)
        {
            return Path.Combine(_applicationUpdateDirectory,
                $"{package.Guid}.zip");
        }

        /// <summary>
        ///     Removes the specified update package.
        /// </summary>
        /// <param name="package">The update package.</param>
        /// <seealso cref="RemoveLocalPackages()" />
        /// <seealso cref="RemoveLocalPackages(IEnumerable{UpdatePackage})" />
        public void RemoveLocalPackage(UpdatePackage package)
        {
            var path = GetLocalPackagePath(package);
            if (File.Exists(path))
                File.Delete(path);
        }

        /// <summary>
        ///     Removes the specified update packages.
        /// </summary>
        /// <param name="packages">The update packages.</param>
        /// <seealso cref="RemoveLocalPackage" />
        /// <seealso cref="RemoveLocalPackages()" />
        public void RemoveLocalPackages(IEnumerable<UpdatePackage> packages)
        {
            foreach (var package in packages)
                RemoveLocalPackage(package);
        }

        /// <summary>
        ///     Removes all downloaded update packages.
        /// </summary>
        /// <seealso cref="RemoveLocalPackage" />
        /// <seealso cref="RemoveLocalPackages(IEnumerable{UpdatePackage})" />
        public void RemoveLocalPackages()
        {
            RemoveLocalPackages(AvailablePackages);
        }

        /// <summary>
        ///     Searches for updates.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        ///     Returns a <c>UpdateResult</c> object that contains the result data of the update search.
        /// </returns>
        /// <exception cref="OperationCanceledException">The update search was canceled.</exception>
        public async Task<bool> SearchForUpdates(CancellationToken cancellationToken)
        {
            this.Log().Info("Checking the network connection.");
            if (!WebConnection.IsAvailable())
                return false;

            // Check for SSL and ignore it
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            IEnumerable<UpdateChannel> masterChannel;

            try
            {
                this.Log().Info("Loading the master channel from the server.");
                masterChannel =
                    await UpdateChannel.GetMasterChannel(new Uri(UpdateDirectoryUri, "masterfeed.json"), Proxy);
            }
            catch (JsonReaderException ex)
            {
                this.Log().ErrorException("The received master channel data is no valid JSON data.", ex);
                throw new Exception(_lp.InvalidJsonExceptionText, ex);
            }

            cancellationToken.ThrowIfCancellationRequested();

            this.Log().Info("Initializing the update result for this search.");
            var result = new UpdateResult();
            await result.Initialize(masterChannel, ApplicationVersion,
                ApplicationChannel, LowestSearchChannel);
            AvailablePackages = result.NewPackages;

            this.Log().Info("Determining the total size of all packages.");
            TotalSize = await Utility.GetTotalPackageSize(AvailablePackages);
            return result.UpdatesFound;
        }

        /// <summary>
        ///     Terminates the application.
        /// </summary>
        /// <remarks>Override this method to add custom actions that should be performed.</remarks>
        public virtual void TerminateApplication()
        {
            Environment.Exit(0);
        }

        /// <summary>
        ///     Determines whether the specified update package is valid, or not.
        /// </summary>
        /// <returns>A <see cref="ValidationResult" /> containing information about the validation of all packages.</returns>
        /// <exception cref="FileNotFoundException">One of the specified update packages could not be found.</exception>
        /// <exception cref="ArgumentException">The signature of an update package is <c>null</c> or empty.</exception>
        /// <seealso cref="RemoveLocalPackage" />
        /// <seealso cref="ValidateUpdates()" />
        /// <seealso cref="ValidateUpdates(IEnumerable{UpdatePackage})" />
        public Task<bool> ValidateUpdate(UpdatePackage package)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            return Task.Run(() =>
            {
                if (package.Signature == null || package.Signature.Length <= 0)
                    throw new ArgumentException(
                        $"The signature of version \"{package.Version}\" is empty and thus invalid.");
                // TODO: Localize

                var packagePath = GetLocalPackagePath(package);
                if (!File.Exists(packagePath))
                    throw new FileNotFoundException(string.Format(_lp.PackageFileNotFoundExceptionText,
                        package.Version));

                // TODO: Check if packages will be disposed properly
                using (var stream = File.Open(packagePath, FileMode.Open))
                {
                    try
                    {
                        this.Log().Info($"Verifying the signature for the package of version {package.Version}.");
                        using (var rsa = new RsaManager(PublicKey))
                        {
                            return rsa.VerifyData(stream, Convert.FromBase64String(package.Signature));
                        }
                    }
                    catch
                    {
                        // Don't throw an exception.
                        this.Log().Warn($"Invalid signature found for the package of version {package.Version}.");
                        return false;
                    }
                }
            });
        }

        /// <summary>
        ///     Determines whether all available packages are valid, or not.
        /// </summary>
        /// <returns>A <see cref="ValidationResult" /> containing information about the validation of all packages.</returns>
        /// <exception cref="FileNotFoundException">One of the specified update packages could not be found.</exception>
        /// <exception cref="ArgumentException">The signature of an update package is <c>null</c> or empty.</exception>
        /// <seealso cref="RemoveLocalPackage" />
        /// <seealso cref="ValidateUpdate" />
        public Task<ValidationResult> ValidateUpdates()
        {
            return ValidateUpdates(AvailablePackages);
        }

        /// <summary>
        ///     Determines whether all specified update packages are valid, or not.
        /// </summary>
        /// <returns>A <see cref="ValidationResult" /> containing information about the validation of all packages.</returns>
        /// <exception cref="FileNotFoundException">One of the specified update packages could not be found.</exception>
        /// <exception cref="ArgumentException">The signature of an update package is <c>null</c> or empty.</exception>
        /// <seealso cref="RemoveLocalPackage" />
        /// <seealso cref="ValidateUpdate" />
        public async Task<ValidationResult> ValidateUpdates(IEnumerable<UpdatePackage> packages)
        {
            if (packages == null)
                throw new ArgumentNullException(nameof(packages));

            var updatePackages = packages.ToArray();
            if (updatePackages.Length == 0)
                return default(ValidationResult);

            var validationResult = new ValidationResult(updatePackages.Length);
            await updatePackages.ForEachAsync(async p =>
            {
                if (!await ValidateUpdate(p))
                    validationResult.InvalidPackages.Add(p);
            });
            return validationResult;
        }
    }
}