using System.Windows.Input;
using nUpdate.Ui;
using nUpdate.Updating;

namespace nUpdate.UI.WPF.ViewModel;

/// <summary>Runs the search while the dialog is open; cancelling closes the dialog and the search.</summary>
internal sealed class UpdateSearchViewModel : DialogViewModel, IDisposable
{
    private readonly Func<CancellationToken, Task<bool>> _search;
    private readonly DialogOperation<bool> _operation = new();

    internal UpdateSearchViewModel(UpdateManager updateManager, Func<CancellationToken, Task<bool>> search)
        : base(updateManager)
    {
        _search = search ?? throw new ArgumentNullException(nameof(search));
        CancelCommand = new RelayCommand(_operation.Cancel);
    }

    public override string WindowTitle => LocProperties.Searching;

    public ICommand CancelCommand { get; }

    /// <summary>Completes with the search result once the dialog has closed; cancelled or faulted like the search.</summary>
    internal Task<bool> Completion => _operation.Completion;

    public void Dispose() => _operation.Dispose();

    internal override async Task OnLoadedAsync()
    {
        await _operation.RunAsync(_search);
        RequestClose(_operation.Succeeded);
    }

    internal override bool OnClosing() => _operation.TryClose();
}
