using nUpdate.Localization;
using nUpdate.Updating;

namespace nUpdate.UI.WindowsForms.Dialogs;

/// <summary>Common base of the update dialogs: carries the update manager and the host application's identity.</summary>
internal class BaseDialog : Form
{
    protected BaseDialog(UpdateManager updateManager)
    {
        UpdateManager = updateManager ?? throw new ArgumentNullException(nameof(updateManager));
    }

    protected UpdateManager UpdateManager { get; }

    protected UpdateTexts Localization => UpdateManager.Texts;

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        Text = UpdateManager.ApplicationName;
        if (ApplicationIcon.Get() is { } icon)
            Icon = icon;
    }
}
