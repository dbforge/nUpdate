using nUpdate.Administration.TransferInterface;

namespace nUpdate.Administration.Core.Transfer;

/// <summary>
///     Maps the library-specific exceptions of a transfer provider to <see cref="TransferException" />, with the message
///     the provider makes of them.
/// </summary>
internal static class TransferGuard
{
    public static async Task RunAsync(Func<Task> action, Func<Exception, bool> isTransferError, Func<Exception, string> describe)
    {
        try
        {
            await action().ConfigureAwait(false);
        }
        catch (Exception ex) when (isTransferError(ex))
        {
            throw new TransferException(describe(ex), ex);
        }
    }

    public static async Task<T> RunAsync<T>(Func<Task<T>> action, Func<Exception, bool> isTransferError, Func<Exception, string> describe)
    {
        try
        {
            return await action().ConfigureAwait(false);
        }
        catch (Exception ex) when (isTransferError(ex))
        {
            throw new TransferException(describe(ex), ex);
        }
    }
}
