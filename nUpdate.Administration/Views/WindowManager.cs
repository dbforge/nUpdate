// Author: Dominic Beger (Trade/ProgTrade) 2017

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace nUpdate.Administration.Views
{
    public static class WindowManager
    {
        public static ExtendedWindow GetCurrentWindow()
        {
            return Application.Current.Windows.Cast<ExtendedWindow>().SingleOrDefault(w => w.IsActive);
        }

        // ReSharper disable SuggestBaseTypeForParameter
        private static bool? InternalShowModalWindow(ExtendedWindow window)
            // ReSharper restore SuggestBaseTypeForParameter
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

        public static bool? ShowModalWindow(ExtendedWindow window)
        {
            return InternalShowModalWindow(window);
        }

        public static bool? ShowModalWindow<T>(Action<T> action = null) where T : ExtendedWindow, new()
        {
            var window = Activator.CreateInstance<T>();
            action?.Invoke(window);

            return InternalShowModalWindow(window);
        }

        public static bool? ShowModalWindow<TWindowType, TDataContextType>(
            Func<TWindowType, TDataContextType> func, Action<TWindowType> action = null)
            where TWindowType : ExtendedWindow, new()
            where TDataContextType : INotifyPropertyChanged
        {
            var window = Activator.CreateInstance<TWindowType>();
            window.DataContext = func(window);
            action?.Invoke(window);

            return InternalShowModalWindow(window);
        }

        public static void ShowWindow(ExtendedWindow window)
        {
            window.Show();
        }

        public static void ShowWindow<T>() where T : ExtendedWindow, new()
        {
            var window = Activator.CreateInstance<T>();
            window.Show();
        }
    }
}