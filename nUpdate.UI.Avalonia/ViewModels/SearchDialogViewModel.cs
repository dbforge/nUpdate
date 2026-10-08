using System.Windows.Input;
using nUpdate.Ui;
using nUpdate.Updating;

namespace nUpdate.UI.Avalonia.ViewModels;

/// <summary>Runs the search while the dialog is open; cancelling stops the search and closes the dialog.</summary>
public sealed class SearchDialogViewModel : DialogViewModel, IDisposable
{
    private readonly Func<CancellationToken, Task<bool>> _search;
    private readonly DialogOperation<bool> _operation = new();

    internal SearchDialogViewModel(UpdateManager updateManager, Func<CancellationToken, Task<bool>> search)
        : base(updateManager)
    {
        _search = search ?? throw new ArgumentNullException(nameof(search));
        CancelCommand = new RelayCommand(_operation.Cancel);
    }

    public override string Title => Texts.Searching;

    public ICommand CancelCommand { get; }

    /// <summary>Completes with the search result once the dialog has closed; cancelled or faulted like the search.</summary>
    internal Task<bool> Completion => _operation.Completion;

    public void Dispose() => _operation.Dispose();

    public override async Task OnOpenedAsync()
    {
        await _operation.RunAsync(_search);
        RequestClose(_operation.Succeeded);
    }

    public override bool OnClosing() => _operation.TryClose();
}
