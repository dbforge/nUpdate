using nUpdate.Administration.Services;

namespace nUpdate.Tests.Integration.Scenarios.Support;

/// <summary>Stands in for the native file dialogs: a scenario says in advance what the user picks.</summary>
public sealed class ScriptedFilePicker : IFilePickerService
{
    private readonly Queue<string?> _files = new();
    private readonly Queue<IReadOnlyList<string>> _fileSets = new();
    private readonly Queue<string?> _folders = new();
    private readonly Queue<string?> _saveTargets = new();

    /// <summary>The titles of the dialogs that were opened, so a scenario can check which picker the user saw.</summary>
    public List<string> Shown { get; } = [];

    public ScriptedFilePicker UserPicksFile(string? path)
    {
        _files.Enqueue(path);
        return this;
    }

    public ScriptedFilePicker UserPicksFiles(params string[] paths)
    {
        _fileSets.Enqueue(paths);
        return this;
    }

    public ScriptedFilePicker UserPicksFolder(string? path)
    {
        _folders.Enqueue(path);
        return this;
    }

    public ScriptedFilePicker UserSavesAs(string? path)
    {
        _saveTargets.Enqueue(path);
        return this;
    }

    public Task<string?> PickFileAsync(string title, params FileTypeFilter[] filters) =>
        Task.FromResult(Next(_files, title));

    public Task<IReadOnlyList<string>> PickFilesAsync(string title, params FileTypeFilter[] filters) =>
        Task.FromResult(Next(_fileSets, title));

    public Task<string?> PickFolderAsync(string title) => Task.FromResult(Next(_folders, title));

    public Task<string?> SaveFileAsync(string title, string suggestedFileName, params FileTypeFilter[] filters) =>
        Task.FromResult(Next(_saveTargets, title));

    private T Next<T>(Queue<T> queue, string title)
    {
        Shown.Add(title);
        return queue.Count > 0
            ? queue.Dequeue()
            : throw new InvalidOperationException($"The scenario did not say what the user picks in \"{title}\".");
    }
}

/// <summary>Remembers what the application copied so a scenario can read the clipboard.</summary>
public sealed class MemoryClipboard : IClipboardService
{
    public string? Text { get; private set; }

    public Task SetTextAsync(string text)
    {
        Text = text;
        return Task.CompletedTask;
    }
}
