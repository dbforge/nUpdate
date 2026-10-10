using System.Windows.Media;
using nUpdate.Localization;
using nUpdate.Updating;

namespace nUpdate.UI.WPF.ViewModel;

/// <summary>The view model of one dialog in the update process, hosted by a <see cref="Views.DialogWindow" />.</summary>
public abstract class DialogViewModel(UpdateManager updateManager) : ViewModelBase
{
    /// <summary>Raised with <c>true</c> when the dialog was accepted and <c>false</c> when it was cancelled.</summary>
    public event EventHandler<bool>? CloseRequested;

    internal UpdateManager UpdateManager { get; } =
        updateManager ?? throw new ArgumentNullException(nameof(updateManager));

    public UpdateTexts LocProperties { get; } = updateManager.Texts;

    public ImageSource? WindowIcon { get; } = ApplicationIcon.Load();

    public abstract string WindowTitle { get; }

    /// <summary>Called once the window is shown; long-running work starts here.</summary>
    internal virtual Task OnLoadedAsync() => Task.CompletedTask;

    /// <summary>Called when the user closes the window; return <c>false</c> to keep it open.</summary>
    internal virtual bool OnClosing() => true;

    protected void RequestClose(bool accepted) => CloseRequested?.Invoke(this, accepted);
}
