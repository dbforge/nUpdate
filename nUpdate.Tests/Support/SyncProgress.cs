namespace nUpdate.Tests.Support;

/// <summary>Records progress reports as they happen; <see cref="Progress{T}" /> would post them to the thread pool instead.</summary>
public sealed class SyncProgress<T>(List<T> reports) : IProgress<T>
{
    public void Report(T value) => reports.Add(value);
}
