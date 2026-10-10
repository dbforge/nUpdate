using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using nUpdate;
using nUpdate.Ui;
using nUpdate.UI.Avalonia;
using nUpdate.Updating;

namespace Aurora;

public partial class MainWindow : Window
{
    private readonly AuroraSettings _settings = AuroraSettings.Load();

    public MainWindow()
    {
        InitializeComponent();
        Title = $"Aurora {CurrentVersion}";
        VersionText.Text = CurrentVersion;
        FeedUrlBox.Text = _settings.FeedUrl;
        PublicKeyBox.Text = _settings.PublicKey;
        StabilityBox.ItemsSource = Enum.GetValues<Stability>();
        StabilityBox.SelectedItem = _settings.MinimumStability;
        CheckOnStartBox.IsChecked = _settings.CheckOnStart;
        ShowDiagnostics();
        Opened += async (_, _) =>
        {
            if (_settings.CheckOnStart)
                await CheckAsync(hiddenSearch: true);
        };
        Closing += (_, _) => SaveSettings();
    }

    /// <summary>The version this build declares with <see cref="ApplicationVersionAttribute" /> (see Aurora.csproj).</summary>
    private static string CurrentVersion =>
        typeof(MainWindow).Assembly.GetCustomAttribute<ApplicationVersionAttribute>()?.Version ?? "unknown";

    private async void OnCheckClicked(object? sender, RoutedEventArgs e) =>
        await CheckAsync(HiddenSearchBox.IsChecked == true);

    private async Task CheckAsync(bool hiddenSearch)
    {
        SaveSettings();
        if (!TryCreateManager(out var manager, out var problem))
        {
            ResultText.Text = problem;
            return;
        }

        CheckButton.IsEnabled = false;
        ResultText.Text = "Checking…";
        try
        {
            using (manager)
            {
                var ui = new UpdaterUI(manager, this) { UseHiddenSearch = hiddenSearch };
                var result = await ui.RunAsync();
                ResultText.Text = Describe(result, manager);
            }
        }
        catch (Exception ex)
        {
            // A sample shows what went wrong instead of closing.
            ResultText.Text = $"{ex.GetType().Name}: {ex.Message}";
        }
        finally
        {
            CheckButton.IsEnabled = true;
            ShowDiagnostics();
        }
    }

    private bool TryCreateManager(out UpdateManager manager, out string problem)
    {
        manager = null!;
        if (!Uri.TryCreate(_settings.FeedUrl.Trim(), UriKind.Absolute, out var feed))
        {
            problem = "Enter the feed URL, for example http://localhost/aurora/nupdate.json.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(_settings.PublicKey))
        {
            problem = "Paste the public key of the project from nUpdate Administration.";
            return false;
        }

        manager = new UpdateManager(feed, _settings.PublicKey.Trim(), CultureInfo.CurrentUICulture)
        {
            ApplicationName = "Aurora",
            MinimumStability = _settings.MinimumStability,
        };
        problem = string.Empty;
        return true;
    }

    private static string Describe(UpdateFlowResult result, UpdateManager manager) => result switch
    {
        UpdateFlowResult.NoUpdates => $"Aurora {manager.CurrentVersion} is up to date.",
        UpdateFlowResult.Cancelled => "The search or the download was cancelled.",
        UpdateFlowResult.Declined => $"{manager.AvailableUpdates.Count} update(s) found, not installed.",
        UpdateFlowResult.InsufficientDiskSpace => "There is not enough disk space for the updates.",
        UpdateFlowResult.InvalidSignature =>
            "A package did not carry a valid signature and was deleted. Does the public key belong to the project?",
        UpdateFlowResult.ElevationDeclined => "The installer was not allowed to run as administrator.",
        UpdateFlowResult.InstallerStarted => "The installer started; Aurora closes now.",
        UpdateFlowResult.AlreadyRunning => "A check is still running.",
        _ => "The update failed; the dialog showed why.",
    };

    private void SaveSettings()
    {
        _settings.FeedUrl = FeedUrlBox.Text?.Trim() ?? string.Empty;
        _settings.PublicKey = PublicKeyBox.Text?.Trim() ?? string.Empty;
        _settings.MinimumStability = StabilityBox.SelectedItem is Stability stability ? stability : Stability.Release;
        _settings.CheckOnStart = CheckOnStartBox.IsChecked == true;
        try
        {
            _settings.Save();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            ResultText.Text = $"The settings could not be saved: {ex.Message}";
        }
    }

    private void ShowDiagnostics()
    {
        ExecutableText.Text = Environment.ProcessPath ?? "unknown";
        PlatformText.Text = RuntimeInformation.RuntimeIdentifier;
        SettingsText.Text = AuroraSettings.FilePath;
        if (TryCreateManager(out var manager, out _))
        {
            using (manager)
            {
                var installer = manager.InstallerPath;
                InstallerText.Text = installer is null ? "unknown"
                    : File.Exists(installer) ? installer
                    : $"{installer} (missing: publish Aurora with publish.sh)";
            }
        }
        else
        {
            InstallerText.Text = "shown once the feed URL and the public key are set";
        }
    }
}
