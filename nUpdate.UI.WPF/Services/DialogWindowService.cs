// DialogWindowService.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System.Windows;
using nUpdate.UI.WPF.ServiceInterfaces;
using nUpdate.UI.WPF.UI.Windows;
using nUpdate.UI.WPF.ViewModel.Interfaces;

// ReSharper disable once CheckNamespace
namespace nUpdate.UI.WPF.Services
{
    internal class DialogWindowService : IDialogWindowService
    {
        private DialogWindow _currentWindow;

        public void ShowDialog(string windowname, IDialogViewModel datacontext)
        {
            _currentWindow = new DialogWindow(datacontext)
            {
                Name = windowname,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStyle = WindowStyle.SingleBorderWindow,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            _currentWindow.ShowDialog();
        }

        public void CloseDialog()
        {
            _currentWindow.Close();
        }
    }
}