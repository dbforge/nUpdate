using System.Globalization;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using nUpdate.Exceptions;
using nUpdate.Installer;
using nUpdate.Localization;
using nUpdate.Packaging;
using nUpdate.Platform;
using nUpdate.Security;
using nUpdate.Statistics;

namespace nUpdate.Updating;

/// <summary>
///     Checks for, downloads, verifies and installs updates. Call the steps in order:
///     <see cref="CheckForUpdatesAsync" />, <see cref="DownloadAsync" />, <see cref="VerifyAsync" />, <see cref="StartInstaller" />.
/// </summary>
public sealed class UpdateManager : IDisposable
{
    /// <summary>The folder next to the application executable that holds the built-in installer, one subfolder per runtime identifier.</summary>
    public const string InstallerFolderName = "nUpdate.Installer";

    /// <summary>The file name of the built-in installer, without the <c>.exe</c> it has on Windows.</summary>
    public const string BuiltInInstallerName = "nUpdate.UpdateInstaller.UI.Avalonia";

    /// <summary>The name of the installer icon in the installer's temp folder.</summary>
    internal const string InstallerIconFileName = "installer-icon.png";

    private const string OptionsFileName = "installer-options.json";

    /// <summary>The folder in the temp directory that holds one installer folder per application.</summary>
    private const string InstallerTempFolderName = "nUpdate Installer";

    private readonly UpdateManagerServices _services;
    private readonly IFileSystem _fileSystem;
    private readonly LocalizationProvider _localizationProvider;
    private readonly Dictionary<UpdateVersion, string> _downloadedPackages = [];
    private readonly Dictionary<UpdateVersion, Uri> _packageUris = [];
    private readonly Dictionary<UpdateVersion, PackageFile> _packageFiles = [];
    private readonly Dictionary<CultureInfo, string> _textFiles = [];
    private UpdateFeed? _feed;
    private HttpClient? _httpClient;
    private HttpClient? _ownedHttpClient;
    private CultureInfo _culture = LocalizationProvider.DefaultCulture;
    private string? _installerPath;
    private string? _installerAccentColor;
    private string? _applicationExecutablePath;
    private bool _disposed;

    /// <param name="feedUri">The absolute URI of the project's <c>nupdate.json</c>.</param>
    /// <param name="publicKey">The PEM public key of the project, shown in nUpdate Administration.</param>
    /// <param name="culture">The culture of the texts. Defaults to English.</param>
    /// <param name="currentVersion">The installed version. Defaults to the <see cref="ApplicationVersionAttribute" /> of the entry assembly.</param>
    /// <param name="services">External dependencies; defaults to the production implementations.</param>
    public UpdateManager(Uri feedUri, string publicKey, CultureInfo? culture = null, UpdateVersion? currentVersion = null, UpdateManagerServices? services = null)
    {
        FeedUri = feedUri ?? throw new ArgumentNullException(nameof(feedUri));
        if (!feedUri.IsAbsoluteUri)
            throw new ArgumentException("The feed URI must be absolute.", nameof(feedUri));
        if (string.IsNullOrWhiteSpace(publicKey))
            throw new ArgumentNullException(nameof(publicKey));

        PublicKey = publicKey;
        _services = services ?? new UpdateManagerServices();
        _fileSystem = _services.FileSystem;
        _localizationProvider = new LocalizationProvider(_fileSystem);

        var applicationInfo = _services.ApplicationInfo;
        ApplicationName = applicationInfo.ProductName;
        // A path the process cannot tell (empty, or relative) counts as unknown, so StartInstaller asks for it clearly.
        _applicationExecutablePath = IsAbsolutePath(applicationInfo.ExecutablePath) ? applicationInfo.ExecutablePath : null;
        CurrentVersion = currentVersion ?? ParseDeclaredVersion(applicationInfo.DeclaredVersion);
        Texts = _localizationProvider.Load(LocalizationProvider.DefaultCulture);
        if (culture is not null)
            Culture = culture;
    }

    public Uri FeedUri { get; }

    public string PublicKey { get; }

    public UpdateVersion CurrentVersion { get; }

    /// <summary>The texts of the current <see cref="Culture" />.</summary>
    public UpdateTexts Texts { get; private set; }

    /// <summary>Custom text files per culture. Register them before setting <see cref="Culture" />.</summary>
    public IDictionary<CultureInfo, string> TextFiles => _textFiles;

    /// <summary>
    ///     The culture of the texts. A culture without texts of its own uses the nearest parent that has some (so
    ///     <c>en-US</c> reads the English texts and <c>de-CH</c> the Swiss German ones) and English when nothing matches;
    ///     register a file in <see cref="TextFiles" /> to add a language.
    /// </summary>
    public CultureInfo Culture
    {
        get => _culture;
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            var files = new Dictionary<CultureInfo, string>(_textFiles);
            Texts = _localizationProvider.Load(LocalizationProvider.Resolve(value, files), files);
            _culture = value;
        }
    }

    /// <summary>The product name used for the download folder and shown by the installer.</summary>
    public string ApplicationName { get; set; }

    /// <summary>
    ///     The absolute path of the executable the installer restarts; its folder is the one the installer updates.
    ///     Derived from the running process; set it when that fails, for example to <c>Environment.ProcessPath</c>.
    /// </summary>
    /// <exception cref="ArgumentException">The path is empty or not absolute.</exception>
    public string? ApplicationExecutablePath
    {
        get => _applicationExecutablePath;
        set => _applicationExecutablePath = value is null || IsAbsolutePath(value)
            ? value
            : throw new ArgumentException($"The executable path must be absolute, such as Path.Combine(AppContext.BaseDirectory, \"MyApp.exe\"); \"{value}\" is not.", nameof(value));
    }

    /// <summary>
    ///     The runtime identifier this client installs package files for, see <see cref="PackagePlatform" />: the one of
    ///     the running process, for example <c>win-x64</c>.
    /// </summary>
    public string Platform => _services.SystemInformation.RuntimeIdentifier;

    /// <summary>
    ///     The installer executable. Defaults to the built-in installer that the <c>nUpdate.UpdateInstaller.UI.Avalonia</c>
    ///     package places next to the application, <c>nUpdate.Installer/&lt;rid&gt;/nUpdate.UpdateInstaller.UI.Avalonia</c>
    ///     (<c>.exe</c> on Windows); set it to an installer of your own, per platform if needed. Its whole folder is
    ///     copied to the temp folder and started from there, so give it a folder of its own.
    /// </summary>
    public string? InstallerPath
    {
        get => _installerPath ?? DefaultInstallerPath();
        set => _installerPath = value;
    }

    /// <summary>Whether the installer shows its window. Without a display it installs without one anyway.</summary>
    public bool ShowInstallerWindow { get; set; } = true;

    /// <summary>A PNG file the installer window shows as its icon, or <c>null</c> for the nUpdate icon.</summary>
    public string? InstallerIcon { get; set; }

    /// <summary>
    ///     The accent color of the installer window (and of the dialogs of <c>nUpdate.UI.Avalonia</c>) as <c>#RRGGBB</c> or
    ///     <c>#AARRGGBB</c>, or <c>null</c> for the default.
    /// </summary>
    public string? InstallerAccentColor
    {
        get => _installerAccentColor;
        set => _installerAccentColor = value is null || IsColor(value)
            ? value
            : throw new ArgumentException($"\"{value}\" is not a color. Write it as #RRGGBB or #AARRGGBB.", nameof(value));
    }

    /// <summary>The folder downloaded packages are stored in.</summary>
    public string DownloadDirectory => _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), "nUpdate", ApplicationName);

    /// <summary>The least stable versions this client installs, for example <see cref="Stability.Beta" /> for betas, release candidates and releases. Defaults to releases only.</summary>
    public Stability MinimumStability { get; set; }

    /// <summary>Pre-release labels (their first identifier, such as <c>nightly</c>) this client installs regardless of <see cref="MinimumStability" />.</summary>
    public ISet<string> AcceptedPreReleaseLabels { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>Whether downloads are reported to the project's statistics.</summary>
    public bool ReportDownloads { get; set; } = true;

    /// <summary>What this client is, matched against the packages' rollout conditions.</summary>
    public IDictionary<string, string> RolloutConditions { get; } = new Dictionary<string, string>(StringComparer.Ordinal);

    /// <summary>Applied to the HTTP client when it is created on first use.</summary>
    public IWebProxy? Proxy { get; set; }

    /// <summary>Applied to the HTTP client when it is created on first use.</summary>
    public ICredentials? HttpAuthenticationCredentials { get; set; }

    /// <summary>Timeout for every HTTP request when the HTTP client is created on first use.</summary>
    public TimeSpan HttpTimeout { get; set; } = TimeSpan.FromSeconds(100);

    public AfterInstall AfterInstall { get; set; } = AfterInstall.Restart;

    /// <summary>Whether the installer asks for administrator rights through UAC. Only applies on Windows; Linux and macOS install as the current user.</summary>
    public bool RunInstallerAsAdmin { get; set; } = true;

    /// <summary>Arguments the installer passes to the restarted application.</summary>
    public IList<InstallerArgument> Arguments { get; } = [];

    /// <summary>The packages selected by the last check, in installation order.</summary>
    public IReadOnlyList<PackageInfo> AvailableUpdates { get; private set; } = [];

    /// <summary>The total size of the selected packages in bytes.</summary>
    public long TotalDownloadSize { get; private set; }

    /// <summary>The downloaded package files by version.</summary>
    public IReadOnlyDictionary<UpdateVersion, string> DownloadedPackages => _downloadedPackages;

    /// <summary>The file system the manager works on, shared with <see cref="Ui.UpdateFlow" />.</summary>
    internal IFileSystem FileSystem => _fileSystem;

    /// <summary>Downloads the feed and selects the packages for this client.</summary>
    /// <returns><c>true</c> when at least one package should be installed.</returns>
    /// <exception cref="HttpRequestException">The feed could not be downloaded.</exception>
    /// <exception cref="InvalidFeedException">The feed is not valid.</exception>
    /// <exception cref="UnsupportedFormatException">The feed was written in a format this nUpdate does not read.</exception>
    public async Task<bool> CheckForUpdatesAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        ResetPackages();

        UpdateFeed feed;
        try
        {
            feed = await FeedLoader.LoadAsync(GetHttpClient(), FeedUri, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw TimedOut(FeedUri, ex);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var options = new UpdateFilterOptions(CurrentVersion)
        {
            MinimumStability = MinimumStability,
            AcceptedPreReleaseLabels = AcceptedPreReleaseLabels.ToList(),
            RolloutConditions = new Dictionary<string, string>(RolloutConditions, StringComparer.Ordinal),
            Platform = Platform,
        };
        var selected = UpdateFilter.Select(feed.Packages, options);
        foreach (var package in selected)
        {
            // The filter only selects packages with a file for this platform.
            var file = package.FindFile(Platform)!;
            _packageFiles[package.Version] = file;
            _packageUris[package.Version] = Resolve(file.Path);
        }

        _feed = feed;
        AvailableUpdates = selected;
        TotalDownloadSize = _packageFiles.Values.Sum(f => f.Size);
        return selected.Count > 0;
    }

    /// <summary>
    ///     Downloads the selected packages into <see cref="DownloadDirectory" />, which is emptied first so that files left
    ///     behind by an earlier run do not accumulate. Every file is checked against the size and hash in the feed.
    /// </summary>
    /// <exception cref="InvalidOperationException">No check has selected packages.</exception>
    /// <exception cref="HttpRequestException">A package could not be downloaded or the server stopped responding.</exception>
    /// <exception cref="InvalidPackageException">A downloaded package does not match the feed.</exception>
    /// <exception cref="OperationCanceledException">The download was cancelled; partial files are deleted.</exception>
    public async Task DownloadAsync(IProgress<UpdateDownloadProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (AvailableUpdates.Count == 0)
            throw new InvalidOperationException("There are no packages to download. Check for updates first.");

        DeleteDownloads();
        _fileSystem.Directory.CreateDirectory(DownloadDirectory);
        DeleteStaleFiles();
        var client = GetHttpClient();
        long received = 0;
        var buffer = new byte[81920];
        var current = FeedUri;

        try
        {
            foreach (var package in AvailableUpdates)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var file = _packageFiles[package.Version];
                var filePath = _fileSystem.Path.Combine(DownloadDirectory, $"{package.Version}.zip");
                _downloadedPackages[package.Version] = filePath;
                current = _packageUris[package.Version];

                using var response = await client.GetAsync(current, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                using var input = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA512);
                long length = 0;
                using (var output = _fileSystem.File.Create(filePath))
                {
                    int read;
                    while ((read = await input.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
                    {
                        length += read;
                        if (length > file.Size)
                            throw new InvalidPackageException($"The package \"{package.Version}\" is larger than the {file.Size} bytes the feed announced.");
                        await output.WriteAsync(buffer, 0, read, cancellationToken).ConfigureAwait(false);
                        hash.AppendData(buffer, 0, read);
                        received += read;
                        progress?.Report(new UpdateDownloadProgress(received, TotalDownloadSize));
                    }
                }

                if (length != file.Size)
                    throw new InvalidPackageException($"The package \"{package.Version}\" has {length} bytes, but the feed announced {file.Size}.");
                if (!string.Equals(Convert.ToBase64String(hash.GetHashAndReset()), file.Sha512, StringComparison.Ordinal))
                    throw new InvalidPackageException($"The package \"{package.Version}\" does not match the hash in the feed.");

                await ReportDownloadAsync(package, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            DeleteDownloads();
            throw TimedOut(current, ex);
        }
        catch
        {
            DeleteDownloads();
            throw;
        }
    }

    /// <summary>
    ///     Verifies every downloaded package: the RSA-PSS signature against the project's public key, and the manifest
    ///     inside the zip against the feed entry (same project, version and platform), so a feed cannot relabel an old,
    ///     validly signed package as a newer version or hand out the package of another platform. When any package fails,
    ///     all downloads are deleted.
    /// </summary>
    /// <returns><c>true</c> when every package is authentic.</returns>
    /// <exception cref="InvalidOperationException">No packages have been downloaded.</exception>
    /// <exception cref="FileNotFoundException">A downloaded package is missing.</exception>
    /// <exception cref="ArgumentException">The public key is invalid.</exception>
    /// <exception cref="InvalidFeedException">The feed names an unknown signature algorithm or the signature is not valid Base64.</exception>
    public Task<bool> VerifyAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (_downloadedPackages.Count == 0)
            throw new InvalidOperationException("There are no downloaded packages to verify. Download them first.");
        return Task.Run(() => Verify(cancellationToken), cancellationToken);
    }

    /// <summary>
    ///     Writes the installer options, copies the installer to a temp folder, starts it and, depending on
    ///     <see cref="AfterInstall" />, terminates the host application. On Linux and macOS the installer runs as the
    ///     current user, who therefore needs write access to the application folder (for a macOS bundle, to the folder
    ///     that contains the bundle).
    /// </summary>
    /// <returns><c>false</c> when the user declined the elevation prompt; the downloaded packages are then deleted.</returns>
    /// <exception cref="InvalidOperationException">No packages have been downloaded.</exception>
    /// <exception cref="FileNotFoundException">The installer is missing.</exception>
    /// <exception cref="UnauthorizedAccessException">On Linux or macOS, the current user may not change the application's files.</exception>
    public bool StartInstaller()
    {
        ThrowIfDisposed();
        if (_downloadedPackages.Count == 0)
            throw new InvalidOperationException("There are no downloaded packages to install.");

        var executablePath = ApplicationExecutablePath
                             ?? throw new InvalidOperationException(
                                 $"The application's executable could not be determined. Set {nameof(ApplicationExecutablePath)} to its absolute path, for example Environment.ProcessPath.");
        var applicationDirectory = _fileSystem.Path.GetDirectoryName(executablePath);
        if (applicationDirectory is null || applicationDirectory.Length == 0)
            throw new InvalidOperationException($"{nameof(ApplicationExecutablePath)} \"{executablePath}\" names no file in a folder, so there is no application folder to update.");
        var installerPath = InstallerPath!; // never null once the executable path is known
        if (!_fileSystem.File.Exists(installerPath))
            throw new FileNotFoundException(string.Format(CultureInfo.CurrentCulture, Texts.InstallerNotFound, installerPath), installerPath);

        var isWindows = PackagePlatform.IsWindows(Platform);
        var bundle = PackagePlatform.OperatingSystemOf(Platform) == PackagePlatform.MacOS ? FindBundle(executablePath) : null;
        if (!isWindows)
        {
            var writable = bundle is null ? applicationDirectory : _fileSystem.Path.GetDirectoryName(bundle)!;
            if (!_services.FilePermissions.CanWrite(writable))
                throw new UnauthorizedAccessException(string.Format(CultureInfo.CurrentCulture, Texts.NoWriteAccess, ApplicationName, writable));
        }

        var targetDirectory = PrepareInstallerDirectory();
        CopyDirectory(_fileSystem.Path.GetDirectoryName(installerPath)!, targetDirectory);
        var installer = _fileSystem.Path.Combine(targetDirectory, _fileSystem.Path.GetFileName(installerPath));
        _services.FilePermissions.SetMode(installer, FilePermissions.ExecutableMode); // NuGet and copies do not always keep the execute bit

        var options = new InstallerOptions
        {
            Packages = _downloadedPackages.OrderBy(pair => pair.Key).Select(pair => new InstallerPackage { Path = pair.Value }).ToList(),
            Application = new ApplicationOptions
            {
                Name = ApplicationName,
                Directory = applicationDirectory,
                ExecutablePath = executablePath,
                Bundle = bundle,
            },
            Host = new HostOptions
            {
                ProcessId = AfterInstall == AfterInstall.KeepRunning ? null : _services.ApplicationInfo.CurrentProcessId,
                AfterInstall = AfterInstall,
            },
            Arguments = Arguments.ToList(),
            Ui = new InstallerUiOptions
            {
                ShowWindow = ShowInstallerWindow,
                IconPath = CopyInstallerIcon(targetDirectory),
                AccentColor = InstallerAccentColor,
            },
            Texts = InstallerTextMapper.ToInstallerTexts(Texts),
        };
        var optionsPath = _fileSystem.Path.Combine(targetDirectory, OptionsFileName);
        _fileSystem.File.WriteAllText(optionsPath, Serializer.Serialize(options, indented: true));

        var started = _services.ProcessLauncher.Start(installer, $"\"{optionsPath}\"", RunInstallerAsAdmin && isWindows);
        if (!started)
        {
            DeleteDownloads();
            return false;
        }

        if (AfterInstall != AfterInstall.KeepRunning)
            _services.ApplicationTerminator.Terminate();
        return true;
    }

    /// <summary>Deletes the downloaded package files. A file that cannot be deleted is logged and left behind.</summary>
    public void DeleteDownloads()
    {
        foreach (var path in _downloadedPackages.Values)
        {
            try
            {
                if (_fileSystem.File.Exists(path))
                    _fileSystem.File.Delete(path);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                _services.Logger.LogWarning(ex, "The downloaded package {Path} could not be deleted.", path);
            }
        }

        _downloadedPackages.Clear();
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _ownedHttpClient?.Dispose();
        _disposed = true;
    }

    /// <summary>The built-in installer for the running process next to the application, or <c>null</c> while the executable is unknown.</summary>
    private string? DefaultInstallerPath()
    {
        if (ApplicationExecutablePath is null)
            return null;
        var fileName = PackagePlatform.IsWindows(Platform) ? BuiltInInstallerName + ".exe" : BuiltInInstallerName;
        return _fileSystem.Path.Combine(_fileSystem.Path.GetDirectoryName(ApplicationExecutablePath) ?? string.Empty, InstallerFolderName, Platform, fileName);
    }

    /// <summary>
    ///     The temp folder the installer is copied to: one per application, emptied first so installer files do not
    ///     accumulate. When an earlier installer still holds a file in it (it is still running, or a virus scanner reads
    ///     it), the installer goes to a new folder next to it, which a later update removes again.
    /// </summary>
    private string PrepareInstallerDirectory()
    {
        var root = _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), InstallerTempFolderName);
        var folder = _fileSystem.Path.Combine(root, ApplicationName);
        if (_fileSystem.Directory.Exists(root))
        {
            foreach (var earlier in _fileSystem.Directory.GetDirectories(root))
            {
                if (IsAlternativeInstallerFolder(_fileSystem.Path.GetFileName(earlier)))
                    TryDeleteDirectory(earlier);
            }
        }

        if (TryDeleteDirectory(folder))
            return folder;
        var alternative = folder + "-" + Guid.NewGuid().ToString("N");
        _services.Logger.LogWarning("The installer folder {Folder} is still in use, so the installer runs from {Alternative}.", folder, alternative);
        return alternative;
    }

    /// <summary>
    ///     Whether the folder is one an earlier run used instead of the application's folder: the name, a dash and 32 hex
    ///     digits. Checked here, not with a search pattern, because a trailing <c>?</c> also matches nothing on Windows,
    ///     which would take in the folders of other applications.
    /// </summary>
    private bool IsAlternativeInstallerFolder(string name) =>
        name.Length == ApplicationName.Length + 33
        && name.StartsWith(ApplicationName + "-", StringComparison.Ordinal)
        && name.Skip(ApplicationName.Length + 1).All(c => Uri.IsHexDigit(c));

    /// <summary>Deletes the directory if it exists; <c>false</c> when a file in it is in use.</summary>
    private bool TryDeleteDirectory(string path)
    {
        try
        {
            if (_fileSystem.Directory.Exists(path))
                _fileSystem.Directory.Delete(path, recursive: true);
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }

    private bool IsAbsolutePath(string? path) => !string.IsNullOrWhiteSpace(path) && _fileSystem.Path.IsPathRooted(path);

    /// <summary>The <c>.app</c> folder of an executable in <c>…/MyApp.app/Contents/MacOS/</c>, or <c>null</c>.</summary>
    internal static string? FindBundle(string executablePath)
    {
        var parts = executablePath.Split('/');
        if (parts.Length < 4 || parts[parts.Length - 2] != "MacOS" || parts[parts.Length - 3] != "Contents" || !parts[parts.Length - 4].EndsWith(".app", StringComparison.OrdinalIgnoreCase))
            return null;
        return string.Join("/", parts, 0, parts.Length - 3);
    }

    /// <summary>Copies <see cref="InstallerIcon" /> next to the installer, so it neither locks nor loses the application's file. A missing icon is logged and left out.</summary>
    private string? CopyInstallerIcon(string targetDirectory)
    {
        if (InstallerIcon is null)
            return null;
        if (!_fileSystem.File.Exists(InstallerIcon))
        {
            _services.Logger.LogWarning("The installer icon {Path} does not exist; the installer shows its own.", InstallerIcon);
            return null;
        }

        var target = _fileSystem.Path.Combine(targetDirectory, InstallerIconFileName);
        _fileSystem.File.Copy(InstallerIcon, target, overwrite: true);
        return target;
    }

    private static bool IsColor(string value) =>
        (value.Length == 7 || value.Length == 9) && value[0] == '#' && value.Skip(1).All(Uri.IsHexDigit);

    private static UpdateVersion ParseDeclaredVersion(string? declaredVersion)
    {
        if (declaredVersion is null)
            throw new InvalidOperationException($"No current version was given and the entry assembly has no {nameof(ApplicationVersionAttribute)}.");
        if (!UpdateVersion.TryParse(declaredVersion, out var version))
            throw new InvalidOperationException($"The {nameof(ApplicationVersionAttribute)} of the entry assembly declares \"{declaredVersion}\", which is not a valid version. {UpdateVersion.FormatDescription}");
        return version!;
    }

    /// <summary>An absolute http(s) URL as it is, anything else (including a root-relative path, which Unix would take for a file URI) relative to the feed.</summary>
    private Uri Resolve(string path) =>
        Uri.TryCreate(path, UriKind.Absolute, out var absolute) && (absolute.Scheme == Uri.UriSchemeHttp || absolute.Scheme == Uri.UriSchemeHttps)
            ? absolute
            : new Uri(FeedUri, path);

    private bool Verify(CancellationToken cancellationToken)
    {
        using var verifier = PackageSigning.FromPublicKey(PublicKey);
        var allValid = true;
        foreach (var pair in _downloadedPackages)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!_fileSystem.File.Exists(pair.Value))
                throw new FileNotFoundException(string.Format(CultureInfo.CurrentCulture, Texts.PackageFileNotFound, pair.Key), pair.Value);

            var file = _packageFiles[pair.Key];
            if (!string.Equals(file.Signature.Algorithm, PackageSignature.RsaPssSha512, StringComparison.OrdinalIgnoreCase))
                throw new InvalidFeedException($"The package \"{pair.Key}\" is signed with \"{file.Signature.Algorithm}\", which this nUpdate does not verify.");

            byte[] signature;
            try
            {
                signature = Convert.FromBase64String(file.Signature.Value);
            }
            catch (FormatException ex)
            {
                throw new InvalidFeedException($"The signature of version \"{pair.Key}\" is not valid Base64.", ex);
            }

            using (var stream = _fileSystem.File.OpenRead(pair.Value))
            {
                if (!verifier.Verify(stream, signature))
                {
                    allValid = false;
                    continue;
                }
            }

            if (!ManifestMatches(pair.Value, pair.Key, file))
                allValid = false;
        }

        if (allValid)
            return true;

        DeleteDownloads();
        return false;
    }

    /// <summary>The signed manifest must describe this project, version and platform; otherwise the feed relabelled another package.</summary>
    private bool ManifestMatches(string packagePath, UpdateVersion version, PackageFile file)
    {
        try
        {
            using var stream = _fileSystem.File.OpenRead(packagePath);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
            var entry = archive.GetEntry(PackageLayout.ManifestFileName);
            if (entry is null)
                return false;
            using var manifestStream = entry.Open();
            var manifest = Serializer.Deserialize<PackageManifest>(manifestStream);
            return manifest is not null && manifest.ProjectId == _feed!.ProjectId && manifest.Version == version
                   && string.Equals(manifest.Platform, file.Platform, StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex) when (ex is InvalidDataException or Newtonsoft.Json.JsonException or IOException)
        {
            _services.Logger.LogWarning(ex, "The manifest of {Version} could not be read.", version);
            return false;
        }
    }

    /// <summary>
    ///     <see cref="HttpClient" /> signals a timeout as a cancellation. Only a cancellation the caller asked for is one;
    ///     everything else is a failed request.
    /// </summary>
    private static HttpRequestException TimedOut(Uri uri, OperationCanceledException exception) =>
        new($"The request to \"{uri}\" timed out.", exception);

    /// <summary>Removes files an earlier run left in <see cref="DownloadDirectory" />; best effort.</summary>
    private void DeleteStaleFiles()
    {
        foreach (var file in _fileSystem.Directory.GetFiles(DownloadDirectory))
        {
            try
            {
                _fileSystem.File.Delete(file);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                _services.Logger.LogWarning(ex, "The stale download {Path} could not be deleted.", file);
            }
        }
    }

    private async Task ReportDownloadAsync(PackageInfo package, CancellationToken cancellationToken)
    {
        if (!ReportDownloads || package.Statistics is not { Enabled: true } statistics || string.IsNullOrWhiteSpace(statistics.Url))
            return;

        var report = new DownloadReport(_feed!.ProjectId, package.Version, _services.SystemInformation.OperatingSystemName);
        try
        {
            await StatisticsApi.ReportDownloadAsync(GetHttpClient(), Resolve(statistics.Url), report, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is HttpRequestException or UriFormatException or NotSupportedException || (ex is OperationCanceledException && !cancellationToken.IsCancellationRequested))
        {
            _services.Logger.LogWarning(ex, "The download of {Version} could not be reported to {Endpoint}.", package.Version, statistics.Url);
        }
    }

    private void CopyDirectory(string source, string target)
    {
        _fileSystem.Directory.CreateDirectory(target);
        foreach (var file in _fileSystem.Directory.GetFiles(source))
            _fileSystem.File.Copy(file, _fileSystem.Path.Combine(target, _fileSystem.Path.GetFileName(file)), overwrite: true);
        foreach (var directory in _fileSystem.Directory.GetDirectories(source))
            CopyDirectory(directory, _fileSystem.Path.Combine(target, _fileSystem.Path.GetFileName(directory)));
    }

    private void ResetPackages()
    {
        DeleteDownloads();
        _packageUris.Clear();
        _packageFiles.Clear();
        _feed = null;
        AvailableUpdates = [];
        TotalDownloadSize = 0;
    }

    private HttpClient GetHttpClient()
    {
        if (_httpClient is not null)
            return _httpClient;

        if (_services.HttpClient is not null)
        {
            _httpClient = _services.HttpClient;
            return _httpClient;
        }

        var handler = new HttpClientHandler();
        if (Proxy is not null)
        {
            handler.Proxy = Proxy;
            handler.UseProxy = true;
        }

        if (HttpAuthenticationCredentials is not null)
        {
            handler.Credentials = HttpAuthenticationCredentials;
            handler.PreAuthenticate = true;
        }

        var client = new HttpClient(handler, disposeHandler: true) { Timeout = HttpTimeout };
        client.DefaultRequestHeaders.UserAgent.Clear();
        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", UserAgent());
        client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
        _ownedHttpClient = client;
        _httpClient = client;
        return client;
    }

    /// <summary>For example <c>MyApp/2.1.0 (Windows 11; win-x64; nUpdate/5.0)</c>.</summary>
    internal string UserAgent()
    {
        var library = typeof(UpdateManager).Assembly.GetName().Version!.ToString(2);
        return $"{_services.ApplicationInfo.UserAgentProduct} ({_services.SystemInformation.OperatingSystemName}; {Platform}; nUpdate/{library})";
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(UpdateManager));
    }
}
