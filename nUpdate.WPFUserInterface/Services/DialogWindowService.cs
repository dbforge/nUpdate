using System.Windows;
using nUpdate.UI.Windows;
using nUpdate.WPFUserInterface.ServiceInterfaces;
using nUpdate.WPFUserInterface.ViewModel.Interfaces;

// ReSharper disable once CheckNamespace
namespace nUpdate.WPFUserInterface.Services
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