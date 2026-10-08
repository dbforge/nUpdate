using System.Windows.Media;
using nUpdate.Localization;
using nUpdate.Updating;

namespace nUpdate.UI.WPF.ViewModel;

/// <summary>The view model of one dialog in the update process, hosted by a <see cref="Views.DialogWindow" />.</summary>
public abstract class DialogViewModel : ViewModelBase
{
    protected DialogViewModel(UpdateManager updateManager)
    {
        UpdateManager = updateManager ?? throw new ArgumentNullException(nameof(updateManager));
        LocProperties = updateManager.Texts;
        WindowIcon = ApplicationIcon.Load();
    }

    /// <summary>Raised with <c>true</c> when the dialog was accepted and <c>false</c> when it was cancelled.</summary>
    public event EventHandler<bool>? CloseRequested;

    internal UpdateManager UpdateManager { get; }

    public UpdateTexts LocProperties { get; }

    public ImageSource? WindowIcon { get; }

    public abstract string WindowTitle { get; }

    /// <summary>Called once the window is shown; long-running work starts here.</summary>
    internal virtual Task OnLoadedAsync() => Task.CompletedTask;

    /// <summary>Called when the user closes the window; return <c>false</c> to keep it open.</summary>
    internal virtual bool OnClosing() => true;

    protected void RequestClose(bool accepted) => CloseRequested?.Invoke(this, accepted);
}
