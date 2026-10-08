using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Input;
using nUpdate.Administration.ViewModels;

namespace nUpdate.Administration.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    [ExcludeFromCodeCoverage] // Input event glue.
    private void OnProjectDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel && viewModel.OpenSelectedCommand.CanExecute(null))
            viewModel.OpenSelectedCommand.Execute(null);
    }
}
