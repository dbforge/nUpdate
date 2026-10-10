using Avalonia.Controls;
using nUpdate.Ui;
using nUpdate.UI.Avalonia.ViewModels;
using nUpdate.UI.Avalonia.Views;
using nUpdate.Updating;

namespace nUpdate.UI.Avalonia;

/// <summary>
///     Runs the complete update process with the built-in Avalonia dialogs on Windows, Linux and macOS. Create and use it
///     on the UI thread.
/// </summary>
public sealed class UpdaterUI
{
    private readonly UpdateFlow _flow;

    /// <param name="updateManager">The configured update manager.</param>
    /// <param name="owner">The window that owns the dialogs, which are then modal to it; <c>null</c> shows them on their own.</param>
    public UpdaterUI(UpdateManager updateManager, Window? owner = null)
    {
        ArgumentNullException.ThrowIfNull(updateManager);
        Presenter = new AvaloniaPresenter(updateManager, owner);
        _flow = new UpdateFlow(updateManager, Presenter);
    }

    internal AvaloniaPresenter Presenter { get; }

    /// <summary>Searches in the background and shows nothing when the application is up to date.</summary>
    public bool UseHiddenSearch
    {
        get => _flow.UseHiddenSearch;
        set => _flow.UseHiddenSearch = value;
    }

    /// <summary>Starts the update process and shows the dialogs for every step.</summary>
    public Task<UpdateFlowResult> RunAsync(CancellationToken cancellationToken = default) =>
        _flow.RunAsync(cancellationToken);

    /// <summary>Shows the dialogs of <see cref="UpdateFlow" /> as Avalonia windows.</summary>
    internal sealed class AvaloniaPresenter(UpdateManager updateManager, Window? owner) : IUpdateFlowPresenter
    {
        /// <summary>Called with every dialog right after it is shown.</summary>
        public Action<UpdateDialog>? DialogShown { get; set; }

        public async Task<bool> RunSearchAsync(Func<CancellationToken, Task<bool>> search)
        {
            using var viewModel = new SearchDialogViewModel(updateManager, search);
            await ShowAsync(viewModel);
            return await viewModel.Completion;
        }

        public Task ShowNoUpdatesAsync() =>
            ShowAsync(new MessageDialogViewModel(updateManager, updateManager.Texts.NoUpdatesTitle,
                updateManager.Texts.NoUpdatesInfo));

        public async Task<bool> ConfirmInstallAsync() =>
            await ShowAsync(new NewUpdateDialogViewModel(updateManager)) == true;

        public async Task RunDownloadAsync(Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task> download)
        {
            using var viewModel = new DownloadDialogViewModel(updateManager, download);
            await ShowAsync(viewModel);
            await viewModel.Completion;
        }

        public Task ShowErrorAsync(UpdateErrorMessage message, Exception? exception) =>
            ShowAsync(new MessageDialogViewModel(updateManager, message.Caption, message.Text, exception));

        /// <summary>Shows the dialog, modal to the owner when there is one, and returns once it has closed.</summary>
        private async Task<bool?> ShowAsync(DialogViewModel viewModel)
        {
            var dialog = new UpdateDialog(viewModel);
            // The flow shows the next dialog in the continuation, which must not run inside this dialog's Closed event.
            var closed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            dialog.Closed += (_, _) => closed.TrySetResult(true);
            if (owner is not null)
            {
                dialog.Icon = owner.Icon;
                _ = dialog.ShowDialog(owner);
            }
            else
            {
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                dialog.Show();
            }

            DialogShown?.Invoke(dialog);
            await closed.Task;
            return dialog.Accepted;
        }
    }
}
