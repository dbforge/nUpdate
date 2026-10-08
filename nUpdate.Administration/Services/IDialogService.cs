using nUpdate.Administration.ViewModels;

namespace nUpdate.Administration.Services;

/// <summary>Shows messages and modal dialogs. Abstracted so view models can be tested without windows.</summary>
public interface IDialogService
{
    Task ShowErrorAsync(string title, string message);

    Task ShowInfoAsync(string title, string message);

    Task<bool> ConfirmAsync(string title, string message, string confirmText = "OK", string cancelText = "Cancel");

    /// <summary>Opens the window registered for the view model and returns whether it was accepted.</summary>
    Task<bool> ShowDialogAsync(DialogViewModel viewModel);

    /// <summary>Opens a non-modal window for the view model; the task completes when the window has closed.</summary>
    Task ShowWindowAsync(ViewModelBase viewModel);
}

/// <summary>Native file and folder pickers.</summary>
public interface IFilePickerService
{
    Task<string?> PickFileAsync(string title, params FileTypeFilter[] filters);

    Task<IReadOnlyList<string>> PickFilesAsync(string title, params FileTypeFilter[] filters);

    Task<string?> PickFolderAsync(string title);

    Task<string?> SaveFileAsync(string title, string suggestedFileName, params FileTypeFilter[] filters);
}

public sealed record FileTypeFilter(string Name, params string[] Patterns)
{
    public static FileTypeFilter Project { get; } = new("nUpdate project", "*.nupdproj");

    public static FileTypeFilter All { get; } = new("All files", "*");
}

public interface IClipboardService
{
    Task SetTextAsync(string text);
}
