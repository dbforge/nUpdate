using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views;

namespace nUpdate.Administration.Services;

/// <summary>Avalonia implementation of the dialog, picker and clipboard services.</summary>
[ExcludeFromCodeCoverage] // Needs real windows; the view models behind the dialogs are tested instead.
public sealed class DialogService : IDialogService, IFilePickerService, IClipboardService
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

    /// <summary>The main window; dialogs are owned by whichever window is active at the time they open.</summary>
    public Window? Owner { get; set; }

    public Task ShowErrorAsync(string title, string message) =>
        MessageWindow.ShowAsync(ActiveOwner, title, message, MessageWindow.Kind.Error);

    public Task ShowInfoAsync(string title, string message) =>
        MessageWindow.ShowAsync(ActiveOwner, title, message, MessageWindow.Kind.Info);

    public Task<bool> ConfirmAsync(string title, string message, string confirmText = "OK",
        string cancelText = "Cancel") =>
        MessageWindow.ConfirmAsync(ActiveOwner, title, message, confirmText, cancelText);

    public async Task<bool> ShowDialogAsync(DialogViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        var window = Create(viewModel);
        viewModel.CloseRequested += (_, accepted) => window.Close(accepted);
        return await window.ShowDialog<bool?>(ActiveOwner) ?? false;
    }

    public Task ShowWindowAsync(ViewModelBase viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        var window = Create(viewModel);
        var closed = new TaskCompletionSource();
        window.Closed += (_, _) => closed.TrySetResult();
        if (viewModel is DialogViewModel dialog)
            dialog.CloseRequested += (_, _) => window.Close();
        window.Show(ActiveOwner);
        return closed.Task;
    }

    private Window ActiveOwner =>
        (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Windows
        .FirstOrDefault(w => w.IsActive)
        ?? Owner ?? throw new InvalidOperationException("The main window is not open yet.");

    public async Task<string?> PickFileAsync(string title, params FileTypeFilter[] filters)
    {
        var files = await Storage().OpenFilePickerAsync(new FilePickerOpenOptions
        { Title = title, AllowMultiple = false, FileTypeFilter = Map(filters) });
        return files.Count > 0 ? files[0].TryGetLocalPath() : null;
    }

    public async Task<IReadOnlyList<string>> PickFilesAsync(string title, params FileTypeFilter[] filters)
    {
        var files = await Storage().OpenFilePickerAsync(new FilePickerOpenOptions
        { Title = title, AllowMultiple = true, FileTypeFilter = Map(filters) });
        return files.Select(f => f.TryGetLocalPath()).Where(p => p is not null).Cast<string>().ToList();
    }

    public async Task<string?> PickFolderAsync(string title)
    {
        var folders = await Storage().OpenFolderPickerAsync(new FolderPickerOpenOptions
        { Title = title, AllowMultiple = false });
        return folders.Count > 0 ? folders[0].TryGetLocalPath() : null;
    }

    public async Task<string?> SaveFileAsync(string title, string suggestedFileName, params FileTypeFilter[] filters)
    {
        var file = await Storage().SaveFilePickerAsync(new FilePickerSaveOptions
        { Title = title, SuggestedFileName = suggestedFileName, FileTypeChoices = Map(filters) });
        return file?.TryGetLocalPath();
    }

    public Task SetTextAsync(string text) => ActiveOwner.Clipboard?.SetTextAsync(text) ?? Task.CompletedTask;

    private static Window Create(ViewModelBase viewModel)
    {
        if (!Windows.TryGetValue(viewModel.GetType(), out var factory))
            throw new InvalidOperationException($"No window is registered for {viewModel.GetType().Name}.");
        var window = factory();
        window.DataContext = viewModel;
        return window;
    }

    private IStorageProvider Storage() => ActiveOwner.StorageProvider;

    private static List<FilePickerFileType> Map(FileTypeFilter[] filters) =>
        filters.Select(f => new FilePickerFileType(f.Name) { Patterns = f.Patterns }).ToList();
}
