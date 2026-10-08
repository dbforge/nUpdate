using System.ComponentModel;
using System.Windows;
using nUpdate.UI.WPF.ViewModel;

namespace nUpdate.UI.WPF.Views;

/// <summary>Hosts one <see cref="DialogViewModel" /> and closes when it asks to.</summary>
public partial class DialogWindow : Window
{
    private readonly DialogViewModel _viewModel;

    public DialogWindow(DialogViewModel viewModel, Window? owner = null)
    {
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        InitializeComponent();
        DataContext = viewModel;
        Owner = owner;
        WindowStartupLocation = owner is null ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner;
        viewModel.CloseRequested += OnCloseRequested;
        Loaded += OnLoaded;
        Closing += OnClosing;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        await _viewModel.OnLoadedAsync();
    }

    private void OnClosing(object? sender, CancelEventArgs e)
    {
        if (_viewModel.OnClosing())
            _viewModel.CloseRequested -= OnCloseRequested;
        else
            e.Cancel = true;
    }

    private void OnCloseRequested(object? sender, bool accepted)
    {
        _viewModel.CloseRequested -= OnCloseRequested;
        if (IsLoaded)
            DialogResult = accepted; // closes a modal window
        else
            Close();
    }
}
