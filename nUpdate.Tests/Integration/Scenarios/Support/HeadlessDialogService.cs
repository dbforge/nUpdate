using Avalonia.Controls;
using nUpdate.Administration.Services;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views;

namespace nUpdate.Tests.Integration.Scenarios.Support;

/// <summary>
///     The dialog service of the scenarios: it opens the very same windows the application opens (modal where the
///     application is modal) and keeps a list of them so a scenario can find the dialog the user would see.
/// </summary>
public sealed class HeadlessDialogService : IDialogService
{
    private static readonly Dictionary<Type, Func<Window>> Windows = new()
    {
        [typeof(NewProjectViewModel)] = () => new NewProjectWindow(),
        [typeof(ProjectSettingsViewModel)] = () => new ProjectSettingsWindow(),
        [typeof(PackageEditorViewModel)] = () => new PackageEditorWindow(),
        [typeof(CredentialsViewModel)] = () => new CredentialsWindow(),
        [typeof(ProjectPasswordViewModel)] = () => new ProjectPasswordWindow(),
        [typeof(ProjectViewModel)] = () => new ProjectWindow(),
        [typeof(MigrationViewModel)] = () => new MigrationWindow(),
    };

    /// <summary>The main window; it owns the first dialog, every further dialog is owned by the one opened before it.</summary>
    public Window Owner { get; set; } = null!;

    /// <summary>Every window opened through this service that is still open, oldest first.</summary>
    public List<Window> Open { get; } = [];

    /// <summary>Every message window shown so far, including closed ones, so scenarios can assert what the user was told.</summary>
    public List<MessageWindow> Popups { get; } = [];

    private Window ActiveOwner => Open.LastOrDefault(w => w.IsVisible) ?? Owner;

    public async Task ShowErrorAsync(string title, string message)
    {
        var window = new MessageWindow(title, message, MessageWindow.Kind.Error, "Close", null);
        Popups.Add(window);
        await ShowDialogAsync<object?>(window);
    }

    public async Task ShowInfoAsync(string title, string message)
    {
        var window = new MessageWindow(title, message, MessageWindow.Kind.Info, "OK", null);
        Popups.Add(window);
        await ShowDialogAsync<object?>(window);
    }

    public async Task<bool> ConfirmAsync(string title, string message, string confirmText = "OK", string cancelText = "Cancel")
    {
        var window = new MessageWindow(title, message, MessageWindow.Kind.Question, confirmText, cancelText);
        Popups.Add(window);
        return await ShowDialogAsync<bool?>(window) ?? false;
    }

    public async Task<bool> ShowDialogAsync(DialogViewModel viewModel)
    {
        var window = Create(viewModel);
        viewModel.CloseRequested += (_, accepted) => window.Close(accepted);
        return await ShowDialogAsync<bool?>(window) ?? false;
    }

    public Task ShowWindowAsync(ViewModelBase viewModel)
    {
        var window = Create(viewModel);
        var closed = new TaskCompletionSource();
        window.Closed += (_, _) => closed.TrySetResult();
        if (viewModel is DialogViewModel dialog)
            dialog.CloseRequested += (_, _) => window.Close();
        Track(window);
        window.Show(ActiveOwner);
        return closed.Task;
    }

    private async Task<T?> ShowDialogAsync<T>(Window window)
    {
        Track(window);
        return await window.ShowDialog<T>(ActiveOwner);
    }

    private void Track(Window window)
    {
        Open.Add(window);
        window.Closed += (_, _) => Open.Remove(window);
    }

    private static Window Create(ViewModelBase viewModel)
    {
        var window = Windows[viewModel.GetType()]();
        window.DataContext = viewModel;
        return window;
    }
}
