using nUpdate.Updating;

namespace nUpdate.Ui;

/// <summary>
///     The dialogs an <see cref="UpdateFlow" /> shows. Every member is called on the thread that started the flow, so a
///     UI framework implements it with its own windows and lets the flow drive the <see cref="UpdateManager" />.
/// </summary>
public interface IUpdateFlowPresenter
{
    /// <summary>
    ///     Shows the search dialog while <paramref name="search" /> runs. Cancelling the dialog cancels the token passed to
    ///     the search, which then throws <see cref="OperationCanceledException" />.
    /// </summary>
    /// <returns>Whether updates were found.</returns>
    Task<bool> RunSearchAsync(Func<CancellationToken, Task<bool>> search);

    /// <summary>Tells the user that the application is up to date.</summary>
    Task ShowNoUpdatesAsync();

    /// <summary>Shows the found updates with their changelog and asks whether to install them.</summary>
    /// <returns><c>true</c> when the user chose to install.</returns>
    Task<bool> ConfirmInstallAsync();

    /// <summary>
    ///     Shows the download dialog while <paramref name="download" /> runs, forwarding its progress. Cancelling the dialog
    ///     cancels the token passed to the download.
    /// </summary>
    Task RunDownloadAsync(Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task> download);

    /// <summary>Shows an error to the user.</summary>
    /// <param name="message">The caption and text.</param>
    /// <param name="exception">The underlying exception, if any, for a details view.</param>
    Task ShowErrorAsync(UpdateErrorMessage message, Exception? exception);
}
