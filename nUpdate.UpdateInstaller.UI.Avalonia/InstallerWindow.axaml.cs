using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace nUpdate.UpdateInstaller.UI.Avalonia;

/// <summary>The built-in installer window. It cannot be closed while the update runs.</summary>
public partial class InstallerWindow : Window
{
    /// <summary>The nUpdate blue, used when the application asks for no accent color.</summary>
    public static readonly Color DefaultAccent = Color.Parse("#1B4F9C");

    public InstallerWindow()
    {
        InitializeComponent();
        ApplyAccent(DefaultAccent);
    }

    public InstallerWindow(InstallerWindowViewModel viewModel)
        : this()
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        DataContext = viewModel;
        var icon = LoadCustomIcon(viewModel.IconPath);
        IconImage.Source = icon ?? LoadDefaultIcon();
        if (icon is not null)
            Icon = new WindowIcon(icon); // the title bar and the task bar show the application's icon as well
        if (viewModel.AccentColor is { } accent)
            ApplyAccent(accent);
        viewModel.CloseRequested += (_, _) => Close();
        Closing += (_, e) => e.Cancel = !viewModel.CanClose;
    }

    /// <summary>Colors the progress bar and the accent buttons (Fluent's resources for them) in the accent.</summary>
    private void ApplyAccent(Color accent)
    {
        Resources["InstallerAccentBrush"] = new SolidColorBrush(accent);
        Resources["AccentButtonBackground"] = new SolidColorBrush(accent);
        Resources["AccentButtonBackgroundPointerOver"] = new SolidColorBrush(accent, 0.9);
        Resources["AccentButtonBackgroundPressed"] = new SolidColorBrush(accent, 0.8);
    }

    /// <summary>The application's PNG, or <c>null</c> when there is none or it cannot be read.</summary>
    internal static Bitmap? LoadCustomIcon(string? path)
    {
        if (path is null)
            return null;
        try
        {
            using var stream = File.OpenRead(path);
            return new Bitmap(stream);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException or InvalidOperationException)
        {
            return null;
        }
    }

    internal static Bitmap LoadDefaultIcon()
    {
        using var stream = AssetLoader.Open(new Uri("avares://nUpdate.UpdateInstaller.UI.Avalonia/Assets/nUpdate.png"));
        return new Bitmap(stream);
    }
}
