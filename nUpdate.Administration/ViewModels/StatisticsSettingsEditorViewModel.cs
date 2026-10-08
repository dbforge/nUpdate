using CommunityToolkit.Mvvm.ComponentModel;
using nUpdate.Administration.Core.Models;

namespace nUpdate.Administration.ViewModels;

/// <summary>Edits the statistics settings of a project.</summary>
public partial class StatisticsSettingsEditorViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _enabled;

    [ObservableProperty]
    private string _databaseHost = "localhost";

    [ObservableProperty]
    private string _databaseName = string.Empty;

    [ObservableProperty]
    private string _databaseUsername = string.Empty;

    [ObservableProperty]
    private string _databasePassword = string.Empty;

    [ObservableProperty]
    private string? _endpointUrl;

    public void Load(StatisticsSettings settings, ProjectSecrets secrets)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(secrets);
        Enabled = settings.Enabled;
        EndpointUrl = settings.EndpointUrl;
        DatabaseHost = settings.Database?.Host ?? "localhost";
        DatabaseName = settings.Database?.Name ?? string.Empty;
        DatabaseUsername = settings.Database?.Username ?? string.Empty;
        DatabasePassword = secrets.StatisticsDatabasePassword ?? string.Empty;
    }

    public StatisticsSettings ToSettings() => new()
    {
        Enabled = Enabled,
        EndpointUrl = string.IsNullOrWhiteSpace(EndpointUrl) ? null : EndpointUrl.Trim(),
        Database = Enabled ? new StatisticsDatabaseSettings { Host = DatabaseHost.Trim(), Name = DatabaseName.Trim(), Username = DatabaseUsername.Trim() } : null,
    };

    public void ApplySecrets(ProjectSecrets secrets)
    {
        ArgumentNullException.ThrowIfNull(secrets);
        secrets.StatisticsDatabasePassword = Enabled && !string.IsNullOrEmpty(DatabasePassword) ? DatabasePassword : null;
    }

    public string? Validate()
    {
        if (!Enabled)
            return null;
        if (string.IsNullOrWhiteSpace(DatabaseHost) || string.IsNullOrWhiteSpace(DatabaseName) || string.IsNullOrWhiteSpace(DatabaseUsername))
            return "Enter the database host, name and user for the statistics.";
        if (!string.IsNullOrWhiteSpace(EndpointUrl) && !UpdateProject.IsValidUpdateUrl(EndpointUrl))
            return "The statistics endpoint must be an absolute HTTP(S) URL, or empty.";
        return null;
    }
}
