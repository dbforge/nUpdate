using nUpdate.Administration.Core.Publishing;
using nUpdate.Tests.Support;

namespace nUpdate.Tests.Administration.Core;

public class CompensatingPipelineTests
{
    [Fact]
    public async Task Run_ExecutesStepsInOrderAndReportsProgress()
    {
        var log = new List<string>();
        var progress = new List<PipelineProgress>();
        var pipeline = new CompensatingPipeline()
            .Add("one", _ => { log.Add("one"); return Task.CompletedTask; }, () => { log.Add("undo one"); return Task.CompletedTask; })
            .Add("two", _ => { log.Add("two"); return Task.CompletedTask; });
        pipeline.Steps.Count.ShouldBe(2);

        await pipeline.RunAsync(new SyncProgress<PipelineProgress>(progress));

        log.ShouldBe(["one", "two"]);
        progress.Select(p => $"{p.StepName}:{p.StepIndex}/{p.StepCount}").ShouldBe(["one:0/2", "two:1/2", "Done:2/2"]);
        progress[1].Percentage.ShouldBe(50);
        new PipelineProgress("x", 0, 0).Percentage.ShouldBe(100);
        Should.Throw<ArgumentNullException>(() => new PipelineProgress(null!, 0, 1));
    }

    [Fact]
    public async Task Run_CompensatesCompletedStepsInReverseOrder()
    {
        var log = new List<string>();
        var pipeline = new CompensatingPipeline()
            .Add("one", _ => { log.Add("one"); return Task.CompletedTask; }, () => { log.Add("undo one"); return Task.CompletedTask; })
            .Add("two", _ => { log.Add("two"); return Task.CompletedTask; }, () => { log.Add("undo two"); throw new InvalidOperationException("undo failed"); })
            .Add("three", _ => { log.Add("three"); return Task.CompletedTask; })
            .Add("four", _ => throw new IOException("boom"), () => { log.Add("never"); return Task.CompletedTask; })
            .Add("five", _ => { log.Add("five"); return Task.CompletedTask; });

        var ex = await Should.ThrowAsync<PipelineException>(() => pipeline.RunAsync());

        ex.FailedStep.ShouldBe("four");
        ex.InnerException.ShouldBeOfType<IOException>();
        ex.Message.ShouldContain("four");
        ex.Message.ShouldContain("boom");
        ex.CompensationErrors.Single().Message.ShouldBe("undo failed");
        log.ShouldBe(["one", "two", "three", "undo two", "undo one"]);
    }

    [Fact]
    public async Task Run_LetsAStepCleanUpAfterItsOwnFailure()
    {
        var log = new List<string>();
        var pipeline = new CompensatingPipeline()
            .Add("one", _ => { log.Add("one"); return Task.CompletedTask; }, () => { log.Add("undo one"); return Task.CompletedTask; })
            .Add("two halves", _ => { log.Add("first half"); throw new IOException("second half failed"); }, () => { log.Add("undo two"); return Task.CompletedTask; }, compensatesOwnFailure: true);
        pipeline.Steps[1].CompensatesOwnFailure.ShouldBeTrue();
        pipeline.Steps[0].CompensatesOwnFailure.ShouldBeFalse();

        (await Should.ThrowAsync<PipelineException>(() => pipeline.RunAsync())).FailedStep.ShouldBe("two halves");

        log.ShouldBe(["one", "first half", "undo two", "undo one"]);
    }

    [Fact]
    public async Task Run_ToleratesCompletedStepsWithoutCompensationAndAwaitsSlowCompensations()
    {
        var undone = false;
        var pipeline = new CompensatingPipeline()
            .Add("prepare", _ => Task.CompletedTask)
            .Add("slow", _ => Task.CompletedTask, async () =>
            {
                await Task.Delay(1);
                undone = true;
            })
            .Add("fail", _ => throw new IOException("boom"));

        var ex = await Should.ThrowAsync<PipelineException>(() => pipeline.RunAsync());

        ex.FailedStep.ShouldBe("fail");
        ex.CompensationErrors.ShouldBeEmpty();
        undone.ShouldBeTrue();
    }

    [Fact]
    public async Task Run_StopsOnCancellation()
    {
        var ran = false;
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var pipeline = new CompensatingPipeline().Add("one", _ => { ran = true; return Task.CompletedTask; });
        var ex = await Should.ThrowAsync<PipelineException>(() => pipeline.RunAsync(cancellationToken: cts.Token));
        ex.InnerException.ShouldBeOfType<OperationCanceledException>();
        ran.ShouldBeFalse();
    }

    [Fact]
    public void Add_ValidatesArguments()
    {
        var pipeline = new CompensatingPipeline();
        Should.Throw<ArgumentException>(() => pipeline.Add(" ", _ => Task.CompletedTask));
        Should.Throw<ArgumentNullException>(() => pipeline.Add("x", null!));
        var inner = new InvalidOperationException();
        new PipelineException().CompensationErrors.ShouldBeEmpty();
        new PipelineException("m").Message.ShouldBe("m");
        new PipelineException("m", inner).InnerException.ShouldBe(inner);
        new PipelineStep("n", _ => Task.CompletedTask, null).Compensate.ShouldBeNull();
    }
}
