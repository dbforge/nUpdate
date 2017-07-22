using System.Linq;
using System.Windows;

namespace nUpdate.Administration.Views
{
    public static class WindowManager
    {
        public static void Show(Window window)
        {
            window.Show();
        }
        
        public static bool? ShowDialog(Window window)
        {
            var currentWindow = GetCurrentWindow();
            // ReSharper disable once InvertIf
            if (currentWindow != null)
            {
                window.Owner = currentWindow;
                window.Top = currentWindow.Top + (currentWindow.Height - window.Height) / 2;
                window.Left = currentWindow.Left + (currentWindow.Width - window.Width) / 2;
            }
            
            return window.ShowDialog();
        }

        public static Window GetCurrentWindow()
        {
            return Application.Current.Windows.Cast<Window>().SingleOrDefault(w => w.IsActive);
        }
    }
}
