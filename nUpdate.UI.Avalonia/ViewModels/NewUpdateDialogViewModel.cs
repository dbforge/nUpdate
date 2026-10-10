using System.Windows.Input;
using nUpdate.Ui;
using nUpdate.Updating;

namespace nUpdate.UI.Avalonia.ViewModels;

/// <summary>The found updates with their changelog; the user installs or cancels.</summary>
public sealed class NewUpdateDialogViewModel : DialogViewModel
{
    internal NewUpdateDialogViewModel(UpdateManager updateManager)
        : base(updateManager)
    {
        Summary = new UpdateSummary(updateManager, Environment.NewLine);
        InstallCommand = new RelayCommand(() => RequestClose(true));
        CancelCommand = new RelayCommand(() => RequestClose(false));
    }

    public override string Title => UpdateManager.ApplicationName;

    /// <summary>The texts of the dialog, shared with the other built-in user interfaces.</summary>
    internal UpdateSummary Summary { get; }

    public ICommand InstallCommand { get; }

    public ICommand CancelCommand { get; }
}
