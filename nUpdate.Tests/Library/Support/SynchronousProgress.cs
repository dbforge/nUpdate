using nUpdate.Updating;

namespace nUpdate.Tests.Library.Support;

/// <summary>Collects download progress on the reporting thread, unlike <see cref="Progress{T}" />, which posts to a context.</summary>
public sealed class SynchronousProgress(List<UpdateDownloadProgress> reports) : IProgress<UpdateDownloadProgress>
{
    public Action? OnReport { get; init; }

    public void Report(UpdateDownloadProgress value)
    {
        reports.Add(value);
        OnReport?.Invoke();
    }
}
