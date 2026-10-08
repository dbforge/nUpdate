using System.Drawing;
using System.Windows.Forms;

namespace nUpdate.UI.WindowsForms;

/// <summary>The icon of the host application's executable, shown in the dialogs' title bars.</summary>
internal static class ApplicationIcon
{
    private static Icon? _icon;
    private static bool _loaded;

    public static Icon? Get()
    {
        if (_loaded)
            return _icon;

        try
        {
            _icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }
        catch (Exception exception) when (exception is ArgumentException or IOException or UnauthorizedAccessException)
        {
            _icon = null;
        }

        _loaded = true;
        return _icon;
    }
}
