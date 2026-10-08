using System.Drawing;
using System.Windows.Forms;
using nUpdate.Ui;
using nUpdate.UI.WindowsForms.Dialogs;
using nUpdate.UI.WindowsForms.Popups;
using nUpdate.Updating;

namespace nUpdate.UI.WindowsForms;

/// <summary>
///     Runs the complete update process with the built-in Windows Forms dialogs. Create and use it on the UI thread.
/// </summary>
public sealed class UpdaterUI
{
    private readonly UpdateFlow _flow;

    /// <param name="updateManager">The configured update manager.</param>
    /// <param name="owner">The window the dialogs are centred on; <c>null</c> uses the active form.</param>
    public UpdaterUI(UpdateManager updateManager, IWin32Window? owner = null)
    {
        if (updateManager is null)
            throw new ArgumentNullException(nameof(updateManager));
        _flow = new UpdateFlow(updateManager, new WinFormsPresenter(updateManager, owner));
    }

    /// <summary>Searches in the background and shows nothing when the application is up to date.</summary>
    public bool UseHiddenSearch
    {
        get => _flow.UseHiddenSearch;
        set => _flow.UseHiddenSearch = value;
    }

    /// <summary>Starts the update process and shows the dialogs for every step.</summary>
    public Task<UpdateFlowResult> RunAsync(CancellationToken cancellationToken = default) => _flow.RunAsync(cancellationToken);

    private sealed class WinFormsPresenter(UpdateManager updateManager, IWin32Window? owner) : IUpdateFlowPresenter
    {
        private IWin32Window? Owner => owner ?? Form.ActiveForm;

        public async Task<bool> RunSearchAsync(Func<CancellationToken, Task<bool>> search)
        {
            using var dialog = new UpdateSearchDialog(updateManager, search);
            dialog.ShowDialog(Owner);
            return await dialog.Completion;
        }

        public Task ShowNoUpdatesAsync()
        {
            using var dialog = new NoUpdateFoundDialog(updateManager);
            dialog.ShowDialog(Owner);
            return Task.CompletedTask;
        }

        public Task<bool> ConfirmInstallAsync()
        {
            using var dialog = new NewUpdateDialog(updateManager);
            return Task.FromResult(dialog.ShowDialog(Owner) == DialogResult.OK);
        }

        public async Task RunDownloadAsync(Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task> download)
        {
            using var dialog = new UpdateDownloadDialog(updateManager, download);
            dialog.ShowDialog(Owner);
            await dialog.Completion;
        }

        public Task ShowErrorAsync(UpdateErrorMessage message, Exception? exception)
        {
            Popup.Show(Owner, SystemIcons.Error, message.Caption, message.Text, exception);
            return Task.CompletedTask;
        }
    }
}
