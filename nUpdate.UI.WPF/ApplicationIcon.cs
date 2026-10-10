using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using nUpdate.UI.WPF.Win32;

namespace nUpdate.UI.WPF;

/// <summary>The icon of the host application's executable, shown in the dialogs' title bars.</summary>
internal static class ApplicationIcon
{
    private static ImageSource? _icon;
    private static bool _loaded;

    public static ImageSource? Load()
    {
        if (_loaded)
            return _icon;

        _icon = Extract(new Platform.EntryAssemblyApplicationInfo().ExecutablePath);
        _loaded = true;
        return _icon;
    }

    private static BitmapSource? Extract(string? path)
    {
        if (string.IsNullOrEmpty(path) || path!.Length >= NativeMethods.MaxPath || !System.IO.File.Exists(path))
            return null;

        // The shell may rewrite the buffer with the path of the file that actually holds the icon.
        var buffer = new char[NativeMethods.MaxPath];
        path.CopyTo(0, buffer, 0, path.Length);
        ushort index = 0;
        var handle = NativeMethods.ExtractAssociatedIcon(IntPtr.Zero, buffer, ref index);
        if (handle == IntPtr.Zero)
            return null;

        try
        {
            var source =
                Imaging.CreateBitmapSourceFromHIcon(handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            source.Freeze();
            return source;
        }
        finally
        {
            _ = NativeMethods.DestroyIcon(handle);
        }
    }
}
