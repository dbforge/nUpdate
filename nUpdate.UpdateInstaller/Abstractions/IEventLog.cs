namespace nUpdate.UpdateInstaller.Abstractions;

/// <summary>The system log for failures that nobody sees otherwise.</summary>
internal interface IEventLog
{
    /// <summary>Writes an error entry; best effort, never throws.</summary>
    void WriteError(string message);
}
