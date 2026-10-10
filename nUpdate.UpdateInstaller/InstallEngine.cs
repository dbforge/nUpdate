using System.Text;
using nUpdate.Installer;
using nUpdate.Packaging;
using nUpdate.UpdateInstaller.Operations;
using nUpdate.Updating;

namespace nUpdate.UpdateInstaller;

/// <summary>
///     Applies downloaded packages: waits for the host to exit, extracts every package, reads its manifest, runs its
///     operations and copies its files in version order, cleans up and restarts the host. Runs synchronously;
///     <see cref="InstallerHost" /> starts it on a background thread while the UI blocks in
///     <see cref="IProgressReporter.Initialize" />. For a macOS bundle the <c>Program</c> root is built next to the
///     installed bundle and swapped in as a whole.
/// </summary>
internal sealed class InstallEngine
{
    private readonly InstallerServices _services;
    private readonly OperationDispatcher _dispatcher;

    public InstallEngine(InstallerServices? services = null)
    {
        _services = services ?? new InstallerServices();
        _dispatcher = new OperationDispatcher(_services.OperationHandlers);
    }

    /// <summary>
    ///     Runs the whole update. Apart from the argument checks it never throws: failures, including failures of the
    ///     reporter itself, are reported where possible and returned.
    /// </summary>
    public InstallResult Run(InstallerOptions options, IProgressReporter reporter)
    {
        if (options is null)
            throw new ArgumentNullException(nameof(options));
        if (reporter is null)
            throw new ArgumentNullException(nameof(reporter));

        var fileSystem = _services.FileSystem;
        var workingDirectory = string.Empty;
        try
        {
            if (options.Packages.Count == 0)
                throw new InvalidDataException("The installer options name no packages.");
            workingDirectory = fileSystem.Path.GetDirectoryName(options.Packages[0].Path) ?? string.Empty;
            if (workingDirectory.Length == 0)
                throw new InvalidDataException(
                    $"The package directory of \"{options.Packages[0].Path}\" could not be determined.");
            foreach (var package in options.Packages)
            {
                if (!fileSystem.File.Exists(package.Path))
                    throw new FileNotFoundException($"The package \"{package.Path}\" does not exist.", package.Path);
            }

            if (options.Host.ProcessId is { } hostProcessId)
            {
                reporter.ReportOperationProgress(0f,
                    options.Text(InstallerText.WaitingForApplication, options.Application.Name));
                _services.ProcessService.WaitForExit(hostProcessId, _services.HostExitTimeout);
            }

            var progress = new ProgressTracker();
            var context = new OperationContext(options, _services,
                new PathPlaceholderResolver(fileSystem, options.Application.ProgramDirectory, _services.SpecialFolders),
                progress, reporter);
            var copier = new DirectoryCopier(fileSystem, _services.MaxLockedFileAttempts,
                probeLocks: _services.EnvironmentInfo.IsWindows);

            var packages = Prepare(options, workingDirectory, copier, progress);
            foreach (var package in packages)
            {
                _dispatcher.Execute(package.Manifest.Operations.Where(o => o.RunBeforeFileReplacement), context);
                CopyRoots(package, copier, context, options);
                _dispatcher.Execute(package.Manifest.Operations.Where(o => !o.RunBeforeFileReplacement), context);
            }

            Finish(options, workingDirectory, reporter, ArgumentCondition.Succeeded);
            return InstallResult.Success;
        }
        catch (Exception ex)
        {
            Report(reporter, ex);
            Finish(options, workingDirectory, reporter, ArgumentCondition.Failed);
            return InstallResult.Failure(ex);
        }
        finally
        {
            reporter.Terminate();
        }
    }

    /// <summary>
    ///     Builds the command line the host is restarted with: the arguments for the outcome plus those for every
    ///     outcome. Arguments are quoted and escaped the way the Windows command line parser expects; an argument that
    ///     is already enclosed in quotes is passed through unchanged.
    /// </summary>
    public static string BuildArguments(IEnumerable<InstallerArgument> arguments, ArgumentCondition outcome)
    {
        if (arguments is null)
            throw new ArgumentNullException(nameof(arguments));
        return string.Join(" ",
            arguments.Where(a => a.When == outcome || a.When == ArgumentCondition.Always).Select(a => Quote(a.Value)));
    }

    private static string Quote(string argument)
    {
        if (argument.Length >= 2 && argument[0] == '"' && argument[argument.Length - 1] == '"')
            return argument;
        if (argument.Length > 0 && argument.IndexOfAny([' ', '\t', '"']) < 0)
            return argument;

        var builder = new StringBuilder("\"");
        var backslashes = 0;
        foreach (var c in argument)
        {
            if (c == '\\')
            {
                backslashes++;
                continue;
            }

            if (c == '"')
            {
                builder.Append('\\', backslashes * 2 + 1).Append('"');
                backslashes = 0;
                continue;
            }

            builder.Append('\\', backslashes).Append(c);
            backslashes = 0;
        }

        return builder.Append('\\', backslashes * 2).Append('"').ToString();
    }

    /// <summary>Cleans up and restarts the host. Both are best effort: a clean-up problem is a warning, a failed restart an error.</summary>
    private void Finish(InstallerOptions options, string workingDirectory, IProgressReporter reporter,
        ArgumentCondition outcome)
    {
        CleanUp(workingDirectory, reporter);
        try
        {
            RestartHost(options, outcome);
        }
        catch (Exception ex)
        {
            Report(reporter, ex);
        }
    }

    /// <summary>
    ///     Records a problem that does not fail the update, such as a temp folder that cannot be deleted: it shows in the
    ///     status line and lands in <c>install.log</c>, but no error is shown.
    /// </summary>
    private static void Warn(IProgressReporter reporter, float progress, string message)
    {
        try
        {
            reporter.ReportOperationProgress(progress, "Warning: " + message);
        }
        catch (Exception)
        {
            // Nothing is left to report to.
        }
    }

    /// <summary>Reports a failure; a reporter that throws must not take the engine down.</summary>
    private static void Report(IProgressReporter reporter, Exception exception)
    {
        try
        {
            reporter.Fail(exception);
        }
        catch (Exception)
        {
            // Nothing is left to report to.
        }
    }

    private List<ExtractedPackage> Prepare(InstallerOptions options, string workingDirectory, DirectoryCopier copier,
        ProgressTracker progress)
    {
        var fileSystem = _services.FileSystem;
        var packages = new List<ExtractedPackage>();
        foreach (var package in options.Packages)
        {
            var directory = fileSystem.Path.Combine(workingDirectory,
                fileSystem.Path.GetFileNameWithoutExtension(package.Path));
            // A folder left by an earlier, failed run must not contribute stale files.
            if (fileSystem.Directory.Exists(directory))
                fileSystem.Directory.Delete(directory, recursive: true);
            _services.PackageExtractor.Extract(package.Path, directory);
            var manifest = ReadManifest(fileSystem.Path.Combine(directory, PackageLayout.ManifestFileName),
                package.Path);
            if (!_services.EnvironmentInfo.IsWindows && manifest.Operations.Any(o => o.RequiresWindows))
                throw new InvalidDataException(
                    $"The package {manifest.Version} contains registry or service operations, which only exist on Windows.");
            CheckBundle(options, directory, manifest);

            progress.AddTasks(PackageLayout.Roots.Sum(root =>
                copier.CountFiles(fileSystem.Path.Combine(directory, PackageLayout.FolderName(root)))));
            progress.AddTasks(_dispatcher.CountTasks(manifest.Operations));
            packages.Add(new ExtractedPackage(manifest, directory));
        }

        return packages.OrderBy(p => p.Manifest.Version).ToList();
    }

    /// <summary>A bundle is replaced as a whole, so a package that ships program files must ship the whole bundle.</summary>
    private void CheckBundle(InstallerOptions options, string packageDirectory, PackageManifest manifest)
    {
        if (string.IsNullOrEmpty(options.Application.Bundle))
            return;
        var fileSystem = _services.FileSystem;
        var program = fileSystem.Path.Combine(packageDirectory, PackageLayout.FolderName(PackageRoot.Program));
        if (fileSystem.Directory.Exists(program) &&
            !fileSystem.File.Exists(fileSystem.Path.Combine(program, "Contents", "Info.plist")))
        {
            throw new InvalidDataException(
                $"The package {manifest.Version} replaces the application bundle, but its Program folder is not a complete bundle (Contents/Info.plist is missing).");
        }
    }

    private PackageManifest ReadManifest(string manifestPath, string packagePath)
    {
        if (!_services.FileSystem.File.Exists(manifestPath))
            throw new InvalidDataException(
                $"The package \"{packagePath}\" has no {PackageLayout.ManifestFileName}. It was not created by nUpdate Administration 5.");
        PackageManifest? manifest;
        try
        {
            manifest = Serializer.Deserialize<PackageManifest>(_services.FileSystem.File.ReadAllText(manifestPath));
        }
        catch (Newtonsoft.Json.JsonException ex)
        {
            throw new InvalidDataException($"The manifest of the package \"{packagePath}\" is not valid: {ex.Message}",
                ex);
        }

        if (manifest is null)
            throw new InvalidDataException($"The manifest of the package \"{packagePath}\" is empty.");
        FormatVersion.Check(manifest.Format, PackageManifest.CurrentFormat, "package manifest");
        return manifest;
    }

    private void CopyRoots(ExtractedPackage package, DirectoryCopier copier, OperationContext context,
        InstallerOptions options)
    {
        var fileSystem = _services.FileSystem;
        var packageDirectory = package.Directory;
        var targets = new Dictionary<PackageRoot, string?>
        {
            [PackageRoot.Program] = options.Application.ProgramDirectory,
            [PackageRoot.AppData] = _services.SpecialFolders.ApplicationData,
            [PackageRoot.Temp] = _services.SpecialFolders.Temp,
            [PackageRoot.Desktop] =
                _services.EnvironmentInfo.IsServiceContext ? null : _services.SpecialFolders.Desktop,
        };

        foreach (var root in PackageLayout.Roots)
        {
            var source = fileSystem.Path.Combine(packageDirectory, PackageLayout.FolderName(root));
            if (!fileSystem.Directory.Exists(source) || copier.CountFiles(source) == 0)
                continue;
            var target = targets[root];
            if (target is null)
            {
                // Desktop files are skipped in a service context; account for their progress steps anyway.
                foreach (var _ in fileSystem.Directory.GetFiles(source, "*", SearchOption.AllDirectories))
                    context.Progress.Advance();
                continue;
            }

            if (root == PackageRoot.Program && !string.IsNullOrEmpty(options.Application.Bundle))
            {
                ReplaceBundle(source, target, copier, context, package.Manifest);
            }
            else
            {
                copier.Copy(source, target, context);
                ApplyCodeSignatures(package.Manifest, root, target, context);
            }
        }
    }

    /// <summary>
    ///     Sets the macOS code signature attributes the manifest stores for the files of a root on their copies. A file
    ///     that does not get them still works, but its bundle no longer verifies, so a failure is a warning.
    /// </summary>
    private void ApplyCodeSignatures(PackageManifest manifest, PackageRoot root, string target, OperationContext context)
    {
        var fileSystem = _services.FileSystem;
        var prefix = PackageLayout.FolderName(root) + "/";
        var directory = fileSystem.Path.GetFullPath(target).TrimEnd(fileSystem.Path.DirectorySeparatorChar)
                        + fileSystem.Path.DirectorySeparatorChar;
        foreach (var file in manifest.CodeSignatures.Where(s => s.Key.StartsWith(prefix, StringComparison.Ordinal)))
        {
            var path = fileSystem.Path.GetFullPath(fileSystem.Path.Combine(directory,
                file.Key.Substring(prefix.Length).Replace('/', fileSystem.Path.DirectorySeparatorChar)));
            // Only files the package installed below the root: the manifest is signed, but a stray name changes nothing.
            if (!path.StartsWith(directory, StringComparison.Ordinal) || !fileSystem.File.Exists(path))
                continue;
            foreach (var attribute in file.Value)
            {
                try
                {
                    _services.CodeSignatures.Write(path, attribute.Key, Convert.FromBase64String(attribute.Value));
                }
                catch (Exception ex) when (ex is IOException or ArgumentException or FormatException)
                {
                    Warn(context.Reporter, context.Progress.Percentage, ex.Message);
                }
            }
        }
    }

    /// <summary>
    ///     Builds the new bundle next to the installed one, with its code signature attributes, swaps the two and deletes
    ///     the old one.
    /// </summary>
    private void ReplaceBundle(string source, string bundle, DirectoryCopier copier, OperationContext context,
        PackageManifest manifest)
    {
        var fileSystem = _services.FileSystem;
        var replacement = bundle.TrimEnd('/') + ".new";
        if (fileSystem.Directory.Exists(replacement))
            fileSystem.Directory.Delete(replacement, recursive: true);
        try
        {
            copier.Copy(source, replacement, context);
            ApplyCodeSignatures(manifest, PackageRoot.Program, replacement, context);
        }
        catch (Exception)
        {
            TryDeleteDirectory(replacement); // a half-built bundle is of no use; the installed one is untouched
            throw;
        }

        _services.DirectorySwap.Swap(bundle, replacement);
        if (!TryDeleteDirectory(replacement))
            Warn(context.Reporter, context.Progress.Percentage,
                $"The previous bundle could not be deleted from \"{replacement}\".");
    }

    private void CleanUp(string workingDirectory, IProgressReporter reporter)
    {
        try
        {
            if (workingDirectory.Length > 0 && _services.FileSystem.Directory.Exists(workingDirectory))
                _services.FileSystem.Directory.Delete(workingDirectory, recursive: true);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Warn(reporter, 100f, ex.Message);
        }
    }

    /// <summary>Deletes the directory if it exists; <c>false</c> when it is still there.</summary>
    private bool TryDeleteDirectory(string path)
    {
        try
        {
            if (_services.FileSystem.Directory.Exists(path))
                _services.FileSystem.Directory.Delete(path, recursive: true);
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }

    private void RestartHost(InstallerOptions options, ArgumentCondition outcome)
    {
        if (options.Host.AfterInstall != AfterInstall.Restart)
            return;
        _services.ProcessService.Start(options.Application.ExecutablePath, BuildArguments(options.Arguments, outcome));
    }

    private sealed class ExtractedPackage(PackageManifest manifest, string directory)
    {
        public PackageManifest Manifest { get; } = manifest;

        public string Directory { get; } = directory;
    }
}
