using System.Globalization;
using System.IO.Abstractions;
using System.Text;

namespace nUpdate.UpdateInstaller.Reporting;

/// <summary>
///     <c>install.log</c>: one line per event with a timestamp, flushed as it happens so the file is complete even when
///     the installer is killed, and readable while the installer runs. Writing is best effort; a log that cannot be
///     written never stops an update.
/// </summary>
public sealed class InstallLog : IDisposable
{
    private readonly IFileSystem _fileSystem;
    private readonly Func<DateTimeOffset> _clock;
    private readonly object _gate = new();
    private StreamWriter? _writer;
    private bool _unwritable;

    /// <param name="fileSystem">The file system to write in.</param>
    /// <param name="path">The log file, or <c>null</c> when there is nowhere to write it.</param>
    /// <param name="clock">The time of each line.</param>
    public InstallLog(IFileSystem fileSystem, string? path, Func<DateTimeOffset> clock)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        Path = path;
        _unwritable = path is null;
    }

    /// <summary>The log file, or <c>null</c> when nothing is written.</summary>
    public string? Path { get; }

    public void Write(string message)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));

        var line = _clock().ToString("yyyy-MM-dd HH:mm:ss.fff zzz", CultureInfo.InvariantCulture) + "  " + message;
        lock (_gate)
        {
            if (_unwritable)
                return;
            try
            {
                _writer ??= new StreamWriter(
                    _fileSystem.FileStream.New(Path!, FileMode.Append, FileAccess.Write, FileShare.ReadWrite),
                    new UTF8Encoding(false))
                { AutoFlush = true };
                _writer.WriteLine(line);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                _unwritable = true; // logging is best effort
            }
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            _writer?.Dispose();
            _writer = null;
            _unwritable = true;
        }
    }
}
