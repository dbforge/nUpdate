namespace nUpdate.Platform;

/// <summary>Starts the update installer process.</summary>
public interface IProcessLauncher
{
    /// <summary>
    ///     Starts the executable, elevated through UAC when <paramref name="elevated" /> is set (Windows only). Returns
    ///     <c>false</c> when the user declined the elevation prompt, so the caller can clean up without treating it as an
    ///     error. Any other failure throws.
    /// </summary>
    bool Start(string fileName, string arguments, bool elevated);
}
