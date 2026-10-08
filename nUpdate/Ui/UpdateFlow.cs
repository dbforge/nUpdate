using System.IO.Abstractions;
using nUpdate.Updating;

namespace nUpdate.Ui;

/// <summary>
///     The complete interactive update process: search, confirmation, download, validation and installer start. The
///     built-in Windows Forms and WPF user interfaces only provide the dialogs through an <see cref="IUpdateFlowPresenter" />.
///     Continuations deliberately stay on the caller's synchronization context (<c>ConfigureAwait(true)</c>) because
///     the presenter shows windows.
/// </summary>
public sealed class UpdateFlow
{
    private readonly UpdateManager _manager;
    private readonly IUpdateFlowPresenter _presenter;
    private readonly IFileSystem _fileSystem;
    private int _running;

    /// <param name="manager">The configured update manager.</param>
    /// <param name="presenter">The dialogs of the hosting UI framework.</param>
    /// <param name="fileSystem">The file system for the disk-space check; defaults to the one the manager uses.</param>
    public UpdateFlow(UpdateManager manager, IUpdateFlowPresenter presenter, IFileSystem? fileSystem = null)
    {
        _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
        _fileSystem = fileSystem ?? manager.FileSystem;
    }

    /// <summary>Searches without a dialog and shows nothing when the application is up to date.</summary>
    public bool UseHiddenSearch { get; set; }

    /// <summary>Whether a run is in progress.</summary>
    public bool IsRunning => _running == 1;

    /// <summary>Runs the whole process once. Every dialog is shown through the presenter on the calling thread.</summary>
    public async Task<UpdateFlowResult> RunAsync(CancellationToken cancellationToken = default)
    {
        if (Interlocked.CompareExchange(ref _running, 1, 0) == 1)
            return UpdateFlowResult.AlreadyRunning;

        try
        {
            return await RunCoreAsync(cancellationToken).ConfigureAwait(true);
        }
        finally
        {
            Interlocked.Exchange(ref _running, 0);
        }
    }

    private async Task<UpdateFlowResult> RunCoreAsync(CancellationToken cancellationToken)
    {
        var texts = _manager.Texts;

        bool found;
        try
        {
            found = UseHiddenSearch
                ? await _manager.CheckForUpdatesAsync(cancellationToken).ConfigureAwait(true)
                : await _presenter.RunSearchAsync(token => SearchAsync(token, cancellationToken)).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            return UpdateFlowResult.Cancelled;
        }
        catch (Exception exception)
        {
            await _presenter.ShowErrorAsync(UpdateErrorMessages.ForSearch(exception, texts), exception).ConfigureAwait(true);
            return UpdateFlowResult.Failed;
        }

        if (!found)
        {
            if (!UseHiddenSearch)
                await _presenter.ShowNoUpdatesAsync().ConfigureAwait(true);
            return UpdateFlowResult.NoUpdates;
        }

        if (!await _presenter.ConfirmInstallAsync().ConfigureAwait(true))
            return UpdateFlowResult.Declined;

        if (!UpdateSizeCheck.HasEnoughSpace(_fileSystem, _manager.TotalDownloadSize, out var bytesToFree))
        {
            await _presenter.ShowErrorAsync(UpdateErrorMessages.ForInsufficientDiskSpace(_manager.TotalDownloadSize, bytesToFree, texts), null).ConfigureAwait(true);
            return UpdateFlowResult.InsufficientDiskSpace;
        }

        try
        {
            await _presenter.RunDownloadAsync((progress, token) => DownloadAsync(progress, token, cancellationToken)).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            return UpdateFlowResult.Cancelled;
        }
        catch (Exception exception)
        {
            await _presenter.ShowErrorAsync(UpdateErrorMessages.ForDownload(exception, texts), exception).ConfigureAwait(true);
            return UpdateFlowResult.Failed;
        }

        bool valid;
        try
        {
            valid = await _manager.VerifyAsync(cancellationToken).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            return UpdateFlowResult.Cancelled;
        }
        catch (Exception exception)
        {
            await _presenter.ShowErrorAsync(UpdateErrorMessages.ForVerification(exception, texts), exception).ConfigureAwait(true);
            return UpdateFlowResult.Failed;
        }

        if (!valid)
        {
            await _presenter.ShowErrorAsync(UpdateErrorMessages.ForInvalidSignature(texts), null).ConfigureAwait(true);
            return UpdateFlowResult.InvalidSignature;
        }

        bool started;
        try
        {
            started = _manager.StartInstaller();
        }
        catch (Exception exception)
        {
            await _presenter.ShowErrorAsync(UpdateErrorMessages.ForInstall(exception, texts), exception).ConfigureAwait(true);
            return UpdateFlowResult.Failed;
        }

        return started ? UpdateFlowResult.InstallerStarted : UpdateFlowResult.ElevationDeclined;
    }

    private async Task<bool> SearchAsync(CancellationToken dialogToken, CancellationToken outerToken)
    {
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(dialogToken, outerToken);
        return await _manager.CheckForUpdatesAsync(linked.Token).ConfigureAwait(false);
    }

    private async Task DownloadAsync(IProgress<UpdateDownloadProgress> progress, CancellationToken dialogToken, CancellationToken outerToken)
    {
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(dialogToken, outerToken);
        await _manager.DownloadAsync(progress, linked.Token).ConfigureAwait(false);
    }
}
