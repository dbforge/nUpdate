using nUpdate.Administration.Core.Transfer;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Tests.Administration.Core;

public class TransferGuardTests
{
    [Fact]
    public async Task Run_WrapsOnlyTheProviderExceptions()
    {
        static bool IsTransferError(Exception ex) => ex is IOException;

        await TransferGuard.RunAsync(() => Task.CompletedTask, IsTransferError);
        (await TransferGuard.RunAsync(() => Task.FromResult(7), IsTransferError)).ShouldBe(7);

        var wrapped = await Should.ThrowAsync<TransferException>(() => TransferGuard.RunAsync(() => throw new IOException("broken pipe"), IsTransferError));
        wrapped.Message.ShouldBe("broken pipe");
        wrapped.InnerException.ShouldBeOfType<IOException>();
        await Should.ThrowAsync<TransferException>(() => TransferGuard.RunAsync<int>(() => throw new IOException("x"), IsTransferError));

        await Should.ThrowAsync<InvalidOperationException>(() => TransferGuard.RunAsync(() => throw new InvalidOperationException(), IsTransferError));
        await Should.ThrowAsync<InvalidOperationException>(() => TransferGuard.RunAsync<int>(() => throw new InvalidOperationException(), IsTransferError));
    }
}
