using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace nUpdate.Administration.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
}

/// <summary>A view model shown in its own window that can be accepted or cancelled.</summary>
public abstract partial class DialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsIdle))]
    [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
    private bool _isBusy;

    [ObservableProperty]
    private string _busyText = string.Empty;

    [ObservableProperty]
    private double _progress;

    [ObservableProperty]
    private string? _errorMessage;

    /// <summary>Raised with <c>true</c> when the dialog was accepted, <c>false</c> when cancelled.</summary>
    public event EventHandler<bool>? CloseRequested;

    public bool IsIdle => !IsBusy;

    /// <summary>The exception behind <see cref="ErrorMessage" />, for callers that react to specific failures.</summary>
    public Exception? LastError { get; private set; }

    [RelayCommand(CanExecute = nameof(IsIdle))]
    protected void Cancel() => Close(false);

    protected void Close(bool accepted) => CloseRequested?.Invoke(this, accepted);

    /// <summary>Runs an operation with the busy indicator and shows failures inline instead of throwing.</summary>
    protected async Task<bool> RunBusyAsync(string busyText, Func<IProgress<Core.Publishing.PipelineProgress>, Task> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        IsBusy = true;
        BusyText = busyText;
        ErrorMessage = null;
        LastError = null;
        Progress = 0;
        try
        {
            var progress = new Progress<Core.Publishing.PipelineProgress>(p =>
            {
                BusyText = p.StepName;
                Progress = p.Percentage;
            });
            await action(progress);
            return true;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            LastError = ex;
            ErrorMessage = Describe(ex);
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    ///     Like <see cref="RunBusyAsync" />; when the server presents a certificate or host key that is not trusted yet,
    ///     asks whether to trust it, stores the fingerprint in the transfer settings and runs the operation again.
    /// </summary>
    protected async Task<bool> RunTrustingAsync(Services.IDialogService dialogs, TransferInterface.TransferSettings transfer, string busyText, Func<IProgress<Core.Publishing.PipelineProgress>, Task> action)
    {
        ArgumentNullException.ThrowIfNull(dialogs);
        ArgumentNullException.ThrowIfNull(transfer);
        if (await RunBusyAsync(busyText, action))
            return true;
        var untrusted = LastError switch
        {
            Core.Publishing.PipelineException { InnerException: TransferInterface.UntrustedServerException inner } => inner,
            TransferInterface.UntrustedServerException direct => direct,
            _ => null,
        };
        if (untrusted?.Fingerprint is null)
            return false;

        var isSftp = transfer.Protocol == TransferInterface.TransferProtocol.Sftp;
        var what = isSftp ? "host key" : "certificate";
        if (!await dialogs.ConfirmAsync($"Unknown {what}", $"{untrusted.Message}{Environment.NewLine}{Environment.NewLine}{untrusted.Subject}{Environment.NewLine}SHA-256: {untrusted.Fingerprint}{Environment.NewLine}{Environment.NewLine}Trust this {what} from now on?", "Trust"))
            return false;
        if (isSftp)
            transfer.TrustedHostKeyFingerprint = untrusted.Fingerprint;
        else
            transfer.TrustedCertificateFingerprint = untrusted.Fingerprint;
        return await RunBusyAsync(busyText, action);
    }

    /// <summary>A user-facing description: the step that failed, its cause, and any rollback problems.</summary>
    public static string Describe(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        if (exception is Core.Publishing.PipelineException pipeline)
        {
            var text = pipeline.Message;
            if (pipeline.CompensationErrors.Count > 0)
                text += Environment.NewLine + "Rolling back failed too: " + string.Join("; ", pipeline.CompensationErrors.Select(e => e.Message));
            return text;
        }

        // Services report invalid input as ArgumentException; the parameter name is for developers, not the user.
        if (exception is ArgumentException { ParamName: { } parameter } argument)
            return argument.Message.Replace($" (Parameter '{parameter}')", string.Empty, StringComparison.Ordinal);
        return exception.Message;
    }
}
