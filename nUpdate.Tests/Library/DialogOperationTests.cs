using nUpdate.Ui;

namespace nUpdate.Tests.Library;

public class DialogOperationTests
{
    [Fact]
    public async Task Run_RecordsTheResult()
    {
        using var operation = new DialogOperation<int>();
        operation.IsCompleted.ShouldBeFalse();
        await operation.RunAsync(_ => Task.FromResult(42));
        operation.IsCompleted.ShouldBeTrue();
        operation.Succeeded.ShouldBeTrue();
        (await operation.Completion).ShouldBe(42);
        operation.TryClose().ShouldBeTrue();
        operation.Cancel(); // no effect after completion
    }

    [Fact]
    public async Task Run_RecordsFailures()
    {
        using var operation = new DialogOperation<int>();
        await operation.RunAsync(_ => throw new InvalidOperationException("boom"));
        operation.Succeeded.ShouldBeFalse();
        (await Should.ThrowAsync<InvalidOperationException>(() => operation.Completion)).Message.ShouldBe("boom");
    }

    [Fact]
    public async Task TryClose_CancelsARunningOperation()
    {
        using var operation = new DialogOperation<int>();
        var run = operation.RunAsync(async token =>
        {
            await Task.Delay(Timeout.Infinite, token);
            return 1;
        });
        operation.TryClose().ShouldBeFalse();
        await run;
        operation.Completion.IsCanceled.ShouldBeTrue();
        await Should.ThrowAsync<OperationCanceledException>(() => operation.Completion);
    }

    [Fact]
    public async Task Run_RequiresAnOperation()
    {
        using var operation = new DialogOperation<int>();
        await Should.ThrowAsync<ArgumentNullException>(() => operation.RunAsync(null!));
    }
}
