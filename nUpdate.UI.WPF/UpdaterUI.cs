using System.Windows;
using nUpdate.Ui;
using nUpdate.UI.WPF.ViewModel;
using nUpdate.UI.WPF.Views;
using nUpdate.Updating;

namespace nUpdate.UI.WPF;

/// <summary>
///     Runs the complete update process with the built-in WPF dialogs. Create and use it on the UI thread.
/// </summary>
public sealed class UpdaterUI
{
    private readonly UpdateFlow _flow;

    /// <param name="updateManager">The configured update manager.</param>
    /// <param name="owner">The window that owns the dialogs; <c>null</c> centres them on the screen.</param>
    public UpdaterUI(UpdateManager updateManager, Window? owner = null)
    {
        if (updateManager is null)
            throw new ArgumentNullException(nameof(updateManager));
        _flow = new UpdateFlow(updateManager, new WpfPresenter(updateManager, owner));
    }

    /// <summary>Searches in the background and shows nothing when the application is up to date.</summary>
    public bool UseHiddenSearch
    {
        get => _flow.UseHiddenSearch;
        set => _flow.UseHiddenSearch = value;
    }

    /// <summary>Starts the update process and shows the dialogs for every step.</summary>
    public Task<UpdateFlowResult> RunAsync(CancellationToken cancellationToken = default) =>
        _flow.RunAsync(cancellationToken);

    private sealed class WpfPresenter(UpdateManager updateManager, Window? owner) : IUpdateFlowPresenter
    {
        public async Task<bool> RunSearchAsync(Func<CancellationToken, Task<bool>> search)
        {
            using var viewModel = new UpdateSearchViewModel(updateManager, search);
            Show(viewModel);
            return await viewModel.Completion;
        }

        public Task ShowNoUpdatesAsync()
        {
            ShowMessage(updateManager.Texts.NoUpdatesTitle, updateManager.Texts.NoUpdatesInfo,
                MessageBoxImage.Information);
            return Task.CompletedTask;
        }

        public Task<bool> ConfirmInstallAsync() => Task.FromResult(Show(new ChangelogViewModel(updateManager)) == true);

        public async Task RunDownloadAsync(Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task> download)
        {
            using var viewModel = new DownloadUpdateViewModel(updateManager, download);
            Show(viewModel);
            await viewModel.Completion;
        }

        public Task ShowErrorAsync(UpdateErrorMessage message, Exception? exception)
        {
            ShowMessage(message.Caption, message.Text, MessageBoxImage.Error);
            return Task.CompletedTask;
        }

        private bool? Show(DialogViewModel viewModel) => new DialogWindow(viewModel, owner).ShowDialog();

        private void ShowMessage(string caption, string text, MessageBoxImage image)
        {
            if (owner is null)
                MessageBox.Show(text, caption, MessageBoxButton.OK, image);
            else
                MessageBox.Show(owner, text, caption, MessageBoxButton.OK, image);
        }
    }
}
