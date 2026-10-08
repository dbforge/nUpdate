using System.Globalization;
using System.Windows.Forms;
using nUpdate.Ui;
using nUpdate.Updating;

namespace nUpdate.UI.WindowsForms.Dialogs;

/// <summary>Shows the download progress; cancelling closes the dialog and the download.</summary>
internal sealed partial class UpdateDownloadDialog : BaseDialog
{
    private readonly Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task> _download;
    private readonly DialogOperation<bool> _operation = new();

    internal UpdateDownloadDialog(UpdateManager updateManager, Func<IProgress<UpdateDownloadProgress>, CancellationToken, Task> download)
        : base(updateManager)
    {
        _download = download ?? throw new ArgumentNullException(nameof(download));
        InitializeComponent();
        Disposed += (_, _) => _operation.Dispose();
    }

    /// <summary>Completes once the dialog has closed; cancelled or faulted like the download.</summary>
    internal Task Completion => _operation.Completion;

    private void cancelButton_Click(object sender, EventArgs e) => _operation.Cancel();

    private void UpdateDownloadDialog_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing && !_operation.TryClose())
            e.Cancel = true;
    }

    private void UpdateDownloadDialog_Load(object sender, EventArgs e)
    {
        headerLabel.Text = Localization.Downloading;
        cancelButton.Text = Localization.Cancel;
        ShowProgress(0);
    }

    private async void UpdateDownloadDialog_Shown(object sender, EventArgs e)
    {
        var progress = new Progress<UpdateDownloadProgress>(value => ShowProgress(value.Percentage));
        await _operation.RunAsync(async token =>
        {
            await _download(progress, token);
            return true;
        });
        DialogResult = _operation.Succeeded ? DialogResult.OK : DialogResult.Cancel;
        Close();
    }

    private void ShowProgress(float percentage)
    {
        downloadProgressBar.Value = Math.Max(downloadProgressBar.Minimum, Math.Min(downloadProgressBar.Maximum, (int)percentage));
        infoLabel.Text = string.Format(CultureInfo.CurrentCulture, Localization.DownloadingInfo, Math.Round(percentage, 1));
    }
}
