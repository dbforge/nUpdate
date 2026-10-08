namespace nUpdate.Ui;

/// <summary>
///     The state of an operation that runs while a dialog is open: it can be cancelled from the dialog, its outcome is
///     exposed as a task once the dialog has closed, and the dialog may only close when the operation is over.
/// </summary>
/// <typeparam name="T">The result of the operation.</typeparam>
internal sealed class DialogOperation<T> : IDisposable
{
    private readonly CancellationTokenSource _cancellation = new();
    private readonly TaskCompletionSource<T> _completion = new();

    /// <summary>Completes with the result, is cancelled when the user cancelled, or faults with the operation's exception.</summary>
    public Task<T> Completion => _completion.Task;

    public bool IsCompleted => _completion.Task.IsCompleted;

    /// <summary>Whether the operation finished with a result.</summary>
    public bool Succeeded => _completion.Task.Status == TaskStatus.RanToCompletion;

    /// <summary>Runs the operation and records its outcome. Never throws.</summary>
    public async Task RunAsync(Func<CancellationToken, Task<T>> operation)
    {
        if (operation is null)
            throw new ArgumentNullException(nameof(operation));

        try
        {
            _completion.SetResult(await operation(_cancellation.Token).ConfigureAwait(true));
        }
        catch (OperationCanceledException)
        {
            _completion.SetCanceled();
        }
        catch (Exception exception)
        {
            _completion.SetException(exception);
        }
    }

    /// <summary>Asks the operation to stop; it then completes as cancelled.</summary>
    public void Cancel()
    {
        if (!IsCompleted)
            _cancellation.Cancel();
    }

    /// <summary>
    ///     Called when the user wants to close the dialog: returns <c>true</c> when it may close, otherwise cancels the
    ///     operation so the dialog can close once it has stopped.
    /// </summary>
    public bool TryClose()
    {
        if (IsCompleted)
            return true;
        Cancel();
        return false;
    }

    public void Dispose() => _cancellation.Dispose();
}
