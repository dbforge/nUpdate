// HttpUpdateProvider.cs, 20.06.2019
// Copyright (C) Dominic Beger 20.06.2019

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using nUpdate.Properties;

namespace nUpdate
{
    /// <summary>
    ///     Provides functionality to update .NET-applications with a server accessed through HTTP(S).
    /// </summary>
    public class HttpUpdateProvider : IUpdateProvider
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string _applicationUpdateDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "nUpdate",
                "Updates", ApplicationParameters.ProductName);

        private readonly CancellationTokenSource _downloadCancellationTokenSource = new CancellationTokenSource();

        private readonly CancellationTokenSource _updateCheckCancellationTokenSource = new CancellationTokenSource();

        private UpdateResult _updateResult;

        public HttpUpdateProvider(Uri packageFeedUri, string publicKey, string applicationVersion) : this(
            packageFeedUri, publicKey,
            new SemanticVersion(applicationVersion))
        {
        }

        public HttpUpdateProvider(Uri packageFeedFileUri, string publicKey, SemanticVersion applicationVersion)
        {
            PackageFeedUri = packageFeedFileUri ?? throw new ArgumentNullException(nameof(packageFeedFileUri));
            ApplicationVersion = applicationVersion ?? throw new ArgumentNullException(nameof(applicationVersion));

            if (string.IsNullOrEmpty(publicKey))
                throw new ArgumentException(nameof(publicKey));
            PublicKey = publicKey;

            CreateAppUpdateDirectory();
        }

        /// <summary>
        ///     Gets or sets the host application options during the update installation.
        /// </summary>
        public static HostApplicationOptions HostApplicationOptions { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether pre-releases should be included into the update check.
        /// </summary>
        public bool IncludePreRelease { get; set; }

        /// <summary>
        ///     Gets or sets the public key used for verifying the update packages.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="Uri" /> of the package definition file.
        /// </summary>
        public Uri PackageFeedUri { get; set; }

        /// <summary>
        ///     Gets the version of the current application.
        /// </summary>
        public SemanticVersion ApplicationVersion { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="CultureInfo" /> of the language to use.
        /// </summary>
        public CultureInfo LanguageCulture { get; set; } = CultureInfo.CurrentUICulture;

        public async Task<UpdateResult> BeginUpdateCheck()
        {
            // Empty result
            var result = new UpdateResult();
            var
                packages = await DefaultUpdatePackage.GetPackageEnumerable(PackageFeedUri, null);

            Logger.Info("Initializing the update result for this cbeck.");
            await result.Initialize(packages, ApplicationVersion, IncludePreRelease,
                _updateCheckCancellationTokenSource.Token);
            _updateResult = result;
            return result;
        }

        public async Task DownloadUpdates(IProgress<UpdateProgressData> progress)
        {
            if (_updateResult == null)
                throw new InvalidOperationException(
                    "Invalid update result. There must be a check for updates before being able to download them.");
            var updatePackages = _updateResult.Packages.ToArray();

            Logger.Info("Determining the total size of all packages.");
            var totalSize = await Utility.GetTotalPackageSize(updatePackages);
            await updatePackages.ForEachAsync(async p =>
            {
                if (Utility.IsHttpUri(p.PackageUri))
                {
                    // This package is located on a server.
                    Logger.Info($"Downloading package of version {p.Version}.");
                    await DownloadManager.DownloadFile(p.PackageUri, GetLocalPackagePath(p), (long) totalSize,
                        _downloadCancellationTokenSource.Token, progress);
                }
                else
                {
                    // This package is located on a local drive.
                    Logger.Info($"Copying package of version {p.Version}.");
                    File.Copy(p.PackageUri.ToString(), GetLocalPackagePath(p));
                }
            });
        }

        public async Task InstallUpdates(Action terminateAction = null)
        {
            var updatePackages = _updateResult.Packages.ToArray();
            await updatePackages.ForEachAsync(p =>
            {
                return Task.Run(() =>
                {
                    ZipFile.ExtractToDirectory(GetLocalPackagePath(p),
                        Path.Combine(_applicationUpdateDirectory, p.Guid.ToString()));
                });
            });
        }

        public void CancelUpdateCheck()
        {
            _updateCheckCancellationTokenSource.Cancel();
        }

        public void CancelDownload()
        {
            _downloadCancellationTokenSource.Cancel();
        }

        /// <summary>
        ///     Determines whether all available packages are valid, or not.
        /// </summary>
        /// <returns>A <see cref="VerificationResult" /> containing information about the validation of all packages.</returns>
        /// <exception cref="FileNotFoundException">One of the specified update packages could not be found.</exception>
        /// <exception cref="ArgumentException">The signature of an update package is <c>null</c> or empty.</exception>
        /// <seealso cref="RemoveLocalPackage" />
        /// <seealso cref="VerifyUpdate" />
        public Task<VerificationResult> VerifyUpdates()
        {
            return VerifyUpdates(_updateResult.Packages);
        }

        private string GetLocalPackagePath(DefaultUpdatePackage package)
        {
            return Path.Combine(_applicationUpdateDirectory,
                $"{package.Guid}.zip");
        }

        /// <summary>
        ///     Removes the specified update package.
        /// </summary>
        /// <param name="package">The update package.</param>
        /// <seealso cref="RemoveLocalPackages()" />
        /// <seealso cref="RemoveLocalPackages(System.Collections.Generic.IEnumerable{nUpdate.DefaultUpdatePackage})" />
        public void RemoveLocalPackage(DefaultUpdatePackage package)
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
        public void RemoveLocalPackages(IEnumerable<DefaultUpdatePackage> packages)
        {
            if (packages == null)
                throw new ArgumentNullException(nameof(packages));
            foreach (var package in packages)
                RemoveLocalPackage(package);
        }

        /// <summary>
        ///     Removes all downloaded update packages.
        /// </summary>
        /// <seealso cref="RemoveLocalPackage" />
        /// <seealso cref="RemoveLocalPackages(IEnumerable{DefaultUpdatePackage})" />
        public void RemoveLocalPackages()
        {
            RemoveLocalPackages(_updateResult.Packages);
        }

        /// <summary>
        ///     Creates the application's update directory where the downloaded packages and other files are stored.
        /// </summary>
        private void CreateAppUpdateDirectory()
        {
            Logger.Info("Checking existence of the application's update directory.");
            if (Directory.Exists(_applicationUpdateDirectory))
                return;

            Logger.Info("Creating the application's update directory.");
            try
            {
                Directory.CreateDirectory(_applicationUpdateDirectory);
            }
            catch (Exception ex)
            {
                Logger.Error(ex,
                    "The application's update directory could not be created as an exception occured.");
                throw new IOException(string.Format(strings.MainFolderCreationExceptionText,
                    ex.Message));
            }
        }

        /// <summary>
        ///     Determines whether the specified update package is valid, or not.
        /// </summary>
        /// <returns>A <see cref="VerificationResult" /> containing information about the validation of all packages.</returns>
        /// <exception cref="FileNotFoundException">One of the specified update packages could not be found.</exception>
        /// <exception cref="ArgumentException">The signature of an update package is <c>null</c> or empty.</exception>
        /// <seealso cref="RemoveLocalPackage" />
        /// <seealso cref="VerifyUpdates()" />
        /// <seealso cref="VerifyUpdates(IEnumerable{DefaultUpdatePackage})" />
        public Task<bool> VerifyUpdate(DefaultUpdatePackage package)
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
                    throw new FileNotFoundException(string.Format(strings.PackageFileNotFoundExceptionText,
                        package.Version));

                // TODO: Check if packages will be disposed properly
                using (var stream = File.Open(packagePath, FileMode.Open))
                {
                    try
                    {
                        Logger.Info($"Verifying the signature for the package of version {package.Version}.");
                        using (var rsa = new RsaManager(PublicKey))
                        {
                            return rsa.VerifyData(stream, Convert.FromBase64String(package.Signature));
                        }
                    }
                    catch
                    {
                        // Don't throw an exception.
                        Logger.Warn($"Invalid signature found for the package of version {package.Version}.");
                        return false;
                    }
                }
            });
        }

        /// <summary>
        ///     Determines whether all specified update packages are valid, or not.
        /// </summary>
        /// <returns>A <see cref="VerificationResult" /> containing information about the validation of all packages.</returns>
        /// <exception cref="FileNotFoundException">One of the specified update packages could not be found.</exception>
        /// <exception cref="ArgumentException">The signature of an update package is <c>null</c> or empty.</exception>
        /// <seealso cref="RemoveLocalPackage" />
        /// <seealso cref="VerifyUpdate" />
        public async Task<VerificationResult> VerifyUpdates(IEnumerable<DefaultUpdatePackage> packages)
        {
            if (packages == null)
                throw new ArgumentNullException(nameof(packages));

            var updatePackages = packages.ToArray();
            if (updatePackages.Length == 0)
                return default;

            var validationResult = new VerificationResult(updatePackages.Length);
            await updatePackages.ForEachAsync(async p =>
            {
                if (!await VerifyUpdate(p))
                    validationResult.InvalidPackages.Add(p);
            });
            return validationResult;
        }
    }
}