// Author: Dominic Beger (Trade/ProgTrade) 2016

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
        private LocalizationProperties _lp;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Updater" /> class.
        /// </summary>
        /// <param name="updateDirectoryUri">
        ///     The <see cref="Uri" /> of the directory on the server that contains all relating
        ///     update files.
        /// </param>
        /// <param name="publicKey">The public key used to verify the signature of the update packages.</param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The version string couldn't be loaded because the <see cref="nUpdateVersionAttribute" /> isn't implemented in the
        ///     executing assembly.
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
            CurrentVersion = nUpateVersionAttribute == null
                ? new UpdateVersion(Application.ProductVersion)
                : new UpdateVersion(nUpateVersionAttribute.VersionString);

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
        ///     Gets or sets the <see cref="CultureInfo" /> of the language to use.
        /// </summary>
        /// <remarks>
        ///     In order to use custom languages, you need to download the localization file template from
        ///     <see href="https://www.nupdate.net/langtemplate.json" /> and modify it.
        /// </remarks>
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
        ///     Gets or sets the paths of the localization files accessed by their relating <see cref="CultureInfo" />s.
        /// </summary>
        public Dictionary<CultureInfo, string> LocalizationFilePaths { get; set; } =
            new Dictionary<CultureInfo, string>();

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
        ///     Gets or sets a value indicating whether the current client should be included into the statistics, or not.
        /// </summary>
        public bool CaptureStatistics { get; set; } = true;

        /// <summary>
        ///     Gets or sets a value indicating whether the host application should be closed as soon as the nUpdate
        ///     UpdateInstaller is started, or not.
        /// </summary>
        public bool CloseHostApplication { get; set; } = true;

        /// <summary>
        ///     Gets or sets the <see cref="WebProxy" /> that should be used.
        /// </summary>
        public WebProxy Proxy { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="UpdateArgument" />s that should be handled over to the application by the nUpdate
        ///     UpdateInstaller in order to determine whether the installation process has been successful, or not.
        /// </summary>
        public List<UpdateArgument> Arguments { get; set; } = new List<UpdateArgument>();

        /// <summary>
        ///     Searches for updates.
        /// </summary>
        /// <returns>Returns a <c>UpdateResult</c> object that contains the result data of the update search.</returns>
        /// <exception cref="OperationCanceledException">The update search was canceled.</exception>
        public async Task<UpdateResult> SearchForUpdates(CancellationToken cancellationToken)
        {
            if (!WebConnection.IsAvailable())
                return UpdateResult.Empty();

            // Check for SSL and ignore it
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            IEnumerable<UpdatePackage> packageData;

            try
            {
                packageData =
                    await UpdatePackage.GetRemotePackageData(new Uri(UpdateDirectoryUri, "updates.json"), Proxy);
            }
            catch (JsonReaderException ex)
            {
                throw new Exception(_lp.InvalidJsonExceptionText, ex);
            }

            cancellationToken.ThrowIfCancellationRequested();
            return new UpdateResult(packageData, CurrentVersion,
                IncludeAlpha, IncludeBeta);
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
            double totalSize = await UpdateHelper.GetTotalPackageSize(updatePackages);
            await updatePackages.ForEachAsync(async p =>
            {
                await Downloader.DownloadFile(p.UpdatePackageUri, Path.Combine(_applicationUpdateDirectory,
                    $"{p.LiteralVersion}.zip"), (long) totalSize, cancellationToken, progress);

                if (!p.UseStatistics || !CaptureStatistics)
                    return;

                try
                {
                    string response =
                        await new WebClient().DownloadStringTaskAsync(
                            $"{p.UpdatePhpFileUri}?versionid={p.VersionId}&os={SystemInformation.OperatingSystemName}");
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
            });
        }

        /// <summary>
        ///     Determines whether the specified update package is valid, or not.
        /// </summary>
        /// <returns>A <see cref="ValidationResult" /> containing information about the validation of all packages.</returns>
        /// <exception cref="FileNotFoundException">One of the specified update packages could not be found.</exception>
        /// <exception cref="ArgumentException">The signature of an update package is <c>null</c> or empty.</exception>
        /// <seealso cref="RemoveLocalPackage" />
        /// <seealso cref="ValidateUpdates" />
        public Task<bool> ValidateUpdate(UpdatePackage package)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            return TaskEx.Run(() =>
            {
                if (package.Signature == null || package.Signature.Length <= 0)
                    throw new ArgumentException(
                        $"The signature of version \"{package.LiteralVersion}\" is empty and thus invalid.");
                // TODO: Localize

                string packagePath = GetLocalPackagePath(package);
                if (!File.Exists(packagePath))
                    throw new FileNotFoundException(string.Format(_lp.PackageFileNotFoundExceptionText,
                        package.LiteralVersion.ToUpdateVersion().Description));

                // TODO: Check if packages will be disposed properly
                using (var stream = File.Open(packagePath, FileMode.Open))
                {
                    try
                    {
                        using (var rsa = new RsaManager(PublicKey))
                            return rsa.VerifyData(stream, Convert.FromBase64String(package.Signature));
                    }
                    catch
                    {
                        // Don't throw an exception.
                        return false;
                    }
                }
            });
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
            var validationResult = new ValidationResult(updatePackages.Count());
            await updatePackages.ForEachAsync(async p =>
            {
                if (!await ValidateUpdate(p))
                    validationResult.InvalidPackages.Add(p);
            });
            return validationResult;
        }

        public void RemoveLocalPackage(UpdatePackage package)
        {
            File.Delete(GetLocalPackagePath(package));
        }

        public void RemoveLocalPackages(IEnumerable<UpdatePackage> packages)
        {
            foreach (var package in packages)
                RemoveLocalPackage(package);
        }

        /// <summary>
        ///     Starts the nUpdate UpdateInstaller to unpack the packages and starts the updating process.
        /// </summary>
        /// <exception cref="ApplicationTerminateException">
        ///     The application is being terminated. To implement custom actions, catch
        ///     this exception, execute your action(s) and rethrow it.
        /// </exception>
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
                updatePackages.ToDictionary<UpdatePackage, IUpdateVersion, IEnumerable<Operation>>(
                    package => package.LiteralVersion.ToUpdateVersion(), package => package.Operations);

            var installerUiAssemblyPath = !string.IsNullOrEmpty(CustomInstallerUIAssemblyPath)
                ? $"\"{CustomInstallerUIAssemblyPath}\""
                : string.Empty;
            string[] args =
            {
                $"\"{string.Join("%", updatePackages.Select(GetLocalPackagePath))}\"",
                $"\"{Application.StartupPath}\"",
                $"\"{Application.ExecutablePath}\"",
                $"\"{Application.ProductName}\"",
                $"\"{Convert.ToBase64String(Encoding.UTF8.GetBytes(Serializer.Serialize(packageOperations)))}\"",
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
                if (ex.NativeErrorCode != 1223)
                    throw;
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
                    "There is more than one AppDomain active. The application cannot be terminated."); // TODO: Localize

            if (((Exception) e.ExceptionObject).GetType() == typeof (ApplicationTerminateException))
                Environment.Exit(0);
        }

        private string GetLocalPackagePath(UpdatePackage package)
        {
            return Path.Combine(_applicationUpdateDirectory,
                $"{package.LiteralVersion}.zip");
        }
    }
}