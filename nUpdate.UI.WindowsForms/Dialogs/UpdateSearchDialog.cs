using System.Windows.Forms;
using nUpdate.Ui;
using nUpdate.Updating;

namespace nUpdate.UI.WindowsForms.Dialogs;

/// <summary>Shows an indeterminate progress bar while the search runs; cancelling closes the dialog and the search.</summary>
internal sealed partial class UpdateSearchDialog : BaseDialog
{
    private readonly Func<CancellationToken, Task<bool>> _search;
    private readonly DialogOperation<bool> _operation = new();

    internal UpdateSearchDialog(UpdateManager updateManager, Func<CancellationToken, Task<bool>> search)
        : base(updateManager)
    {
        _search = search ?? throw new ArgumentNullException(nameof(search));
        InitializeComponent();
        Disposed += (_, _) => _operation.Dispose();
    }

    /// <summary>Completes with the search result once the dialog has closed; cancelled or faulted like the search.</summary>
    internal Task<bool> Completion => _operation.Completion;

    private void cancelButton_Click(object sender, EventArgs e) => _operation.Cancel();

    private void SearchDialog_Load(object sender, EventArgs e)
    {
        cancelButton.Text = Localization.Cancel;
        headerLabel.Text = Localization.Searching;
    }

    private void UpdateSearchDialog_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing && !_operation.TryClose())
            e.Cancel = true;
    }

    private async void UpdateSearchDialog_Shown(object sender, EventArgs e)
    {
        await _operation.RunAsync(_search);
        DialogResult = _operation.Succeeded ? DialogResult.OK : DialogResult.Cancel;
        Close();
    }
}
