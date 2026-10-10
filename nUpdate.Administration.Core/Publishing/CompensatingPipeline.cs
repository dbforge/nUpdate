namespace nUpdate.Administration.Core.Publishing;

/// <summary>
///     Runs steps in order. When a step fails, the compensations of the completed steps run in reverse order, and the
///     original exception is rethrown (with compensation failures attached as an aggregate's inner exceptions). A step
///     that does several things (one per platform, for example) can ask for its own compensation to run as well when it
///     fails half-way.
/// </summary>
public sealed class CompensatingPipeline
{
    private readonly List<PipelineStep> _steps = [];

    public IReadOnlyList<PipelineStep> Steps => _steps;

    /// <param name="name">The name shown in the progress and in a <see cref="PipelineException" />.</param>
    /// <param name="execute">The work of the step.</param>
    /// <param name="compensate">Undoes the step, or <c>null</c> when there is nothing to undo.</param>
    /// <param name="compensatesOwnFailure">Whether <paramref name="compensate" /> also runs when this step itself fails, which it then must cope with.</param>
    public CompensatingPipeline Add(string name, Func<CancellationToken, Task> execute, Func<Task>? compensate = null,
        bool compensatesOwnFailure = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(execute);
        _steps.Add(new PipelineStep(name, execute, compensate, compensatesOwnFailure));
        return this;
    }

    /// <exception cref="PipelineException">A step failed; see <see cref="PipelineException.FailedStep" />.</exception>
    public async Task RunAsync(IProgress<PipelineProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var completed = new Stack<PipelineStep>();
        for (var i = 0; i < _steps.Count; i++)
        {
            var step = _steps[i];
            progress?.Report(new PipelineProgress(step.Name, i, _steps.Count));
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                await step.Execute(cancellationToken).ConfigureAwait(false);
                completed.Push(step);
            }
            catch (Exception ex)
            {
                if (step.CompensatesOwnFailure)
                    completed.Push(step);
                var compensationErrors = new List<Exception>();
                while (completed.Count > 0)
                {
                    var done = completed.Pop();
                    if (done.Compensate is null)
                        continue;
                    try
                    {
                        await done.Compensate().ConfigureAwait(false);
                    }
                    catch (Exception compensationError)
                    {
                        compensationErrors.Add(compensationError);
                    }
                }

                throw new PipelineException(step.Name, ex, compensationErrors);
            }
        }

        progress?.Report(new PipelineProgress("Done", _steps.Count, _steps.Count));
    }
}

public sealed class PipelineStep(
    string name,
    Func<CancellationToken, Task> execute,
    Func<Task>? compensate,
    bool compensatesOwnFailure = false)
{
    public string Name { get; } = name;

    public Func<CancellationToken, Task> Execute { get; } = execute;

    public Func<Task>? Compensate { get; } = compensate;

    /// <summary>Whether <see cref="Compensate" /> also runs when this step fails.</summary>
    public bool CompensatesOwnFailure { get; } = compensatesOwnFailure;
}

public sealed class PipelineProgress(string stepName, int stepIndex, int stepCount)
{
    public string StepName { get; } = stepName ?? throw new ArgumentNullException(nameof(stepName));

    public int StepIndex { get; } = stepIndex;

    public int StepCount { get; } = stepCount;

    public double Percentage => StepCount == 0 ? 100 : 100.0 * StepIndex / StepCount;
}

public class PipelineException : Exception
{
    public PipelineException()
    {
    }

    public PipelineException(string message)
        : base(message)
    {
    }

    public PipelineException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public PipelineException(string failedStep, Exception cause, IReadOnlyList<Exception> compensationErrors)
        : base($"The step \"{failedStep}\" failed: {cause.Message}", cause)
    {
        FailedStep = failedStep;
        CompensationErrors = compensationErrors;
    }

    public string? FailedStep { get; }

    /// <summary>Errors that happened while undoing the completed steps; empty when the rollback was clean.</summary>
    public IReadOnlyList<Exception> CompensationErrors { get; } = [];
}
