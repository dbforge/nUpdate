using nUpdate.Administration.Core.Transfer;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Tests.Administration.Core;

public class TransferGuardTests
{
    [Fact]
    public async Task Run_WrapsOnlyTheProviderExceptions()
    {
        static bool IsTransferError(Exception ex) => ex is IOException;
        static string Describe(Exception ex) => "described: " + ex.Message;

        await TransferGuard.RunAsync(() => Task.CompletedTask, IsTransferError, Describe);
        (await TransferGuard.RunAsync(() => Task.FromResult(7), IsTransferError, Describe)).ShouldBe(7);

        var wrapped = await Should.ThrowAsync<TransferException>(() => TransferGuard.RunAsync(() => throw new IOException("broken pipe"), IsTransferError, Describe));
        wrapped.Message.ShouldBe("described: broken pipe");
        wrapped.InnerException.ShouldBeOfType<IOException>();
        (await Should.ThrowAsync<TransferException>(() => TransferGuard.RunAsync<int>(() => throw new IOException("x"), IsTransferError, Describe))).Message.ShouldBe("described: x");

        await Should.ThrowAsync<InvalidOperationException>(() => TransferGuard.RunAsync(() => throw new InvalidOperationException(), IsTransferError, Describe));
        await Should.ThrowAsync<InvalidOperationException>(() => TransferGuard.RunAsync<int>(() => throw new InvalidOperationException(), IsTransferError, Describe));
    }
}
