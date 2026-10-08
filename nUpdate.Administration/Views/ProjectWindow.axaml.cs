using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Media;
using nUpdate.Administration.ViewModels;

namespace nUpdate.Administration.Views;

public partial class ProjectWindow : Window
{
    private ProjectViewModel? _viewModel;

    public ProjectWindow()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => Attach();
        ActualThemeVariantChanged += (_, _) => DrawStatistics();
        Opened += async (_, _) =>
        {
            if (_viewModel is not null)
                await _viewModel.OnOpenedAsync();
        };
    }

    private void Attach()
    {
        if (_viewModel is not null)
            _viewModel.VersionStatistics.CollectionChanged -= OnStatisticsChanged;
        _viewModel = DataContext as ProjectViewModel;
        if (_viewModel is not null)
            _viewModel.VersionStatistics.CollectionChanged += OnStatisticsChanged;
    }

    private void OnStatisticsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Reset)
            DrawStatistics();
    }

    [ExcludeFromCodeCoverage] // Draws with ScottPlot; checked visually.
    private void DrawStatistics()
    {
        if (_viewModel is null)
            return;
        var plot = DownloadsPlot.Plot;
        plot.Clear();
        plot.FigureBackground.Color = ScottPlot.Colors.Transparent;
        plot.DataBackground.Color = ScottPlot.Colors.Transparent;
        plot.Axes.Color(ThemeColor("MutedBrush"));
        plot.Grid.MajorLineColor = ThemeColor("HairlineSoftBrush");
        plot.Axes.Top.FrameLineStyle.Width = 0;
        plot.Axes.Right.FrameLineStyle.Width = 0;
        var versions = _viewModel.VersionStatistics.ToList();
        if (versions.Count > 0)
        {
            var bars = plot.Add.Bars(versions.Select(v => (double)v.Downloads).ToArray());
            bars.Color = ThemeColor("AccentBrush");
            foreach (var bar in bars.Bars)
                bar.LineWidth = 0;
            bars.ValueLabelStyle.IsVisible = true;
            bars.ValueLabelStyle.ForeColor = ThemeColor("InkSecondaryBrush");
            plot.Axes.Bottom.SetTicks(Enumerable.Range(0, versions.Count).Select(i => (double)i).ToArray(),
                versions.Select(v => v.Version.ToString()).ToArray());
            plot.Axes.Margins(bottom: 0);
        }

        DownloadsPlot.Refresh();
    }

    /// <summary>A brush of the current theme (App.axaml) as a ScottPlot colour, so the chart follows light and dark mode.</summary>
    [ExcludeFromCodeCoverage] // Only called while drawing.
    private ScottPlot.Color ThemeColor(string key)
    {
        var color = this.TryFindResource(key, ActualThemeVariant, out var brush) && brush is ISolidColorBrush solid
            ? solid.Color
            : Colors.Gray;
        return new ScottPlot.Color(color.R, color.G, color.B, color.A);
    }
}
