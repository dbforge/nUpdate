using System.Windows.Input;

namespace nUpdate.UpdateInstaller.UI.Avalonia;

/// <summary>A command that runs an action and is always executable.</summary>
internal sealed class RelayCommand(Action execute) : ICommand
{
    public event EventHandler? CanExecuteChanged
    {
        add { }
        remove { }
    }

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) => execute();
}
