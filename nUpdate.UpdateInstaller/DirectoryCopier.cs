using System.IO.Abstractions;
using nUpdate.Installer;
using nUpdate.UpdateInstaller.Operations;

namespace nUpdate.UpdateInstaller;

/// <summary>
///     Copies a package root (for example <c>Program</c>) over the target directory, asking the UI what to do with
///     files that are locked by another process. Every file is first copied next to its target and then swapped in,
///     so a failure never leaves a half-written file behind. On Windows an exclusive open finds locked files before
///     anything is touched; Linux and macOS replace files that are in use (even running executables) without trouble.
/// </summary>
internal sealed class DirectoryCopier
{
    private const int ErrorSharingViolation = 32;
    private const int ErrorLockViolation = 33;

    /// <summary>Suffix of the staged copy written next to a target file before it replaces it.</summary>
    public const string TempSuffix = ".nupdate-tmp";

    private readonly IFileSystem _fileSystem;
    private readonly int _maxLockedFileAttempts;
    private readonly bool _probeLocks;

    /// <param name="fileSystem">The file system to copy in.</param>
    /// <param name="maxLockedFileAttempts">How often a locked file is tried before the copy is aborted.</param>
    /// <param name="probeLocks">Whether to open every existing target exclusively first, which only Windows needs.</param>
    public DirectoryCopier(IFileSystem fileSystem, int maxLockedFileAttempts, bool probeLocks = true)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        if (maxLockedFileAttempts < 1)
            throw new ArgumentOutOfRangeException(nameof(maxLockedFileAttempts));
        _maxLockedFileAttempts = maxLockedFileAttempts;
        _probeLocks = probeLocks;
    }

    /// <summary>Counts the files below the directory, which is the number of progress steps copying it takes.</summary>
    public int CountFiles(string directory) =>
        _fileSystem.Directory.Exists(directory)
            ? _fileSystem.Directory.GetFiles(directory, "*", SearchOption.AllDirectories).Length
            : 0;

    /// <exception cref="LockedFileException">A locked file could not be replaced and the UI chose to abort.</exception>
    public void Copy(string sourceDirectory, string targetDirectory, OperationContext context)
    {
        if (sourceDirectory is null)
            throw new ArgumentNullException(nameof(sourceDirectory));
        if (targetDirectory is null)
            throw new ArgumentNullException(nameof(targetDirectory));
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        _fileSystem.Directory.CreateDirectory(targetDirectory);
        foreach (var file in _fileSystem.Directory.GetFiles(sourceDirectory))
        {
            var fileName = _fileSystem.Path.GetFileName(file);
            CopyFile(file, _fileSystem.Path.Combine(targetDirectory, fileName), context);
            context.Reporter.ReportUnpackingProgress(context.Progress.Advance(), fileName);
        }

        foreach (var directory in _fileSystem.Directory.GetDirectories(sourceDirectory))
            Copy(directory, _fileSystem.Path.Combine(targetDirectory, _fileSystem.Path.GetFileName(directory)),
                context);
    }

    /// <summary>Whether the exception means another process holds the file open (Win32 sharing or lock violation).</summary>
    public static bool IsLockedFileError(IOException exception)
    {
        if (exception is null)
            throw new ArgumentNullException(nameof(exception));
        var code = exception.HResult & 0xFFFF;
        return code is ErrorSharingViolation or ErrorLockViolation;
    }

    private void CopyFile(string source, string target, OperationContext context)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                ReplaceFile(source, target);
                return;
            }
            catch (IOException ex) when (IsLockedFileError(ex))
            {
                // The UI is asked every time; on the last attempt only Skip and Abort remain.
                var decision = context.Reporter.ReportLockedFile(target, attempt);
                if (decision == LockedFileDecision.Retry && attempt >= _maxLockedFileAttempts)
                    decision = LockedFileDecision.Abort;
                switch (decision)
                {
                    case LockedFileDecision.Retry:
                        continue;
                    case LockedFileDecision.Skip:
                        return;
                    default:
                        throw new LockedFileException(context.Options.Text(InstallerText.FileInUseError, target),
                            target, ex);
                }
            }
        }
    }

    /// <summary>Copies the source next to the target (keeping its timestamps) and swaps it in once the target is free.</summary>
    private void ReplaceFile(string source, string target)
    {
        var staged = target + TempSuffix;
        _fileSystem.File.Copy(source, staged, overwrite: true);
        try
        {
            if (_fileSystem.File.Exists(target))
            {
                // Opening exclusively surfaces a sharing violation before anything is touched.
                if (_probeLocks)
                {
                    using (_fileSystem.File.Open(target, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                    }
                }

                _fileSystem.File.Delete(target);
            }

            _fileSystem.File.Move(staged, target);
        }
        catch (Exception)
        {
            TryDelete(staged);
            throw;
        }
    }

    private void TryDelete(string path)
    {
        try
        {
            _fileSystem.File.Delete(path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Best effort; the staged copy is harmless.
        }
    }
}
