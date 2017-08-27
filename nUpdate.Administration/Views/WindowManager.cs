// Author: Dominic Beger (Trade/ProgTrade) 2017

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace nUpdate.Administration.Views
{
    public static class WindowManager
    {
        public static Window GetCurrentWindow()
        {
            return Application.Current.Windows.Cast<Window>().SingleOrDefault(w => w.IsActive);
        }

        private static bool? InternalShowModalWindow(Window window)
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

        public static bool? ShowModalWindow(Window window)
        {
            return InternalShowModalWindow(window);
        }

        public static bool? ShowModalWindow<T>(Action<T> action = null) where T : Window, new()
        {
            var window = Activator.CreateInstance<T>();
            action?.Invoke(window);

            return InternalShowModalWindow(window);
        }

        public static bool? ShowModalWindow<TWindowType, TDataContextType>(
            Func<TWindowType, TDataContextType> func, Action<TWindowType> action = null)
            where TWindowType : Window, new()
            where TDataContextType : INotifyPropertyChanged
        {
            var window = Activator.CreateInstance<TWindowType>();
            window.DataContext = func(window);
            action?.Invoke(window);

            return InternalShowModalWindow(window);
        }

        public static void ShowWindow(Window window)
        {
            window.Show();
        }

        public static void ShowWindow<T>() where T : Window, new()
        {
            var window = Activator.CreateInstance<T>();
            window.Show();
        }
    }
}