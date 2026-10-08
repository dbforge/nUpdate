using System.Drawing;
using System.Windows.Forms;

namespace nUpdate.UI.WindowsForms.Popups;

/// <summary>Shows a message in the style of a task dialog, optionally with the exception behind it.</summary>
internal static class Popup
{
    /// <param name="owner">The window to centre on; <c>null</c> uses the active form.</param>
    /// <param name="icon">One of the <see cref="SystemIcons" />; error, warning and question icons play their sound.</param>
    /// <param name="title">The heading.</param>
    /// <param name="message">The text.</param>
    /// <param name="exception">An exception whose full text the user can copy from the context menu.</param>
    /// <param name="buttons">The buttons to offer.</param>
    public static DialogResult Show(IWin32Window? owner, Icon icon, string title, string message, Exception? exception = null,
        PopupButtons buttons = PopupButtons.Ok)
    {
        using var dialog = new PopupDialog
        {
            PopupIcon = icon,
            Title = title,
            InfoMessage = message,
            Exception = exception,
            Buttons = buttons,
            StartPosition = FormStartPosition.CenterParent,
        };
        return dialog.ShowDialog(owner ?? Form.ActiveForm);
    }
}
