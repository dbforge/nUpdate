using Avalonia.Controls;
using Avalonia.Media;
using nUpdate.UI.Avalonia.ViewModels;

namespace nUpdate.UI.Avalonia.Views;

/// <summary>Hosts one dialog of the update process; its view model decides when it closes.</summary>
public partial class UpdateDialog : Window
{
    /// <summary>The nUpdate blue of the installer window, used when the application set no accent color.</summary>
    public static readonly Color DefaultAccent = Color.Parse("#1B4F9C");

    public UpdateDialog()
    {
        InitializeComponent();
        ApplyAccent(DefaultAccent);
    }

    public UpdateDialog(DialogViewModel viewModel)
        : this()
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        DataContext = viewModel;
        if (Color.TryParse(viewModel.UpdateManager.InstallerAccentColor, out var accent))
            ApplyAccent(accent); // the dialogs and the installer window share the application's accent
        viewModel.CloseRequested += (_, accepted) =>
        {
            Accepted = accepted;
            Close(accepted);
        };
        Opened += async (_, _) => await viewModel.OnOpenedAsync();
        // A dialog with a running search or download stays open until the work has stopped; closing cancels it.
        Closing += (_, e) => e.Cancel = !viewModel.OnClosing();
    }

    /// <summary>Colors the progress bars and the accent buttons (Fluent's resources for them) in the accent.</summary>
    private void ApplyAccent(Color accent)
    {
        Resources["UpdateAccentBrush"] = new SolidColorBrush(accent);
        Resources["AccentButtonBackground"] = new SolidColorBrush(accent);
        Resources["AccentButtonBackgroundPointerOver"] = new SolidColorBrush(accent, 0.9);
        Resources["AccentButtonBackgroundPressed"] = new SolidColorBrush(accent, 0.8);
    }

    /// <summary>Whether the view model accepted the dialog; <c>null</c> while it is open or when the user closed it.</summary>
    public bool? Accepted { get; private set; }
}
