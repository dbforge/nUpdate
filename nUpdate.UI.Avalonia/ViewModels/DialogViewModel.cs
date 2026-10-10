using nUpdate.Localization;
using nUpdate.Updating;

namespace nUpdate.UI.Avalonia.ViewModels;

/// <summary>One dialog of the update process, hosted by <see cref="Views.UpdateDialog" />.</summary>
internal abstract class DialogViewModel(UpdateManager updateManager) : ObservableObject
{
    /// <summary>Raised with <c>true</c> when the dialog was accepted and <c>false</c> when it was cancelled.</summary>
    public event EventHandler<bool>? CloseRequested;

    internal UpdateManager UpdateManager { get; } =
        updateManager ?? throw new ArgumentNullException(nameof(updateManager));

    public UpdateTexts Texts => UpdateManager.Texts;

    public abstract string Title { get; }

    /// <summary>Called once the window is shown; long-running work starts here.</summary>
    public virtual Task OnOpenedAsync() => Task.CompletedTask;

    /// <summary>Called when the user closes the window; return <c>false</c> to keep it open.</summary>
    public virtual bool OnClosing() => true;

    protected void RequestClose(bool accepted) => CloseRequested?.Invoke(this, accepted);
}
