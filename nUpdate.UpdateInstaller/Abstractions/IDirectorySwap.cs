namespace nUpdate.UpdateInstaller.Abstractions;

/// <summary>Exchanges two directories, for replacing a macOS application bundle as a whole.</summary>
public interface IDirectorySwap
{
    /// <summary>
    ///     Exchanges the directories: afterwards <paramref name="current" /> holds what <paramref name="replacement" />
    ///     held and <paramref name="replacement" /> holds the former <paramref name="current" />.
    /// </summary>
    void Swap(string current, string replacement);
}
