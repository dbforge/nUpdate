using Avalonia.Controls;
using nUpdate.Administration.ViewModels;

namespace nUpdate.Administration.Views;

public partial class MigrationWindow : Window
{
    public MigrationWindow()
    {
        InitializeComponent();
        Opened += async (_, _) =>
        {
            if (DataContext is MigrationViewModel viewModel)
                await viewModel.InitializeAsync();
        };

        // The title bar bypasses the disabled Close button: a migration that runs must not lose its window, its sources and the
        // refresh of the project window. Reading the server or checking the feed may be abandoned.
        Closing += (_, e) =>
        {
            if (DataContext is MigrationViewModel { IsMigrating: true })
                e.Cancel = true;
        };

        // Closing with the title bar bypasses the view model's commands; the downloads of the review go either way.
        Closed += (_, _) => (DataContext as MigrationViewModel)?.Dispose();
    }
}
